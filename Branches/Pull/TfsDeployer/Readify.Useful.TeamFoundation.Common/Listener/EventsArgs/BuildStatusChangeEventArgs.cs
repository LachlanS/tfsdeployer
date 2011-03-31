using Readify.Useful.TeamFoundation.Common.Notification;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public class BuildStatusChangeEventArgs : NotificationEventArgs<BuildStatusChangeEvent>
    {
        public BuildStatusChangeEventArgs(BuildStatusChangeEvent eventReceived, TfsIdentity identity)
            : base(eventReceived, identity)
        {
        }
    }
}
