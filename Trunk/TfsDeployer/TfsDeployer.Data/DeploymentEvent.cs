using System;

namespace TfsDeployer.Data
{
    [Serializable]
    public class DeploymentEvent
    {
        public DateTime TriggeredUtc { get; set; }
        public string TriggeredBy { get; set; }
        public string BuildNumber { get; set; }
        public string TeamProject { get; set; }
        public string TeamProjectCollectionUri { get; set; }
        public string OriginalQuality { get; set; }
        public string NewQuality { get; set; }
        public QueuedDeployment[] QueuedDeployments { get; set; }
    }
}