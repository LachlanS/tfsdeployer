using System;

namespace TfsDeployer
{
    public interface IDeployer
    {
        void ExecuteDeploymentProcess(Readify.Useful.TeamFoundation.Common.Notification.BuildStatusChangeEvent statusChanged);
    }
}
