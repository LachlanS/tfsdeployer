using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class DeployerTests
    {
        [TestMethod]
        public void Deployer_should_apply_retain_build_when_script_not_specified()
        {
            // Arrange
            var deployAgentProvider = new DeployAgentProvider();
            var configurationReader = MockRepository.GenerateStub<IConfigurationReader>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var alert = MockRepository.GenerateStub<IAlert>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var buildServer = MockRepository.GenerateStub<IBuildServer>();

            var buildDetail = new StubBuildDetail();
            ((IBuildDetail)buildDetail).KeepForever = false;
            buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
                .IgnoreArguments()
                .Return(buildDetail);

            var mapping = new Mapping {RetainBuildSpecified = true, RetainBuild = true};
            configurationReader.Stub(o => o.ReadMappings(null))
                .IgnoreArguments()
                .Return(new[] {mapping});

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var deployer = new Deployer(deployAgentProvider, configurationReader, deploymentFolderSource, alert, mappingEvaluator, buildServer);
            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged);

            // Assert
            Assert.AreEqual(true, ((IBuildDetail)buildDetail).KeepForever, "KeepForever");
            Assert.AreEqual(1, buildDetail.SaveCount, "Save()");
        }
    }
}
