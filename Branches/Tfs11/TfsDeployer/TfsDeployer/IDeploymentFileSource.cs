using System.IO;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public interface IDeploymentFileSource
    {
        Stream DownloadDeploymentFile(BuildDetail buildDetail);
    }
}