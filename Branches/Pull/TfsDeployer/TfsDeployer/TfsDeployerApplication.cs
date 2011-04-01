using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Listener;
using TfsDeployer.Configuration;
using TfsDeployer.Properties;

namespace TfsDeployer
{
    public class TfsDeployerApplication : IDisposable
    {
        public static readonly DateTime StartTime = DateTime.UtcNow;

        private TfsBuildStatusTrigger _trigger;
        private TfsBuildStatusPoll _poll;

        public void Start()
        {
            if (_trigger != null) throw new InvalidOperationException("Already started.");

            var listenPrefix = Properties.Settings.Default.BaseAddress;
            if (!listenPrefix.EndsWith("/")) listenPrefix += "/";

            var serverProvider = new AppConfigTfsConnectionProvider();
            var server = serverProvider.GetConnection();
            var eventService = server.GetService<IEventService>();
            var buildServer = server.GetService<IBuildServer>();
            var versionControlServer = server.GetService<VersionControlServer>();
            var configurationReader = new ConfigurationReader(new VersionControlDeploymentFileSource(versionControlServer), Properties.Settings.Default.KeyFile);
            var deploymentFolderSource = new VersionControlDeploymentFolderSource(versionControlServer);
            var baseAddress = new Uri(listenPrefix + "event/");
            var listener = new TfsListener(eventService, baseAddress);
            var duplicateEventDetector = new DuplicateEventDetector();

            var deployerFactory = new DeployerFactory(buildServer, configurationReader, deploymentFolderSource);
            _trigger = new TfsBuildStatusTrigger(listener, deployerFactory, duplicateEventDetector);
            _poll = new TfsBuildStatusPoll(buildServer, deployerFactory);

            _trigger.Start();
            if (Settings.Default.UsePolling) _poll.Start();
        }

        #region IDisposable Members

        private bool _isDisposed = false;

        ~TfsDeployerApplication()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_isDisposed) return;

            if (disposeManagedResources)
            {
                TraceHelper.TraceVerbose(TraceSwitches.TfsDeployer, "{0}.Dispose({1})", GetType(), disposeManagedResources);
                try
                {
                    if (_trigger != null)
                    {
                        _trigger.Stop();
                        _trigger = null;
                    }
                    if (_poll != null)
                    {
                        _poll.Stop();
                        _poll = null;
                    }
                }
                catch (Exception)
                {
                    // swallow. Clean up no matter what.
                }
            }

            _isDisposed = true;
        }

        #endregion
    }
}