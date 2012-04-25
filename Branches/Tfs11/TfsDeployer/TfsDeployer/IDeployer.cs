using System;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public interface IDeployer : IDisposable
    {
        void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged, int eventId);
    }
}
