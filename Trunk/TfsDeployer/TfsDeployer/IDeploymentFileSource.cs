using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public interface IDeploymentFileSource
    {
        bool DownloadDeploymentFile(BuildDetail buildDetail, string destination);
    }
}