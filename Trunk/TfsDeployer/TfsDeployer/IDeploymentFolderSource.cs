using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    public interface IDeploymentFolderSource
    {
        void DownloadDeploymentFolder(IBuildDetail buildDetail, string destination);
    }
}