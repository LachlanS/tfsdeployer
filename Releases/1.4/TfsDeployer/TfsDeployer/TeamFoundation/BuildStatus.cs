using System;

namespace TfsDeployer.TeamFoundation
{
    [Flags]
    public enum BuildStatus
    {
        None = 0,
        InProgress = 0x1,
        Succeeded = 0x2,
        PartiallySucceeded = 0x4,
        Failed = 0x8,
        Stopped = 0x10,
        NotStarted = 0x20,
        All = 0x3f,
    }
}