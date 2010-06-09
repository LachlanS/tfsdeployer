using System;
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

        public bool DownloadDeploymentFile(IBuildDetail buildDetail, string destination)
        {
            var deploymentFile = GetDeploymentMappingsFileServerPath(buildDetail);

            try
            {
                _versionControlServer.DownloadFile(deploymentFile, destination);
                return true;
            }
            catch (VersionControlException)
            {
                // file not found
                return false;
            }
        }
        
        private static string GetDeploymentMappingsFileServerPath(IBuildDetail buildDetail)
        {
            var folder = VersionControlPath.GetDeploymentFolderServerPath(buildDetail);
            return folder + "/DeploymentMappings.xml";
        }
    }
}