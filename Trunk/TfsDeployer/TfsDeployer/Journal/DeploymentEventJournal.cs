using System;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Journal
{
    public class DeploymentEventJournal : IDeploymentEventRecorder, IDeploymentEventAccessor
    {
        private readonly IList<DeploymentEvent> _events = new List<DeploymentEvent>();

        public void RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri)
        {
            _events.Add(new DeploymentEvent
                            {
                                BuildNumber = buildNumber,
                                TeamProject = teamProject,
                                TeamProjectCollectionUri = teamProjectCollectionUri,
                                Triggered = DateTime.UtcNow
                            });
        }

        public IEnumerable<DeploymentEvent> Events { get { return _events; } }
    }
}
