using System;

namespace TfsDeployer.Data
{
    [Serializable]
    public class DeploymentOutput
    {
        public string Content { get; set; }
        public bool IsFinal { get; set; }
    }
}