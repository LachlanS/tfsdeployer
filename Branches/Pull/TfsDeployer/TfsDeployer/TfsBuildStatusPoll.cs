using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public class TfsBuildStatusPoll
    {
        private readonly IBuildServer _buildServer;
        private readonly IDeployerFactory _deployerFactory;
        private readonly TswaClientHyperlinkService _webAccessLinks;
        private readonly ICommonStructureService _structureService;

        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly IDictionary<Uri, string> _buildQuality = new Dictionary<Uri, string>();

        private readonly TimeSpan _pollFrequency = new TimeSpan(0, 2, 0);

        public TfsBuildStatusPoll(IBuildServer buildServer, IDeployerFactory deployerFactory)
        {
            _buildServer = buildServer;
            _deployerFactory = deployerFactory;
            _webAccessLinks = _buildServer.TeamProjectCollection.GetService<TswaClientHyperlinkService>();
            _structureService = _buildServer.TeamProjectCollection.GetService<ICommonStructureService>();
            _timer = new Timer(state => PollBuildQualityChanges());
        }

        public void Start()
        {
            _timer.Change(0, (long) _pollFrequency.TotalMilliseconds);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        private void PollBuildQualityChanges()
        {
            lock (_lock)
            {
                var builds = QueryAllBuilds();
                foreach (var build in builds)
                {
                    if (!_buildQuality.ContainsKey(build.Uri)) _buildQuality.Add(build.Uri, build.Quality);
                    if (string.Equals(build.Quality, _buildQuality[build.Uri])) continue;
                    
                    var changeEvent = CreateBuildStatusChangeEvent(build, _buildQuality[build.Uri]);
                    _buildQuality[build.Uri] = build.Quality;

                    var deployer = _deployerFactory.Create();
                    ThreadPool.QueueUserWorkItem(state => deployer.ExecuteDeploymentProcess(changeEvent));
                }
            }
        }

        private IEnumerable<IBuildDetail> QueryAllBuilds()
        {
            var projects = _structureService.ListProjects();
            return projects.SelectMany(project => _buildServer.QueryBuilds(project.Name));
        }

        private BuildStatusChangeEvent CreateBuildStatusChangeEvent(IBuildDetail build, string oldQuality)
        {
            var timeZone = build.BuildServer.TeamProjectCollection.TimeZone;
            return new BuildStatusChangeEvent
                       {
                           ChangedBy = build.LastChangedBy,
                           ChangedTime = build.LastChangedOn.ToString(),
                           Id = build.BuildNumber,
                           StatusChange = new Change
                                              {
                                                  FieldName = "Quality",
                                                  NewValue = build.Quality,
                                                  OldValue = oldQuality
                                              },
                           Subscriber = build.BuildServer.TeamProjectCollection.AuthorizedIdentity.DisplayName,
                           TeamFoundationServerUrl = build.BuildServer.TeamProjectCollection.Uri.ToString(),
                           TeamProject = build.TeamProject,
                           TimeZone = timeZone.IsDaylightSavingTime(build.LastChangedOn) ? timeZone.DaylightName : timeZone.StandardName,
                           TimeZoneOffset = timeZone.GetUtcOffset(build.LastChangedOn).ToString(),
                           Title = string.Format("{0} Build {1} Quality Changed To {2}", build.TeamProject, build.BuildNumber, build.Quality),
                           Url = _webAccessLinks.GetViewBuildDetailsUrl(build.Uri).ToString()
                       };
        }
    }
}