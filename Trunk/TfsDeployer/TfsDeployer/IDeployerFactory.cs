namespace TfsDeployer
{
    public interface IDeployerFactory
    {
        IDeployer Create();
    }
}