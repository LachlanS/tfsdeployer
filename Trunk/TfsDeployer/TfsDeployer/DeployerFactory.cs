using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;

namespace TfsDeployer
{
    public class DeployerFactory : IDeployerFactory
    {
        private readonly IBuildServer _buildServer;
        private readonly VersionControlServer _versionControlServer;

        public DeployerFactory(IBuildServer buildServer, VersionControlServer versionControlServer)
        {
            _buildServer = buildServer;
            _versionControlServer = versionControlServer;
        }

        public IDeployer Create()
        {
            var deploymentFolderSource = new VersionControlDeploymentFolderSource(_versionControlServer);
            var deploymentFileSource = new VersionControlDeploymentFileSource(_versionControlServer);
            return new Deployer(deploymentFileSource, deploymentFolderSource, _buildServer);
        }
    }
}