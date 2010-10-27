using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeployerTests
{
    [TestClass]
    public class DuplicateEventDetectionIntegrationTests
    {
        [TestMethod]
        public void Only_the_first_matching_event_should_trigger_a_deployment()
        {
            // Arrange
            var deployAgentProvider = new DeployAgentProvider();
            var configurationReader = MockRepository.GenerateStub<IConfigurationReader>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var alert = MockRepository.GenerateStub<IAlert>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var buildServer = MockRepository.GenerateStub<IBuildServer>();

            var duplicateEventDetector = new DuplicateEventDetector();

            // this one should be triggered
            var buildDetailShouldKeepForever = new StubBuildDetail();
            ((IBuildDetail)buildDetailShouldKeepForever).KeepForever = false;

            // this one should be treated as a duplicate and ignored
            var buildDetailShouldNotKeepForever = new StubBuildDetail();
            ((IBuildDetail)buildDetailShouldNotKeepForever).KeepForever = false;

            Stack<IBuildDetail> buildDetails = new Stack<IBuildDetail>(new[] { buildDetailShouldNotKeepForever, buildDetailShouldKeepForever });

            buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
                .IgnoreArguments()
                .Return(buildDetails.Pop());

            var mapping = new Mapping { RetainBuildSpecified = true, RetainBuild = true };
            configurationReader.Stub(o => o.ReadMappings(null))
                .IgnoreArguments()
                .Return(new[] { mapping });

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var deployer = new Deployer(deployAgentProvider, configurationReader, deploymentFolderSource, alert, mappingEvaluator, duplicateEventDetector, buildServer);
            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged);   // will pop the "should keep" build details
            deployer.ExecuteDeploymentProcess(statusChanged);   // will pop the "should not keep" build details

            // Assert
            Assert.AreEqual(true, ((IBuildDetail)buildDetailShouldKeepForever).KeepForever, "KeepForever");
            Assert.AreEqual(1, buildDetailShouldKeepForever.SaveCount, "Save()");

            Assert.AreEqual(false, ((IBuildDetail)buildDetailShouldNotKeepForever).KeepForever, "KeepForever");
            Assert.AreEqual(0, buildDetailShouldNotKeepForever.SaveCount, "Save()");
        }
    }
}
