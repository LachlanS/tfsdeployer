using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public interface IDeployer
    {
        void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged, int eventId);
    }
}
