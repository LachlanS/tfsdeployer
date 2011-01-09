using System;
using System.Threading;
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
    public class MappingProcessorTests
    {
        [TestMethod]
        public void MappingProcessor_should_record_started_time()
        {
            // Arrange
            const int eventId = 7;
            const int deploymentId = 23;
            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            deploymentEventRecorder.Stub(o => o.RecordQueued(eventId, mapping.Script, mapping.Queue))
                .Return(deploymentId);

            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(new[] {mapping}, statusChanged, buildDetail, postDeployAction, eventId);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordStarted(deploymentId));
        }

        [TestMethod]
        public void MappingProcessor_should_record_finished_time_and_errors_and_final_output()
        {
            // Arrange
            const int eventId = 7;
            const int deploymentId = 23;
            var deployAgentResult = new DeployAgentResult {HasErrors = true, Output = "Done!"};

            var deployAgent = MockRepository.GenerateStub<IDeployAgent>();
            deployAgent.Stub(o => o.Deploy(null))
                .IgnoreArguments()
                .Return(deployAgentResult);

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(null))
                .IgnoreArguments()
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            deploymentEventRecorder.Stub(o => o.RecordQueued(eventId, mapping.Script, mapping.Queue))
                .Return(deploymentId);

            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(new[] { mapping }, statusChanged, buildDetail, postDeployAction, eventId);
            Thread.Sleep(200);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordFinished(deploymentId, deployAgentResult.HasErrors, deployAgentResult.Output));
        }

        [TestMethod]
        public void MappingProcessor_should_pass_deployment_id_to_deploy_agent_via_deploy_agent_data()
        {
            // Arrange
            const int deploymentId = 23;

            DeployAgentData deployData = null;
            var deployAgent = MockRepository.GenerateStub<IDeployAgent>();
            deployAgent.Stub(o => o.Deploy(null))
                .IgnoreArguments()
                .Return(new DeployAgentResult())
                .WhenCalled(o => deployData = (DeployAgentData) o.Arguments[0]);

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(null))
                .IgnoreArguments()
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();

            var mapping = new Mapping();

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            deploymentEventRecorder.Stub(o => o.RecordQueued(0, null, null))
                .IgnoreArguments()
                .Return(deploymentId);

            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(new[] { mapping }, statusChanged, buildDetail, postDeployAction, 0);
            Thread.Sleep(200);

            // Assert
            Assert.AreEqual(deploymentId, deployData.DeploymentId);
        }

        [TestMethod]
        public void MappingProcessor_should_record_mapped_event_for_applicable_mappings()
        {
            // Arrange
            const int eventId = 7;
            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] { new Mapping { Script = "AScript.ps1", Queue = "AQueue"} };

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, eventId);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordQueued(eventId, mappings[0].Script, mappings[0].Queue));
        }

        [TestMethod]
        public void MappingProcessor_should_call_post_deploy_action_when_script_not_specified()
        {
            // Arrange
            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] {new Mapping {RetainBuildSpecified = true, RetainBuild = true}};

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, 0);

            // Assert
            postDeployAction.AssertWasCalled(o => o.DeploymentFinished(
                Arg<Mapping>.Is.Equal(mappings[0]), 
                Arg<DeployAgentResult>.Matches(result => !result.HasErrors))
                );
        }

        [TestMethod]
        public void MappingProcessor_should_process_multiple_mappings_in_parallel()
        {
            // Arrange
            var deployAgent = new ParallelDeployAgent();

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(Arg<Mapping>.Is.Anything))
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new [] {new Mapping(), new Mapping()};

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, 0);
            var expire = DateTime.UtcNow.AddSeconds(5);
            while (!deployAgent.HasExecuted && DateTime.UtcNow < expire)
            {
                Thread.Sleep(500);
            }

            // Assert
            Assert.IsTrue(deployAgent.WasParallel);
        }

        [TestMethod]
        public void MappingProcessor_should_process_multiple_mappings_in_same_queue_in_series()
        {
            // Arrange
            var deployAgent = new ParallelDeployAgent();

            var deployAgentProvider = MockRepository.GenerateStub<IDeployAgentProvider>();
            deployAgentProvider.Stub(o => o.GetDeployAgent(Arg<Mapping>.Is.Anything))
                .Return(deployAgent);

            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] { new Mapping { Queue = "A" }, new Mapping { Queue = "A"} };

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, 0);
            var expire = DateTime.UtcNow.AddSeconds(5);
            while (!deployAgent.HasExecuted && DateTime.UtcNow < expire)
            {
                Thread.Sleep(500);
            }

            // Assert
            Assert.IsFalse(deployAgent.WasParallel);
        }

        class ParallelDeployAgent : IDeployAgent
        {
            private readonly object _lock = new object();
            private bool _executing;
            private bool _hasExecuted;
            private bool _wasParallel;

            public bool HasExecuted
            {
                get { return _hasExecuted; }
            }

            public bool WasParallel
            {
                get { return _wasParallel; }
            }

            public DeployAgentResult Deploy(DeployAgentData deployAgentData)
            {
                lock(_lock)
                {
                    if (_executing) _wasParallel = true;
                    _executing = true;
                }

                var expire = DateTime.UtcNow.AddSeconds(3);
                while (!WasParallel && DateTime.UtcNow < expire)
                {
                    Thread.Sleep(500);
                }

                lock (_lock)
                {
                    _executing = false;
                }

                _hasExecuted = true;
                return new DeployAgentResult();
            }
        }
    }
}