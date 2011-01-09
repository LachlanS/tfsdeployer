using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Journal;

namespace Tests.TfsDeployer.Journal
{
    [TestClass]
    public class DeploymentEventJournalTests
    {
        [TestMethod]
        public void DeploymentEventJournal_should_add_all_recorded_event_details_to_events_enumerable()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();

            // Act
            journal.RecordTriggered("Foobar_123.1", "Foo", "https://foo/tfs/foo", @"Domain\MrMcGoo", "Staging", "Production");
            var deploymentEvent = journal.Events.First();

            // Assert
            Assert.AreEqual("Foobar_123.1", deploymentEvent.BuildNumber);
            Assert.AreEqual("Foo", deploymentEvent.TeamProject);
            Assert.AreEqual("https://foo/tfs/foo", deploymentEvent.TeamProjectCollectionUri);
            Assert.AreEqual(@"Domain\MrMcGoo", deploymentEvent.TriggeredBy);
            Assert.AreEqual("Staging", deploymentEvent.OriginalQuality);
            Assert.AreEqual("Production", deploymentEvent.NewQuality);
        }

        [TestMethod]
        public void DeploymentEventJournal_should_provide_a_unique_id_for_each_trigger_recorded()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();

            // Act
            var firstEventId = journal.RecordTriggered("Foobar_123.1", null, null, null, null, null);
            var secondEventId = journal.RecordTriggered("Foobar_123.2", null, null, null, null, null);

            // Assert
            Assert.AreNotEqual(firstEventId, secondEventId);
        }

        [TestMethod]
        public void DeploymentEventJournal_should_record_queued_script_and_queue_and_time_against_triggered_event()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.8", null, null, null, null, null);
            var deploymentEvent = journal.Events.First();

            // Act
            journal.RecordQueued(eventId, "Foo.ps1", "QueueCumber");
            var mapped = deploymentEvent.QueuedDeployments[0];

            // Assert
            Assert.AreEqual("Foo.ps1", mapped.Script);
            Assert.AreEqual("QueueCumber", mapped.Queue);
            Assert.IsTrue(DateTime.UtcNow.Subtract(mapped.QueuedUtc).TotalSeconds < 1, "QueuedUtc is not recent.");
        }

        [TestMethod]
        public void DeploymentEventJournal_should_provide_a_unique_id_for_each_queued_deployment_recorded()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.1", null, null, null, null, null);

            // Act
            var firstDeploymentId = journal.RecordQueued(eventId, "Foo.ps1", "A");
            var secondDeploymentId = journal.RecordQueued(eventId, "Bar.ps1", "B");

            // Assert
            Assert.AreNotEqual(firstDeploymentId, secondDeploymentId, "first and second deployment ids are equal.");
        }

        [TestMethod]
        public void DeploymentEventJournal_should_provide_include_deployment_id_in_events_model()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            journal.RecordTriggered("Foobar_123.5", null, null, null, null, null); // an extra event to ensure the deploymentId != 0
            var eventId = journal.RecordTriggered("Foobar_123.1", null, null, null, null, null);
            var deploymentEvent = journal.Events.Last();

            // Act
            var deploymentId = journal.RecordQueued(eventId, "Foo.ps1", "A");
            var queuedDeployment = deploymentEvent.QueuedDeployments[0];

            // Assert
            if (deploymentId == 0) Assert.Inconclusive("Cannot determine if Id property is set if it is expected to be zero.");
            Assert.AreEqual(deploymentId, queuedDeployment.Id);
        }

        [TestMethod]
        public void DeploymentEventJournal_should_record_start_time_against_queued_deployment()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.2", null, null, null, null, null);
            var deploymentId = journal.RecordQueued(eventId, "Foo.ps1", "QueueCumber");
            var deploymentEvent = journal.Events.First();

            // Act
            journal.RecordStarted(deploymentId);
            var queuedDeployment = deploymentEvent.QueuedDeployments[0];

            // Assert
            Assert.IsNotNull(queuedDeployment.StartedUtc, "StartedUtc is null.");
            Assert.IsTrue(DateTime.UtcNow.Subtract(queuedDeployment.StartedUtc.Value).TotalSeconds < 1, "StartedUtc is not recent.");
        }

        [TestMethod]
        public void DeploymentEventJournal_should_record_finished_time_and_errors_against_queued_deployment()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.2", null, null, null, null, null);
            var deploymentId = journal.RecordQueued(eventId, "Foo.ps1", "QueueCumber");
            var deploymentEvent = journal.Events.First();

            // Act
            journal.RecordFinished(deploymentId, true, null);
            var queuedDeployment = deploymentEvent.QueuedDeployments[0];

            // Assert
            Assert.IsTrue(queuedDeployment.HasErrors, "HasErrors is false.");
            Assert.IsNotNull(queuedDeployment.FinishedUtc, "FinishedUtc is null");
            Assert.IsTrue(DateTime.UtcNow.Subtract(queuedDeployment.FinishedUtc.Value).TotalSeconds < 1, "FinishedUtc is not recent.");
        }

        [TestMethod]
        public void DeploymentEventJournal_should_record_finished_output_against_queued_deployment()
        {
            // Arrange 
            const string expectedOutput = "Goodbye!";
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.2", null, null, null, null, null);
            var deploymentId = journal.RecordQueued(eventId, "Foo.ps1", "QueueCumber");

            // Act
            journal.RecordFinished(deploymentId, true, expectedOutput);
            var actualOutput = journal.GetDeploymentOutput(deploymentId);

            // Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

    }
}