﻿using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Data;
using TfsDeployer.Journal;
using TfsDeployer.Service;

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

        [TestMethod]
        public void DeployerService_should_return_only_return_maximum_requested_recent_events()
        {
            // Arrange
            var deploymentEventAccessor = MockRepository.GenerateStub<IDeploymentEventAccessor>();
            deploymentEventAccessor.Stub(o => o.Events)
                .Return(new[] {new DeploymentEvent(), new DeploymentEvent()});

            var deployerService = new DeployerService(null, deploymentEventAccessor);

            // Act
            var events = deployerService.RecentEvents(1);

            // Assert
            Assert.AreEqual(1, events.Length);
        }
    }
}