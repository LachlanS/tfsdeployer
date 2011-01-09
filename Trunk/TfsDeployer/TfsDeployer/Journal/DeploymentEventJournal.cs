using System;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Journal
{
    public class DeploymentEventJournal : IDeploymentEventRecorder, IDeploymentEventAccessor
    {
        private const int EventIdBitShift = 5;

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
                return (eventId << EventIdBitShift) + _events[eventId].QueuedDeployments.Length - 1;
            }
        }

        public void RecordStarted(int deploymentId)
        {
            var queuedDeployment = GetQueuedDeployment(deploymentId);
            queuedDeployment.StartedUtc = DateTime.UtcNow;
        }

        public void RecordFinished(int deploymentId, bool hasErrors)
        {
            var queuedDeployment = GetQueuedDeployment(deploymentId);
            queuedDeployment.HasErrors = hasErrors;
            queuedDeployment.FinishedUtc = DateTime.UtcNow;

        }

        private QueuedDeployment GetQueuedDeployment(int deploymentId)
        {
            var eventId = deploymentId >> EventIdBitShift;
            var deploymentIndex = eventId & (2 ^ EventIdBitShift - 1);
            return _events[eventId].QueuedDeployments[deploymentIndex];
        }

        public IEnumerable<DeploymentEvent> Events { get { return _events; } }
    }
}
