using TfsDeployer;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    internal class StubDeploymentFolderSource : IDeploymentFolderSource
    {
        public void DownloadDeploymentFolder(BuildDetail buildDetail, string destination)
        {
            //NOOP
        }
    }
}