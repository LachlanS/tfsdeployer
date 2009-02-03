using System;

namespace TfsDeployer.TeamFoundation
{
    public class BuildDetail
    {
        public BuildAgent BuildAgent { get; set; }
        public BuildDefinition BuildDefinition { get; set; }
        public Uri BuildDefinitionUri { get; set; }
        public string BuildNumber { get; set; }
        public string DropLocation { get; set; }
        public DateTime FinishTime { get; set; }
        public string LastChangedBy { get; set; }
        public DateTime LastChangedOn { get; set; }
        public string LogLocation { get; set; }
        public string Quality { get; set; }
        public string RequestedBy { get; set; }
        public DateTime StartTime { get; set; }
        public BuildStatus Status { get; set; }
        public Uri Uri { get; set; }
    }
}