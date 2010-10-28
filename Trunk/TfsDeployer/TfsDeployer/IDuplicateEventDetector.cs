using Readify.Useful.TeamFoundation.Common.Notification;
using System;

namespace TfsDeployer
{
    public interface IDuplicateEventDetector
    {
        /// <summary>
        /// Checks for whether we've seen this event in very recent history (about five seconds), and decides
        /// whether we think the event is a duplicate.
        /// </summary>
        /// <returns>true if the event is unique; false otherwise.</returns>
        /// <remarks>
        /// This is a work-around for some weird behaviour we're seeing from TFS. We appear to receive multiple notifications
        /// from TFS for the same event on some servers, sometimes. It could be a multiple-event-subscription issue but I
        /// haven't been able to find it and it still occurred after nuking all the subscriptions to the BuildStatusChangeEvent
        /// on the server I was having trouble with.  -andrewh 22/10/2010
        /// </remarks>
        bool IsUnique(BuildStatusChangeEvent buildStatusChangeEvent);

        /// <summary>
        /// The timespan during which events that look the same will be identified as unique.
        /// </summary>
        TimeSpan TimeoutPeriod { get; }
    }
}
