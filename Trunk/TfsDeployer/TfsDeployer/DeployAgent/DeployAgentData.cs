using System;
using System.Collections.Generic;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer.DeployAgent
{
    public class DeployAgentData
    {
        public string NewQuality { get; set; }
        public string OriginalQuality { get; set; }
        public string DeployServer { get; set; }        
        public string DeployScriptFile { get; set; }
        public string DeployScriptRoot { get; set; }
        public TimeSpan Timeout { get; set; }
        public ICollection<DeployScriptParameter> DeployScriptParameters { get; set; }
        public BuildDetail TfsBuildDetail { get; set; }
    }
}