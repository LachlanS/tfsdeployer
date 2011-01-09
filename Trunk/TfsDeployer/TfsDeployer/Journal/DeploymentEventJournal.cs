using System;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Journal
{
    public class DeploymentEventJournal : IDeploymentEventRecorder, IDeploymentEventAccessor
    {
        private readonly object _eventsLock = new object();
        private readonly IList<DeploymentEvent> _events = new List<DeploymentEvent>();

        public int RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality)
        {
            var deploymentEvent = new DeploymentEvent
                                      {
                                          BuildNumber = buildNumber,
                                          TeamProject = teamProject,
                                          TeamProjectCollectionUri = teamProjectCollectionUri,
                                          TriggeredUtc = DateTime.UtcNow,
                                          TriggeredBy = triggeredBy,
                                          OriginalQuality = originalQuality,
                                          NewQuality = newQuality,
                                          QueuedDeployments = new QueuedDeployment[0]
                                      };

            lock (_eventsLock)
            {
                _events.Add(deploymentEvent);
                return _events.Count - 1;
            }

        }

        public int RecordQueued(int eventId, string script, string queue)
        {
            var queuedDeployment = new QueuedDeployment
                                       {
                                           Script = script,
                                           Queue = queue,
                                           QueuedUtc = DateTime.UtcNow
                                       };
            
            lock (_eventsLock)
            {
                var queuedDeployments = new List<QueuedDeployment>(_events[eventId].QueuedDeployments) {queuedDeployment};
                _events[eventId].QueuedDeployments = queuedDeployments.ToArray();
            }
            return 0;
        }

        public IEnumerable<DeploymentEvent> Events { get { return _events; } }
    }
}
