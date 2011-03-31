using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;

namespace TfsDeployer
{
    public class DeployerFactory : IDeployerFactory
    {
        private readonly IBuildServer _buildServer;
        private readonly IConfigurationReader _configurationReader;
        private readonly IDeploymentFolderSource _deploymentFolderSource;

        public DeployerFactory(IBuildServer buildServer, IConfigurationReader configurationReader, IDeploymentFolderSource deploymentFolderSource)
        {
            _buildServer = buildServer;
            _configurationReader = configurationReader;
            _deploymentFolderSource = deploymentFolderSource;
        }

        public IDeployer Create()
        {
            var deployAgentProvider = new DeployAgentProvider();
            var emailAlerter = new EmailAlerter();
            var mappingEvaluator = new MappingEvaluator();
            var mappingProcessor = new MappingProcessor(deployAgentProvider, _deploymentFolderSource, mappingEvaluator);
            return new Deployer(_configurationReader, emailAlerter, _buildServer, mappingProcessor);
        }
    }
}