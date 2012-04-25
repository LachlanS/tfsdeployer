using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class MappingExecutorTests
    {
        [TestMethod]
        public void MappingExecutor_should_record_started_time()
        {
            // Arrange
            const int eventId = 7;
            const int deploymentId = 23;
            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            deploymentEventRecorder.Stub(o => o.RecordQueued(eventId, mapping.Script, mapping.Queue))
                .Return(deploymentId);

            var namedLockSet = new NamedLockSet();

            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();
            var buildDetail = new BuildDetail();
            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            var mappingExecutor = new MappingExecutor(deploymentEventRecorder, deployAgentProvider, deploymentFolderSource, namedLockSet);

            // Act
            mappingExecutor.Execute(statusChanged, buildDetail, mapping, postDeployAction, deploymentId);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordStarted(deploymentId));
        }

        [TestMethod]
        public void MappingExecutor_should_record_finished_time_and_errors_and_final_output()
        {
            // Arrange
            const int deploymentId = 23;
            var deployAgentResult = new DeployAgentResult { HasErrors = true, Output = "Done!" };

            var deployAgent = MockRepository.GenerateStub<IDeployAgent>();
            deployAgent.Stub(o => o.Deploy(null))
                .IgnoreArguments()
                .Return(deployAgentResult);

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(null))
                .IgnoreArguments()
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();

            var namedLockSet = new NamedLockSet();

            var mappingExecutor = new MappingExecutor(deploymentEventRecorder, deployAgentProvider, deploymentFolderSource, namedLockSet);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingExecutor.Execute(statusChanged, buildDetail, mapping, postDeployAction, deploymentId);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordFinished(deploymentId, deployAgentResult.HasErrors, deployAgentResult.Output));
        }

        [TestMethod]
        public void MappingExecutor_should_pass_deployment_id_to_deploy_agent_via_deploy_agent_data()
        {
            // Arrange
            const int deploymentId = 23;

            DeployAgentData deployData = null;
            var deployAgent = MockRepository.GenerateStub<IDeployAgent>();
            deployAgent.Stub(o => o.Deploy(null))
                .IgnoreArguments()
                .Return(new DeployAgentResult())
                .WhenCalled(o => deployData = (DeployAgentData)o.Arguments[0]);

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(null))
                .IgnoreArguments()
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            deploymentEventRecorder.Stub(o => o.RecordQueued(0, null, null))
                .IgnoreArguments()
                .Return(deploymentId);

            var namedLockSet = new NamedLockSet();

            var mappingExecutor = new MappingExecutor(deploymentEventRecorder, deployAgentProvider, deploymentFolderSource, namedLockSet);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingExecutor.Execute(statusChanged, buildDetail, mapping, postDeployAction, deploymentId);

            // Assert
            Assert.AreEqual(deploymentId, deployData.DeploymentId);
        }

        [TestMethod]
        public void MappingExecutor_should_call_post_deploy_action_when_script_not_specified()
        {
            // Arrange
            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var namedLockSet = new NamedLockSet();
            var mappingExecutor = new MappingExecutor(deploymentEventRecorder, deployAgentProvider, deploymentFolderSource, namedLockSet);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mapping = new Mapping { RetainBuildSpecified = true, RetainBuild = true };

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingExecutor.Execute(statusChanged, buildDetail, mapping, postDeployAction, 0);

            // Assert
            postDeployAction.AssertWasCalled(o => o.DeploymentFinished(
                Arg<Mapping>.Is.Equal(mapping),
                Arg<DeployAgentResult>.Matches(result => !result.HasErrors))
                );
        }


    }
}