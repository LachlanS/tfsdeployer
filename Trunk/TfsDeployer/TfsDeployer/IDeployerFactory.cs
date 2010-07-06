namespace TfsDeployer
{
    public interface IDeployerFactory
    {
        Deployer Create();
    }
}