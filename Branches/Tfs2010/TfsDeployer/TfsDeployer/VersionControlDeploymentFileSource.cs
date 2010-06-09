using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsDeployer
{
    public class VersionControlDeploymentFileSource : IDeploymentFileSource
    {
        private readonly VersionControlServer _versionControlServer;

        public VersionControlDeploymentFileSource(VersionControlServer versionControlServer)
        {
            _versionControlServer = versionControlServer;
        }

        public void DownloadDeploymentFile(IBuildDetail buildDetail, string destination)
        {
            var deploymentFile = GetDeploymentMappingsFileServerPath(buildDetail);
            _versionControlServer.DownloadFile(deploymentFile, destination);
        }
        
        private static string GetDeploymentMappingsFileServerPath(IBuildDetail buildDetail)
        {
            var folder = VersionControlPath.GetDeploymentFolderServerPath(buildDetail);
            return folder + "/DeploymentMappings.xml";
        }
    }
}