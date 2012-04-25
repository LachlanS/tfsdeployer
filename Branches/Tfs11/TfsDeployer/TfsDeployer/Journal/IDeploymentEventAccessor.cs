using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Journal
{
    public interface IDeploymentEventAccessor
    {
        IEnumerable<DeploymentEvent> Events { get; }
        DeploymentOutput GetDeploymentOutput(int deploymentId);
    }
}