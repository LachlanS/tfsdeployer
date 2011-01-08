using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using TfsDeployer;
using TfsDeployer.Journal;

namespace Tests.TfsDeployer.BuildStatusTriggerTests
{
    public class DuplicateEventDetectionTests
    {
        [TestClass]
        public class When_no_events_are_observed : DuplicateEventDetectionTestBase
        {
            [TestInitialize]
            public override void Setup()
            {
                base.Setup();
            }

            [TestCleanup]
            public override void TearDown()
            {
                base.TearDown();
            }

            [TestMethod]
            public void The_deployment_count_should_be_zero()
            {
                Assert.AreEqual(0, _deployerExecuteCount);
            }
        }

        [TestClass]
        public class When_a_single_event_is_observed : DuplicateEventDetectionTestBase
        {
            [TestInitialize]
            public override void Setup()
            {
                base.Setup();

                _eventRaiser.Raise(_tfsListener, new BuildStatusChangeEventArgs(_statusChanged, null));
                Thread.Sleep(1000); // TfsBuildStatusTrigger uses a delegate on a new thread to handle events, so wait until it'd have been scheduled.
            }

            [TestCleanup]
            public override void TearDown()
            {
                base.TearDown();
            }

            [TestMethod]
            public void The_deployment_count_should_be_one()
            {
                Assert.AreEqual(1, _deployerExecuteCount);
            }
        }

        [TestClass]
        public class When_multiple_events_are_observed_within_the_duplicate_detection_period : DuplicateEventDetectionTestBase
        {
            [TestInitialize]
            public override void Setup()
            {
                base.Setup();

                for (int i = 0; i < 42; i++)
                {
                    _eventRaiser.Raise(_tfsListener, new BuildStatusChangeEventArgs(_statusChanged, null));
                }
                Thread.Sleep(1000); // TfsBuildStatusTrigger uses a delegate on a new thread to handle events, so wait until it'd have been scheduled.
            }

            [TestCleanup]
            public override void TearDown()
            {
                base.TearDown();
            }

            [TestMethod]
            public void The_deployment_count_should_be_one()
            {
                Assert.AreEqual(1, _deployerExecuteCount);
            }
        }

        [TestClass]
        public class When_multiple_events_are_observed_outside_the_duplicate_detection_period : DuplicateEventDetectionTestBase
        {
            [TestInitialize]
            public override void Setup()
            {
                base.Setup();

                _eventRaiser.Raise(_tfsListener, new BuildStatusChangeEventArgs(_statusChanged, null));
                Thread.Sleep(_duplicateEventDetector.TimeoutPeriod);
                _eventRaiser.Raise(_tfsListener, new BuildStatusChangeEventArgs(_statusChanged, null));
                Thread.Sleep(1000); // TfsBuildStatusTrigger uses a delegate on a new thread to handle events, so wait until it'd have been scheduled.
            }

            [TestCleanup]
            public override void TearDown()
            {
                base.TearDown();
            }

            [TestMethod]
            public void The_deployment_count_should_be_two()
            {
                Assert.AreEqual(2, _deployerExecuteCount);
            }
        }

        public abstract class DuplicateEventDetectionTestBase
        {
            protected TfsBuildStatusTrigger _buildStatusTrigger;
            protected ITfsListener _tfsListener;
            protected IDuplicateEventDetector _duplicateEventDetector;

            protected IEventRaiser _eventRaiser;
            protected BuildStatusChangeEvent _statusChanged;
            protected int _deployerExecuteCount;

            public virtual void Setup()
            {
                _tfsListener = MockRepository.GenerateStub<ITfsListener>();

                // configure deployer
                var deployer = MockRepository.GenerateStub<IDeployer>();

                deployer.Stub(o => o.ExecuteDeploymentProcess(null))
                    .IgnoreArguments()
                    .WhenCalled(mi => _deployerExecuteCount++)
                    .Repeat.Any();

                _tfsListener.BuildStatusChangeEventReceived += null;
                _eventRaiser = _tfsListener.GetEventRaiser(mo => mo.BuildStatusChangeEventReceived += null);

                // we're using a real DuplicateEventDetector
                _duplicateEventDetector = new DuplicateEventDetector();

                _statusChanged = new BuildStatusChangeEvent { StatusChange = new Change() };

                var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();

                _deployerExecuteCount = 0;

                _buildStatusTrigger = new TfsBuildStatusTrigger(_tfsListener, () => deployer, _duplicateEventDetector, deploymentEventRecorder);
                _buildStatusTrigger.Start();
            }

            public virtual void TearDown()
            {
                _buildStatusTrigger.Stop();
            }
        }
    }
}
