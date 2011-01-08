using System;

namespace TfsDeployer.Data
{
    [Serializable]
    public class DeploymentEvent
    {
        public DateTime Triggered { get; set; }
        public string BuildNumber { get; set; }
        public string TeamProject { get; set; }
        public string TeamProjectCollectionUri { get; set; }
    }
}