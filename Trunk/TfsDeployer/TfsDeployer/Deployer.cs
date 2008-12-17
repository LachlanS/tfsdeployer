// Copyright (c) 2007 Readify Pty. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Build.Client;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.Notifier;
using TfsDeployer.Runner;

namespace TfsDeployer
{
    internal class Deployer
    {
        private static readonly object _lock = new object();

        private readonly IRunnerProvider _runnerProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly IAlert _alerter;

        public Deployer() : this (new RunnerProvider(), new TfsConfigReader(), new EmailAlerter())
        {
        }

        public Deployer(IRunnerProvider runnerProvider, IConfigurationReader reader, IAlert alert)
        {
            _runnerProvider = runnerProvider;
            _configurationReader = reader;
            _alerter = alert;
        }

        public void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged)
        {
            // this prevents deployment folder corruption until the code uses a new folder per event
            // but doesn't fix the "build types without deployment configured can be deployed" bug.
            lock (_lock)
            {
                ExecuteDeploymentProcessWorker(statusChanged);
            }
        }

        private void ExecuteDeploymentProcessWorker(BuildStatusChangeEvent statusChanged)
        {
            try
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Build Status Changed: Team Project {0}  Team Build Version: {1} From {2} : {3}",
                    statusChanged.TeamProject, statusChanged.Id, statusChanged.StatusChange.OldValue, statusChanged.StatusChange.NewValue);
                var info = new BuildInformation(GetBuildDetail(statusChanged));
                DeploymentMappings mappings = _configurationReader.Read(statusChanged.TeamProject, info.Data);
                if (mappings != null)
                {
                    foreach (Mapping mapping in mappings.Mappings)
                    {
                        TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Processing Mapping: Computer:{0}, Script:{1}", mapping.Computer, mapping.Script);
                        if (IsInterestedStatusChange(statusChanged, mapping, statusChanged.StatusChange))
                        {
                            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Matching mapping found, running script {0}", mapping.Script);
                            IRunner runner = _runnerProvider.GetRunner(mapping);
                            runner.Execute(_configurationReader.WorkingDirectory, mapping, info);
                            ApplyRetainBuild(mapping, runner, info.Detail);
                            _alerter.Alert(mapping, info.Data, runner);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }

        }

        private static void ApplyRetainBuild(Mapping mapping, IRunner runner, IBuildDetail detail)
        {
            if (!mapping.RetainBuildSpecified) return;
            if (runner.ErrorOccurred) return;
            if (detail.KeepForever == mapping.RetainBuild) return;

            detail.KeepForever = mapping.RetainBuild;
            detail.Save();
        }

        public bool IsInterestedStatusChange(BuildStatusChangeEvent changeEvent, Mapping mapping, Change statusChange)
        {
            bool isComputerMatch = IsComputerMatch(mapping.Computer);
            
            string wildcardQuality = Properties.Settings.Default.BuildQualityWildcard;
            bool isOldValueMatch = IsQualityMatch(statusChange.OldValue, mapping.OriginalQuality, wildcardQuality);
            bool isNewValueMatch = IsQualityMatch(statusChange.NewValue, mapping.NewQuality, wildcardQuality);
            bool isUserPermitted = IsUserPermitted(changeEvent, mapping);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Mapping evaluation details:\n" +
                              "    MachineName={0}, MappingComputer={1}\n"+
                              "    BuildOldStatus={2}, BuildNewStatus={3}\n" +
                              "    MappingOrigQuality={4}, MappingNewQuality={5}\n" +
                              "    UserIsPermitted={6}, EventCausedBy={7}",
                Environment.MachineName, mapping.Computer, statusChange.OldValue, statusChange.NewValue, mapping.OriginalQuality, mapping.NewQuality, isUserPermitted, changeEvent.ChangedBy);

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                              "Eval results:\n" +
                              "    isComputerMatch={0}, isOldValueMatch={1}, isNewValueMatch={2}, isUserPermitted={3}",
                              isComputerMatch, isOldValueMatch, isNewValueMatch, isUserPermitted);

            return isComputerMatch && isOldValueMatch && isNewValueMatch && isUserPermitted;
        }

        private bool IsComputerMatch(string mappingComputerName)
        {
            var hostNameOnly = Dns.GetHostName().Split('.')[0];
            return string.Equals(hostNameOnly, mappingComputerName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsQualityMatch(string eventQuality, string mappingQuality, string wildcardQuality)
        {
            eventQuality = eventQuality ?? string.Empty;
            mappingQuality = mappingQuality ?? string.Empty;
            if (string.Compare(mappingQuality, wildcardQuality, true) == 0) return true;
            return string.Compare(mappingQuality, eventQuality, true) == 0;
        }

        private bool IsUserPermitted(BuildStatusChangeEvent changeEvent, Mapping mapping)
        {
            if (mapping.PermittedUsers == null) return true;

            bool isUserPermitted;
            string[] permittedUsers = mapping.PermittedUsers.Split(';');
            List<string> permittedUsersList = new List<string>(permittedUsers);
            isUserPermitted = permittedUsersList.Exists(
                delegate(string value) { return string.Compare(changeEvent.ChangedBy, value, true) == 0; }
                );
            return isUserPermitted;
        }

        private static IBuildDetail GetBuildDetail(BuildStatusChangeEvent statusChanged)
        {
            var buildServer = ServiceHelper.GetService<IBuildServer>();
            var buildSpec = buildServer.CreateBuildDefinitionSpec(statusChanged.TeamProject);
            var detail = buildServer.GetBuild(buildSpec, statusChanged.Id, null, QueryOptions.All);
            return detail;
        }

    }
}
