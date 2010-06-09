using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    public interface IDeploymentFileSource
    {
        void DownloadDeploymentFile(IBuildDetail buildDetail, string destination);
    }
}