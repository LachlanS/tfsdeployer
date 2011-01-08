using System;
using System.Linq;
using TfsDeployer.Data;
using TfsDeployer.Journal;

namespace TfsDeployer.Service
{
    public class DeployerService : IDeployerService
    {
        private readonly TfsDeployerApplication _application;
        private readonly IDeploymentEventAccessor _deploymentEventAccessor;

        public DeployerService(TfsDeployerApplication application, IDeploymentEventAccessor deploymentEventAccessor)
        {
            _application = application;
            _deploymentEventAccessor = deploymentEventAccessor;
        }

        public TimeSpan GetUptime()
        {
            return DateTime.UtcNow.Subtract(_application.StartTime);
        }

        public DeploymentEvent[] RecentEvents(int count)
        {
            return _deploymentEventAccessor.Events.Take(count).ToArray();
        }
    }
}
