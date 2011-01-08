using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using TfsDeployer.Data;
using TfsDeployer.Service;

namespace Tests.TfsDeployer.Service
{
    [TestClass]
    public class DeployerServiceHostTests
    {
        [TestMethod]
        public void DeployerServiceHost_should_respond_to_client_requests()
        {
            // Arrange
            var address = string.Format("http://localhost:80/Temporary_Listen_Addresses/{0}", GetType().FullName);

            var containerBuilder = new DeployerContainerBuilder(DeployerContainerBuilder.RunMode.InteractiveConsole);
            using (var host = new DeployerServiceHost(new Uri(address), containerBuilder.Build()))
            {
                host.Start();

                var channel =
                    ChannelFactory<IDeployerService>.CreateChannel(
                        new WSHttpBinding {Security = {Mode = SecurityMode.None}},
                        new EndpointAddress(address + "/IDeployerService"));

                // Act
                channel.GetUptime();
            }

            // Assert
            // no exception
        }
    }
}
