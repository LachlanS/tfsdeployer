using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public interface IDeploymentFolderSource
    {
        void DownloadDeploymentFolder(BuildDetail buildDetail, string destination);
    }
}