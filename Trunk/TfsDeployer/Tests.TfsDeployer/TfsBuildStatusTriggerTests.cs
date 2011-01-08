using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Listener;
using Readify.Useful.TeamFoundation.Common.Notification;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Journal;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class TfsBuildStatusTriggerTests
    {
        [TestMethod]
        public void TfsBuildStatusTrigger_should_record_unique_triggered_event()
        {
            // Arrange

            var listener = MockRepository.GenerateStub<ITfsListener>();
            var buildStatusEventRaiser = listener.GetEventRaiser(o => o.BuildStatusChangeEventReceived += null);
            var buildStatusEventArgs = new BuildStatusChangeEventArgs(
                new BuildStatusChangeEvent {Id = "Foobar_123.4", TeamProject = "Foo"},
                new TfsIdentity()
                );
            
            var deployer = MockRepository.GenerateStub<IDeployer>();
            Func<IDeployer> deployerFactory = () => deployer;
            
            var duplicateEventDetector = MockRepository.GenerateStub<IDuplicateEventDetector>();
            duplicateEventDetector.Stub(o => o.IsUnique(null))
                .IgnoreArguments()
                .Return(true);

            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();

            var trigger = new TfsBuildStatusTrigger(listener, deployerFactory, duplicateEventDetector, deploymentEventRecorder);
            trigger.Start();

            // Act
            buildStatusEventRaiser.Raise(listener, buildStatusEventArgs);

            // Assert
            deploymentEventRecorder.AssertWasCalled(o => o.RecordTriggered(
                Arg<string>.Is.Equal("Foobar_123.4"),
                Arg<string>.Is.Equal("Foo"),
                Arg<string>.Is.Anything)
                );
        }
    }
}
