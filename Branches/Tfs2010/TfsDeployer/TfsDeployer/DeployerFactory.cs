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

        public Deployer Create()
        {
            var configurationSource = new VersionControlConfigurationSource(_versionControlServer, Properties.Settings.Default.ConfigurationPath);
            var configurationReader = new ConfigurationReader(configurationSource);
            var emailAlertSettings = configurationReader.ReadAlerts().Email;
            var emailAlerter = new EmailAlerter(emailAlertSettings.SmtpServer,
                                                emailAlertSettings.SenderAddress,
                                                emailAlertSettings.RecipientAddress);
            return new Deployer(configurationSource, _buildServer, emailAlerter);
        }
    }
}