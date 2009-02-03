using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;
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
        public ICollection<DeployScriptParameter> DeployScriptParameters { get; set; }
        public IBuildData Tfs2005BuildData { get; set; }
        public IBuildDetail Tfs2008BuildDetail { get; set; }
        //TODO generic BuildInformation
    }
}