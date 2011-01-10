using TfsDeployer.Data;

namespace TfsDeployer.Web.Services
{
    public interface IConfigurationService
    {
        string[] GetDeployerInstanceAddress();
        IDeployerService CreateDeployerService(int instanceIndex);
        void SetDeployerInstanceAddress(string deployerServiceUrl);
    }
}