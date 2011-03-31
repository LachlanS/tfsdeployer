using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.Alert
{
    [TestClass]
    public class EmailAlerterTests
    {
        [TestMethod]
        public void EmailAlerter_should_return_silently_if_mapping_notification_address_is_blank()
        {
            // Arrange
            var mapping = new Mapping();
            var build = new BuildDetail();
            var deployAgentResult = new DeployAgentResult();
            var emailAlerter = new EmailAlerter();

            mapping.NotificationAddress = string.Empty;

            // Act
            emailAlerter.Alert(mapping, build, deployAgentResult);
            
            // Assert
            // no exception
        }
    }
}
