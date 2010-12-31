using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class DeployerTests
    {
        [TestMethod]
        public void Deployer_should_pass_BuildDetail_BuildDefinition_Name_to_ConfigurationReader()
        {
            // Arrange
            BuildDetail buildDetail = null;

            var statusChanged = new BuildStatusChangeEvent {StatusChange = new Change()};
            var mappingProcessor = MockRepository.GenerateStub<IMappingProcessor>();

            var tfsBuildDetail = new StubBuildDetail {BuildDefinition = {Name = "foo"}};
            var buildServer = MockRepository.GenerateStub<IBuildServer>();
            buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var reader = MockRepository.GenerateStub<IConfigurationReader>();
            reader.Stub(o => o.ReadMappings(Arg<BuildDetail>.Is.Anything)).WhenCalled(m => buildDetail = (BuildDetail)m.Arguments[0]);

            Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory = (a, b) => MockRepository.GenerateStub<IPostDeployAction>();
            
            var deployer = new Deployer(reader, buildServer, mappingProcessor, postDeployActionFactory);

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged);

            // Assert
            Assert.AreEqual("foo", buildDetail.BuildDefinition.Name);
        }

        [TestMethod]
        public void Deployer_should_pass_BuildDetail_Status_to_MappingProcessor()
        {
            // Arrange
            BuildDetail buildDetail = null;

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };
            var alert = MockRepository.GenerateStub<IAlert>();
            var reader = MockRepository.GenerateStub<IConfigurationReader>();

            var tfsBuildDetail = new StubBuildDetail { Status = Microsoft.TeamFoundation.Build.Client.BuildStatus.PartiallySucceeded };
            var buildServer = MockRepository.GenerateStub<IBuildServer>();
            buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var mappingProcessor = MockRepository.GenerateStub<IMappingProcessor>();
            mappingProcessor.Stub(o => o.ProcessMappings(null, null, null, null))
                .IgnoreArguments()
                .WhenCalled(m => buildDetail = (BuildDetail)m.Arguments[2]);

            Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory = (a, b) => MockRepository.GenerateStub<IPostDeployAction>();

            var deployer = new Deployer(reader, buildServer, mappingProcessor, postDeployActionFactory);

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged);

            // Assert
            Assert.AreEqual(global::TfsDeployer.TeamFoundation.BuildStatus.PartiallySucceeded, buildDetail.Status);
        }

    }
}
