using System;
using System.Net;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace TfsDeployer
{
    public class MappingEvaluator : IMappingEvaluator
    {
        public bool DoesMappingApply(Mapping mapping, BuildStatusChangeEvent triggerEvent)
        {
            var statusChange = triggerEvent.StatusChange;

            bool isComputerMatch = IsComputerMatch(mapping.Computer);

            string wildcardQuality = Properties.Settings.Default.BuildQualityWildcard;
            bool isOldValueMatch = IsQualityMatch(statusChange.OldValue, mapping.OriginalQuality, wildcardQuality);
            bool isNewValueMatch = IsQualityMatch(statusChange.NewValue, mapping.NewQuality, wildcardQuality);
            bool isUserPermitted = IsUserPermitted(triggerEvent, mapping);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Mapping evaluation details:\n" +
                              "    MachineName={0}, MappingComputer={1}\n" +
                              "    BuildOldStatus={2}, BuildNewStatus={3}\n" +
                              "    MappingOrigQuality={4}, MappingNewQuality={5}\n" +
                              "    UserIsPermitted={6}, EventCausedBy={7}",
                Environment.MachineName, mapping.Computer, statusChange.OldValue, statusChange.NewValue, mapping.OriginalQuality, mapping.NewQuality, isUserPermitted, triggerEvent.ChangedBy);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Eval results:\n" +
                              "    isComputerMatch={0}, isOldValueMatch={1}, isNewValueMatch={2}, isUserPermitted={3}",
                              isComputerMatch, isOldValueMatch, isNewValueMatch, isUserPermitted);

            return isComputerMatch && isOldValueMatch && isNewValueMatch && isUserPermitted;
            
        }

        private static bool IsComputerMatch(string mappingComputerName)
        {
            var hostNameOnly = Dns.GetHostName().Split('.')[0];
            return string.Equals(hostNameOnly, mappingComputerName, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsQualityMatch(string eventQuality, string mappingQuality, string wildcardQuality)
        {
            eventQuality = eventQuality ?? string.Empty;
            mappingQuality = mappingQuality ?? string.Empty;
            if (string.Compare(mappingQuality, wildcardQuality, true) == 0) return true;
            return string.Compare(mappingQuality, eventQuality, true) == 0;
        }

        private static bool IsUserPermitted(BuildStatusChangeEvent changeEvent, Mapping mapping)
        {
            if (mapping.PermittedUsers == null) return true;

            var permittedUsers = mapping.PermittedUsers.Split(';');
            foreach (var userName in permittedUsers)
            {
                if (string.Equals(changeEvent.ChangedBy, userName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

    }
}