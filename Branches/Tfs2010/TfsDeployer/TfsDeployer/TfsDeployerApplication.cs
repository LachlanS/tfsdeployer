using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsDeployer
{
    public class TfsDeployerApplication
    {
        private TfsBuildStatusTrigger _trigger;

        public void Start()
        {
            if (_trigger != null) throw new InvalidOperationException("Already started.");
            var serverProvider = new AppConfigTeamFoundationServerProvider();
            var server = serverProvider.GetServer();
            var eventService = server.GetService<IEventService>();
            var buildServer = server.GetService<IBuildServer>();
            var versionControlServer = server.GetService<VersionControlServer>();
            var baseAddress = new Uri(Properties.Settings.Default.BaseAddress);
            _trigger = new TfsBuildStatusTrigger(eventService, new DeployerFactory(buildServer, versionControlServer), baseAddress);
            _trigger.Start();
        }

        public void Stop()
        {
            if (_trigger == null) throw new InvalidOperationException("Not started.");
            _trigger.Stop();
            _trigger = null;
        }
    }
}