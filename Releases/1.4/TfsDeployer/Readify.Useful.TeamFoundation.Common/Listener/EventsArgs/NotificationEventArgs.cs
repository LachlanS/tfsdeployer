using System;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace Readify.Useful.TeamFoundation.Common.Listener
{
    public class NotificationEventArgs<T>:EventArgs
    {
        public T EventRaised { get; set; }
        public TfsIdentity TfsIdentity { get; set; }

        public NotificationEventArgs(T eventRaised, TfsIdentity identity)
        {
            EventRaised = eventRaised;
            TfsIdentity = identity;
        }
	
    }
}
