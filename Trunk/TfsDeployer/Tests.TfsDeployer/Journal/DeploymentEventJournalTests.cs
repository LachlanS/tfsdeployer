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
    }
}

