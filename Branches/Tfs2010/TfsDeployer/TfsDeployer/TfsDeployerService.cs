using System.ServiceProcess;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsDeployer
{
    public class TfsDeployerService : ServiceBase
    {
        private readonly ITeamFoundationServerProvider _serverProvider;
        private TfsBuildStatusTrigger _trigger;

        public TfsDeployerService(ITeamFoundationServerProvider serverProvider)
        {
            _serverProvider = serverProvider;
        }

        protected override void OnStart(string[] args)
        {
            var server = _serverProvider.GetServer();
            var eventService = server.GetService<IEventService>();
            var buildServer = server.GetService<IBuildServer>();
            var versionControlServer = server.GetService<VersionControlServer>();
            _trigger = new TfsBuildStatusTrigger(eventService, new DeployerFactory(buildServer, versionControlServer));
            _trigger.Start();
        }

        protected override void OnStop()
        {
            _trigger.Stop();
        }
    }
}