using System.Management.Automation.Host;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class DeploymentHostRawUITests
    {
        [TestMethod]
        public void DeploymentHostRawUI_should_default_to_BufferSize_with_width_100()
        {
            //Arrange
            //Act
            var rawui = new DeploymentHostRawUi();

            //Assert
            Assert.AreEqual(100, rawui.BufferSize.Width);
        }

        [TestMethod]
        public void DeploymentHostRawUI_should_accept_a_new_BufferSize_width()
        {
            //Arrange
            var rawui = new DeploymentHostRawUi();

            //Act
            rawui.BufferSize = new Size(101, 0);

            //Assert
            Assert.AreEqual(101, rawui.BufferSize.Width);
        }

        [TestMethod]
        public void DeploymentHostRawUI_should_override_a_new_BufferSize_width_less_than_80()
        {
            //Arrange
            var rawui = new DeploymentHostRawUi();

            //Act
            rawui.BufferSize = new Size(50, 0);

            //Assert
            Assert.AreEqual(80, rawui.BufferSize.Width);
        }

    }
}
