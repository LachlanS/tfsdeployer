using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Journal;

namespace Tests.TfsDeployer.Journal
{
    [TestClass]
    public class DeploymentEventJournalTests
    {
        [TestMethod]
        public void DeploymentEventJournal_should_add_recorded_event_to_events_enumerable()
        {
            // Arrange 
            var journal = new DeploymentEventJournal();

            // Act
            journal.RecordTriggered("Foobar_123.1", "Foo", "https://foo/tfs/foo");
            var deploymentEvent = journal.Events.First();

            // Assert
            Assert.AreEqual("Foobar_123.1", deploymentEvent.BuildNumber);
        }
    }
}

