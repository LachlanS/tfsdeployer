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
        public void MappingProcessor_should_record_mapped_event_for_applicable_mappings()
        {
            // Arrange
            const int eventId = 7;
            var deployAgentProvider = new DeployAgentProvider();
            var deploymentFolderSource = MockRepository.GenerateStub<IDeploymentFolderSource>();
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var mappingProcessor = new MappingProcessor(deployAgentProvider, deploymentFolderSource, mappingEvaluator, deploymentEventRecorder);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] { new Mapping() };

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            // Act
            mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, eventId);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordMapped(eventId, mappings[0].Script));
        }

        [TestMethod]
        public void MappingProcessor_should_call_post_deploy_action_when_script_not_specified()
        {
            // Arrange
            var deployAgentProvider = new DeployAgentProvider();
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