namespace TfsDeployer.DeployAgent
{
    public interface IDeployAgentProvider
    {
        IDeployAgent GetDeployAgent(Mapping mapping);
    }
}