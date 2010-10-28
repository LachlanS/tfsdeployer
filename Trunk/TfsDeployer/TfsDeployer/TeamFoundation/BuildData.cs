using System;

namespace TfsDeployer.TeamFoundation
{
    [Serializable]
    public class BuildData : IBuildData
    {
        public string BuildMachine { get; set; }
        public string BuildNumber { get; set; }
        public string BuildQuality { get; set; }
        public string BuildStatus { get; set; }
        public int BuildStatusId { get; set; }
        public string BuildType { get; set; }
        public string BuildTypeFileUri { get; set; }
        public string BuildUri { get; set; }
        public string DropLocation { get; set; }
        public DateTime FinishTime { get; set; }
        public bool GoodBuild { get; set; }
        public string LastChangedBy { get; set; }
        public DateTime LastChangedOn { get; set; }
        public string LogLocation { get; set; }
        public string RequestedBy { get; set; }
        public DateTime StartTime { get; set; }
        public string TeamProject { get; set; }
    }
}