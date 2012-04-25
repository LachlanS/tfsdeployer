using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;

namespace TfsDeployer
{
    public interface IPostDeployAction
    {
        void DeploymentFinished(Mapping mapping, DeployAgentResult result);
    }
}