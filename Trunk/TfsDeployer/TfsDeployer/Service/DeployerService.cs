using System;
using System.ServiceModel;
using TfsDeployer.Data;

namespace TfsDeployer.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DeployerService : IDeployerService
    {
        public TimeSpan GetUptime()
        {
            return DateTime.UtcNow.Subtract(TfsDeployerApplication.StartTime);
        }
    }
}
