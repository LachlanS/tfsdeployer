using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Configuration;
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
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            var mappingExecutor = MockRepository.GenerateStub<IMappingExecutor>();
            Func<IMappingExecutor> executorFactory = () => mappingExecutor;
            
            var mappingProcessor = new MappingProcessor(mappingEvaluator, deploymentEventRecorder, executorFactory);
            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] { new Mapping { Script = "AScript.ps1", Queue = "AQueue" } };

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
        public void MappingProcessor_should_process_multiple_mappings_in_parallel()
        {
            // Arrange
            var mappingEvaluator = MockRepository.GenerateStub<IMappingEvaluator>();
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();

            var postDeployAction = MockRepository.GenerateStub<IPostDeployAction>();

            var buildDetail = new BuildDetail();

            var mappings = new[] { new Mapping(), new Mapping() };

            mappingEvaluator.Stub(o => o.DoesMappingApply(null, null, null))
                .IgnoreArguments()
                .Return(true);

            var statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

            int threadCount;
            using (var mappingExecutor = new ThreadCountingMappingExecutor())
            {
                Func<IMappingExecutor> executorFactory = () => mappingExecutor;
                var mappingProcessor = new MappingProcessor(mappingEvaluator, deploymentEventRecorder, executorFactory);

                // Act
                mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, 0);
                Thread.Sleep(250);
                threadCount = mappingExecutor.ConcurrentThreadCount;
                mappingExecutor.WaitHandle.Set();
            }

            // Assert
            Assert.IsTrue(threadCount > 1);
        }

        class ThreadCountingMappingExecutor : IMappingExecutor
        {
            private int _concurrentThreadCount;
            public readonly EventWaitHandle WaitHandle = new ManualResetEvent(false);

            public int ConcurrentThreadCount { get { return _concurrentThreadCount; } }

            public void Execute(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction, int deploymentId)
            {
                Interlocked.Increment(ref _concurrentThreadCount);
                WaitHandle.WaitOne();
                Interlocked.Decrement(ref _concurrentThreadCount);
            }

            public void Dispose()
            {
                WaitHandle.Dispose();
            }

        }

    }
}