using System;

namespace TfsDeployer.TeamFoundation
{
    /// <summary>
    /// Based on properties from the Microsoft.TeamFoundation.Build.Client.IBuildDetail interface from TFS 2010.
    /// </summary>
    [Serializable]
    public class BuildDetail
    {
        public BuildDetail()
        {
            BuildDefinition = new BuildDefinition();
        }

        public Uri BuildControllerUri { get; set; }
        public BuildDefinition BuildDefinition { get; set; }
        public Uri BuildDefinitionUri { get; set; }
        public bool BuildFinished { get; set; }
        public string BuildNumber { get; set; }
        public string DropLocation { get; set; }
        public string DropLocationRoot { get; set; }
        public DateTime FinishTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool KeepForever { get; set; }
        public string LabelName { get; set; }
        public string LastChangedBy { get; set; }
        public DateTime LastChangedOn { get; set; }
        public string LogLocation { get; set; }
        public string ProcessParameters { get; set; }
        public string Quality { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedFor { get; set; }
        public string ShelvesetName { get; set; }
        public string SourceGetVersion { get; set; }
        public DateTime StartTime { get; set; }
        public BuildStatus Status { get; set; }
        public string TeamProject { get; set; }
        public Uri Uri { get; set; }
    }
}
