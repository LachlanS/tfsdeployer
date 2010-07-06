using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    internal class StubDeploymentFolderSource : IDeploymentFolderSource
    {
        public void DownloadDeploymentFolder(IBuildDetail buildDetail, string destination)
        {
            //NOOP
        }
    }
}