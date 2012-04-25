using System;

namespace TfsDeployer.DeployAgent
{
    [Serializable]
    public class DeployAgentResult
    {
        public bool HasErrors { get; set; }
        public string Output { get; set; }
    }
}