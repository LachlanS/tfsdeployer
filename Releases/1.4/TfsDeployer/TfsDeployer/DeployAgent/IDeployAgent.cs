namespace TfsDeployer.DeployAgent
{
    public interface IDeployAgent
    {
        DeployAgentResult Deploy(DeployAgentData deployAgentData);
    }
}