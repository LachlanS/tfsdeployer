using System;
using TfsDeployer.Data;

namespace TfsDeployer.Hosting
{
    public class DeployerContext : MarshalByRefObject, IDeployerContext
    {
        public TimeSpan Uptime
        {
            get { return DateTime.UtcNow.Subtract(TfsDeployerApplication.StartTime); }
        }
    }
}
