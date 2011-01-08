using System;
using TfsDeployer.Data;

namespace TfsDeployer.Service
{
    public class DeployerService : IDeployerService
    {
        private readonly TfsDeployerApplication _application;

        public DeployerService(TfsDeployerApplication application)
        {
            _application = application;
        }

        public TimeSpan GetUptime()
        {
            return DateTime.UtcNow.Subtract(_application.StartTime);
        }
    }
}
