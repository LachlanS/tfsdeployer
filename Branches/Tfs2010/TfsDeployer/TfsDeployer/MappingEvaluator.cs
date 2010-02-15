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

            bool isStatusUnchanged = string.Equals(statusChange.NewValue, statusChange.OldValue, StringComparison.InvariantCultureIgnoreCase);
            if (isStatusUnchanged) return false;

            bool isBuildStatusMatch = IsBuildStatusMatch(mapping, buildStatus);
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
                              "    UserIsPermitted={6}, EventCausedBy={7}\n" +
                              "    BuildStatus={8}, MappingStatus={9}",
                Environment.MachineName, mapping.Computer, 
                statusChange.OldValue, statusChange.NewValue, 
                mapping.OriginalQuality, mapping.NewQuality, 
                isUserPermitted, triggerEvent.ChangedBy,
                buildStatus, mapping.Status);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Eval results:\n" +
                              "    isComputerMatch={0}, isOldValueMatch={1}, isNewValueMatch={2}, isUserPermitted={3}, isBuildStatusMatch={4}",
                              isComputerMatch, isOldValueMatch, isNewValueMatch, isUserPermitted, isBuildStatusMatch);

            return isComputerMatch && isOldValueMatch && isNewValueMatch && isUserPermitted && isBuildStatusMatch;
            
        }

        private bool IsBuildStatusMatch(Mapping mapping, string buildStatus)
        {
            const string DefaultMappingStatus = "Succeeded,PartiallySucceeded,Failed";
            string mappingStatus = string.IsNullOrEmpty(mapping.Status) ? DefaultMappingStatus : mapping.Status;

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