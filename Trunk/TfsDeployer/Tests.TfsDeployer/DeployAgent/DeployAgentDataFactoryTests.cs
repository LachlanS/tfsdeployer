using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class DeployAgentDataFactoryTests
    {
        [TestMethod]
        public void DeployAgentDataFactory_should_expose_event_qualities()
        {
            const string oldQuality = "QA";
            const string newQuality = "Production";

            // Arrange
            var deployScriptRoot = Path.GetTempPath();
            var mapping = new Mapping();
            var buildDetail = new BuildDetail();
            var changeEvent = new BuildStatusChangeEvent
                                  {
                                      StatusChange = new Change {OldValue = oldQuality, NewValue = newQuality}
                                  };

            var factory = new DeployAgentDataFactory();

            // Act
            var data = factory.Create(deployScriptRoot, mapping, buildDetail, changeEvent);

            // Assert
            Assert.AreEqual(oldQuality, data.OriginalQuality, "OriginalQuality");
            Assert.AreEqual(newQuality, data.NewQuality, "NewQuality");
        }
    }
}
