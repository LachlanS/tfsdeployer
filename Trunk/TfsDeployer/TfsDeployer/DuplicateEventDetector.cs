using System;
using System.Collections.Generic;
using System.Linq;
using Readify.Useful.TeamFoundation.Common.Notification;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    class DuplicateEventDetector : IDuplicateEventDetector
    {
        private class EventRecord
        {
            private readonly DateTime _whenSeen;
            private readonly BuildStatusChangeEvent _buildStatusChangeEvent;

            public EventRecord(DateTime whenSeen, BuildStatusChangeEvent buildStatusChangeEvent)
            {
                _whenSeen = whenSeen;
                _buildStatusChangeEvent = buildStatusChangeEvent;
            }

            public DateTime WhenSeen { get { return _whenSeen; } }
            public BuildStatusChangeEvent BuildStatusChangeEvent { get { return _buildStatusChangeEvent; } }
        }

        readonly List<EventRecord> _recentStatusChangeEvents = new List<EventRecord>();

        public bool IsUnique(BuildStatusChangeEvent buildStatusChangeEvent)
        {
            lock (_recentStatusChangeEvents)
            {
                var cutoffDate = DateTime.Now.Subtract(new TimeSpan(0, 0, 5));
                var toRemove = _recentStatusChangeEvents.Where(ev => ev.WhenSeen < cutoffDate).ToArray();
                foreach (var expiredEvent in toRemove) _recentStatusChangeEvents.Remove(expiredEvent);

                if (_recentStatusChangeEvents.Any(ev => AreTooSimilar(ev.BuildStatusChangeEvent, buildStatusChangeEvent)))
                {
                    TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Received duplicate event '{0}' from TFS.", buildStatusChangeEvent.Title);
                    return false;
                }

                _recentStatusChangeEvents.Add(new EventRecord(DateTime.Now, buildStatusChangeEvent));
            }

            return true;
        }

        private bool AreTooSimilar(BuildStatusChangeEvent a, BuildStatusChangeEvent b)
        {
            bool areTooSimilar = (a.ChangedBy == b.ChangedBy &&
                a.Id == b.Id &&
                a.StatusChange.FieldName == b.StatusChange.FieldName &&
                a.StatusChange.OldValue == b.StatusChange.OldValue &&
                a.StatusChange.NewValue == b.StatusChange.NewValue &&
                a.TeamProject == b.TeamProject &&
                a.Title == b.Title);

            return areTooSimilar;
        }
    }
}
