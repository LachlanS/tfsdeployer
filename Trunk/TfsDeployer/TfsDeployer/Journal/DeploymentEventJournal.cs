using System;
using System.Collections;
using System.Collections.Generic;
using TfsDeployer.Data;

namespace TfsDeployer.Journal
{
    public class DeploymentEventJournal : IDeploymentEventRecorder, IDeploymentEventAccessor
    {
        private const int EventIdBitShift = 5;

        private readonly object _eventsLock = new object();
        private readonly IList<DeploymentEvent> _events = new List<DeploymentEvent>();
        private readonly IDictionary<int, Func<string>> _outputs = new Dictionary<int, Func<string>>();

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
                queuedDeployment.Id = (eventId << EventIdBitShift) + queuedDeployments.Count - 1;
                _events[eventId].QueuedDeployments = queuedDeployments.ToArray();
            }

            return queuedDeployment.Id;
        }

        public void RecordStarted(int deploymentId)
        {
            var queuedDeployment = GetQueuedDeployment(deploymentId);
            queuedDeployment.StartedUtc = DateTime.UtcNow;
        }

        public void RecordFinished(int deploymentId, bool hasErrors, string finalOutput)
        {
            var queuedDeployment = GetQueuedDeployment(deploymentId);
            queuedDeployment.HasErrors = hasErrors;
            queuedDeployment.FinishedUtc = DateTime.UtcNow;

            SetDeploymentOutputDelegate(deploymentId, new FinalOutput {Content = finalOutput}.OutputDelegate);
        }

        private class FinalOutput
        {
            public string Content { get; set; }
            public string OutputDelegate()
            {
                return Content;
            }
        }

        public void SetDeploymentOutputDelegate(int deploymentId, Func<string> outputDelegate)
        {
            lock (((ICollection)_outputs).SyncRoot)
            {
                if (_outputs.ContainsKey(deploymentId))
                {
                    _outputs[deploymentId] = outputDelegate;
                }
                else
                {
                    _outputs.Add(deploymentId, outputDelegate);
                }
            }
        }

        private QueuedDeployment GetQueuedDeployment(int deploymentId)
        {
            var eventId = deploymentId >> EventIdBitShift;
            var deploymentIndex = deploymentId & ((int)Math.Pow(2, EventIdBitShift) - 1);
            return _events[eventId].QueuedDeployments[deploymentIndex];
        }

        public IEnumerable<DeploymentEvent> Events { get { return _events; } }
        
        public DeploymentOutput GetDeploymentOutput(int deploymentId)
        {
            if (_outputs.ContainsKey(deploymentId))
            {
                var outputDelegate = _outputs[deploymentId];
                return new DeploymentOutput {Content = outputDelegate(), IsFinal = outputDelegate.Method.DeclaringType == typeof(FinalOutput)};
            }
            return new DeploymentOutput {Content = string.Empty, IsFinal = false};
        }
    }
}
