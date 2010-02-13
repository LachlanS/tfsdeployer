using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

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

        public Deployer Create()
        {
            return new Deployer(new VersionControlConfigurationSource(_versionControlServer, Properties.Settings.Default.ConfigurationPath), _buildServer);
        }
    }
}