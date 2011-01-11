using System;

namespace TfsDeployer.TeamFoundation
{
    [Serializable]
    public class BuildDefinition
    {
        public BuildDefinition()
        {
            Process = new ProcessTemplate();
        }

        public string DefaultDropLocation { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Id { get; set; }
        public string LastBuildUri { get; set; }
        public string LastGoodBuildLabel { get; set; }
        public Uri LastGoodBuildUri { get; set; }
        public string Name { get; set; }
        public ProcessTemplate Process { get; set; }
        public string ProcessParameters { get; set; }
    }
}