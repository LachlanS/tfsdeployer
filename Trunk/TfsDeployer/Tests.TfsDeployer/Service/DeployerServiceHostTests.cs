using System;
using System.ServiceModel;
using System.Threading;
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
            var startTime = TfsDeployerApplication.StartTime;
            Thread.Sleep(100);

            // Act
            TimeSpan result;
            using (var host = new DeployerServiceHost(new Uri(address)))
            {
                var channel =
                    ChannelFactory<IDeployerService>.CreateChannel(
                        new WSHttpBinding {Security = {Mode = SecurityMode.None}},
                        new EndpointAddress(address + "/IDeployerService"));
                result = channel.GetUptime();
            }

            // Assert
            Assert.AreNotEqual(0, result.TotalMilliseconds);
        }
    }
}
