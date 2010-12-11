using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class MappingProcessorTests
    {
        [TestMethod]
        public void MappingProcessor_should_call_post_deploy_action_when_script_not_specified()
        {
            // Arrange
            var deployAgentProvider = new DeployAgentProvider();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            //var alert = MockRepository.GenerateStub<IAlert>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            //var buildServer = MockRepository.GenerateStub<IBuildServer>();
            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();
            //var buildDetail = new StubBuildDetail();
            //((IBuildDetail)buildDetail).KeepForever = false;
            //buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
            //    .IgnoreArguments()
            //    .Return(buildDetail);

            var mappings = new[] {new Mapping {RetainBuildSpecified = true, RetainBuild = true}};

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction);

            // Assert
            postDeployAction.AssertWasCalled(o => o.DeploymentFinished(
                Arg<Mapping>.Is.Equal(mappings[0]), 
                Arg<DeployAgentResult>.Matches(result => !result.HasErrors))
                );
            //Assert.AreEqual(true, ((IBuildDetail)buildDetail).KeepForever, "KeepForever");
            //Assert.AreEqual(1, buildDetail.SaveCount, "Save()");
        }

    }
}
