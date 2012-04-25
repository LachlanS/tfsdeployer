using Readify.Useful.TeamFoundation.Common.Notification;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public class BuildCompletionEventArgs:NotificationEventArgs<BuildCompletionEvent>
    {
        public BuildCompletionEventArgs(BuildCompletionEvent eventReceived, TfsIdentity identity)
            : base(eventReceived, identity)
        {
        }
    }
}
