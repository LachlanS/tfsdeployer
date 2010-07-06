using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    public interface IDeploymentFileSource
    {
        bool DownloadDeploymentFile(IBuildDetail buildDetail, string destination);
    }
}