using System;
using TfsDeployer.Service;

namespace TfsDeployer
{
    public class TfsDeployerApplication : IDisposable
    {
        public static readonly DateTime StartTime = DateTime.UtcNow;

        private readonly TfsBuildStatusTrigger _trigger;
        private readonly DeployerServiceHost _serviceHost;

        public TfsDeployerApplication(TfsBuildStatusTrigger trigger, DeployerServiceHost serviceHost)
        {
            _trigger = trigger;
            _serviceHost = serviceHost;
        }

        public void Start()
        {
            _trigger.Start();
            _serviceHost.Start();
        }

        public void Dispose()
        {
            _trigger.Stop();
            _serviceHost.Dispose();
        }
    }
}