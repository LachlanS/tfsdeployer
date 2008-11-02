using System;
using System.Collections.Generic;
using System.Text;

namespace TfsDeployer
{
    /// <summary>
    /// Based on obsolete Microsoft.TeamFoundation.Build.Proxy.BuildData.
    /// </summary>
    public interface IBuildData
    {
        string BuildMachine { get; }
        string BuildNumber { get; }
        string BuildQuality { get; }
        string BuildStatus { get; }
        int BuildStatusId { get; }
        string BuildType { get; }
        string BuildTypeFileUri { get; }
        string BuildUri { get; }
        string DropLocation { get; }
        DateTime FinishTime { get; }
        bool GoodBuild { get; }
        string LastChangedBy { get; }
        DateTime LastChangedOn { get; }
        string LogLocation { get; }
        string RequestedBy { get; }
        DateTime StartTime { get; }
        string TeamProject { get; }
    }
}
