using System;

namespace TfsDeployer.TeamFoundation
{
    [Serializable]
    public class ProcessTemplate
    {
        public string Description { get; set; }
        public string Parameters { get; set; }
        public string ServerPath { get; set; }
        public string TeamProject { get; set; }
    }
}