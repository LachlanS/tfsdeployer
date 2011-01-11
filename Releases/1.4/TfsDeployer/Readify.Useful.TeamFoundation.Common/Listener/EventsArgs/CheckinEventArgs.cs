using Microsoft.TeamFoundation.VersionControl.Common;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public class CheckinEventArgs : NotificationEventArgs<CheckinEvent>
    {
        public CheckinEventArgs(CheckinEvent eventReceived, TfsIdentity identity)
            : base(eventReceived, identity)
        {
        }
    }
}
