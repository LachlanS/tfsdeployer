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
        public void DeploymentEventJournal_should_record_mapped_script_againt_triggered_event()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();
            var eventId = journal.RecordTriggered("Foobar_123.8", null, null, null, null, null);
            var deploymentEvent = journal.Events.First();

            // Act
            journal.RecordMapped(eventId, "Foo.ps1");

            // Assert
            // no exception
            // TODO rewrite test when mapped event retrieval design is done.
        }

    }
}

