using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using TfsDeployer.Data;

namespace Tests.TfsDeployer.Service
{
    [TestClass]
    public class DeployerServiceTests
    {
        [TestMethod]
        public void DeployerService_should_return_nonzero_uptime()
        {
            // Arrange
            var containerBuilder = new DeployerContainerBuilder(DeployerContainerBuilder.RunMode.InteractiveConsole);

            TimeSpan result;
            using (var container = containerBuilder.Build())
            {
                var service = container.Resolve<IDeployerService>();

                // Act
                result = service.GetUptime();
            }

            // Assert
            Assert.AreNotEqual(0, result.TotalMilliseconds);
        }
    }
}