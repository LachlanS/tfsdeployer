using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class DeployerTests
    {
        [TestMethod]
        public void Deployer_should_pass_build_definition_name_to_configuration_reader()
        {
            // Arrange
            BuildDetail buildDetail = null;

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change(), BuildUri = "vstfs:///Build/Build/1" };
            var mappingProcessor = MockRepository.GenerateStub<IMappingProcessor>();

            var tfsBuildDetail = new StubBuildDetail {BuildDefinition = {Name = "foo"}};
            var buildServer = MockRepository.GenerateStub<IBuildServer>();
            buildServer.Stub(o => o.GetBuild(null))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var reader = MockRepository.GenerateStub<IConfigurationReader>();
            reader.Stub(o => o.ReadMappings(Arg<BuildDetail>.Is.Anything)).WhenCalled(m => buildDetail = (BuildDetail)m.Arguments[0]);

            Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory = (a, b) => MockRepository.GenerateStub<IPostDeployAction>();
            
            var deployer = new Deployer(reader, buildServer, mappingProcessor, postDeployActionFactory);

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged, 0);

            // Assert
            Assert.AreEqual("foo", buildDetail.BuildDefinition.Name);
        }

        [TestMethod]
        public void Deployer_should_pass_event_id_to_mapping_processor()
        {
            // Arrange
            const int eventId = 11;

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change(), BuildUri = "vstfs:///Build/Build/1" };
            var reader = MockRepository.GenerateStub<IConfigurationReader>();

            var tfsBuildDetail = new StubBuildDetail { Status = Microsoft.TeamFoundation.Build.Client.BuildStatus.PartiallySucceeded };
            var buildServer = MockRepository.GenerateStub<IBuildServer>();
            buildServer.Stub(o => o.GetBuild(null))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var mappingProcessor = MockRepository.GenerateStub<IMappingProcessor>();

            Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory = (a, b) => MockRepository.GenerateStub<IPostDeployAction>();

            var deployer = new Deployer(reader, buildServer, mappingProcessor, postDeployActionFactory);

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged, eventId);

            // Assert
            mappingProcessor.AssertWasCalled(o => o.ProcessMappings(
                Arg<IEnumerable<Mapping>>.Is.Anything,
                Arg<BuildStatusChangeEvent>.Is.Anything,
                Arg<BuildDetail>.Is.Anything,
                Arg<IPostDeployAction>.Is.Anything,
                Arg<int>.Is.Equal(eventId)
                ));
        }

        [TestMethod]
        public void Deployer_should_pass_build_status_to_mapping_processor()
        {
            // Arrange
            BuildDetail buildDetail = null;

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change(), BuildUri = "vstfs:///Build/Build/1" };
            var reader = MockRepository.GenerateStub<IConfigurationReader>();

            var tfsBuildDetail = new StubBuildDetail { Status = Microsoft.TeamFoundation.Build.Client.BuildStatus.PartiallySucceeded };
            var buildServer = MockRepository.GenerateStub<IBuildServer>();
            buildServer.Stub(o => o.GetBuild(null))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var mappingProcessor = MockRepository.GenerateStub<IMappingProcessor>();
            mappingProcessor.Stub(o => o.ProcessMappings(null, null, null, null, 0))
                .IgnoreArguments()
                .WhenCalled(m => buildDetail = (BuildDetail)m.Arguments[2]);

            Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory = (a, b) => MockRepository.GenerateStub<IPostDeployAction>();

            var deployer = new Deployer(reader, buildServer, mappingProcessor, postDeployActionFactory);

            // Act
            deployer.ExecuteDeploymentProcess(statusChanged, 0);

            // Assert
            Assert.AreEqual(global::TfsDeployer.TeamFoundation.BuildStatus.PartiallySucceeded, buildDetail.Status);
        }

    }
}
