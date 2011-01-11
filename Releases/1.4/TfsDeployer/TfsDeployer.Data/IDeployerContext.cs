using System;

namespace TfsDeployer.Data
{
    public interface IDeployerContext
    {
        TimeSpan Uptime { get; }
    }
}
