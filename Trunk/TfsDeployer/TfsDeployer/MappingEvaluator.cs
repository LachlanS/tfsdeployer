using System;
using System.Net;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;

namespace TfsDeployer
{
    public class MappingEvaluator : IMappingEvaluator
    {
        public bool DoesMappingApply(Mapping mapping, BuildStatusChangeEvent triggerEvent, string buildStatus)
        {
            var statusChange = triggerEvent.StatusChange;

            var isDifferentStatusMatch = IsDifferentStatusMatch(statusChange);
            var isBuildStatusMatch = IsBuildStatusMatch(mapping, buildStatus);
            var isComputerMatch = IsComputerMatch(mapping.Computer);

            const string wildcardQuality = "*";
            var isOldValueMatch = IsQualityMatch(statusChange.OldValue, mapping.OriginalQuality, wildcardQuality);
            var isNewValueMatch = IsQualityMatch(statusChange.NewValue, mapping.NewQuality, wildcardQuality);
            var isUserPermitted = IsUserPermitted(triggerEvent, mapping);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Mapping evaluation details:\n" +
                              "    MachineName={0}, MappingComputer={1}\n" +
                              "    BuildOldStatus={2}, BuildNewStatus={3}\n" +
                              "    MappingOrigQuality={4}, MappingNewQuality={5}\n" +
                              "    UserIsPermitted={6}, EventCausedBy={7}\n" +
                              "    BuildStatus={8}, MappingStatus={9}",
                Environment.MachineName, mapping.Computer,
                statusChange.OldValue, statusChange.NewValue,
                mapping.OriginalQuality, mapping.NewQuality,
                isUserPermitted, triggerEvent.ChangedBy,
                buildStatus, mapping.Status);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Eval results:\n" +
                              "    isComputerMatch={0}\n" +
                              "    isOldValueMatch={1}\n" +
                              "    isNewValueMatch={2}\n" +
                              "    isUserPermitted={3}\n" +
                              "    isBuildStatusMatch={4}\n" +
                              "    isDifferentStatusMatch={5}\n",
                              isComputerMatch, isOldValueMatch, isNewValueMatch, isUserPermitted, isBuildStatusMatch, isDifferentStatusMatch);

            return isComputerMatch && isOldValueMatch && isNewValueMatch && isUserPermitted && isBuildStatusMatch && isDifferentStatusMatch;
        }

        private static bool IsDifferentStatusMatch(Change statusChange)
        {
            bool isStatusUnchanged = string.Equals(statusChange.NewValue, statusChange.OldValue, StringComparison.InvariantCultureIgnoreCase);

            bool isDifferentStatusMatch = (!isStatusUnchanged);
            return isDifferentStatusMatch;
        }

        private bool IsBuildStatusMatch(Mapping mapping, string buildStatus)
        {
            const string defaultMappingStatus = "Succeeded,PartiallySucceeded,Failed";
            string mappingStatus = string.IsNullOrEmpty(mapping.Status) ? defaultMappingStatus : mapping.Status;

            foreach (string status in mappingStatus.Split(','))
            {
                if (string.Equals(buildStatus, status.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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