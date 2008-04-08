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
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.Runner;
using TfsDeployer.Notifier;
using TfsDeployer.Alert;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsDeployer
{
    /// <summary>
    /// Implemenation class for TFS Deployer
    /// </summary>
    internal class Deployer
    {
        private IRunner _runner;
        public IRunner Runner
        {
            get { return _runner; }
        }

        private IConfigurationReader _reader;
        public IConfigurationReader ConfigurationReader
        {
            get { return _reader; }
        }
        
        private IAlert _alerter;
        public IAlert Alerter
        {
            get { return _alerter; }
        }
	

        public Deployer()
        {
            _reader = new TfsConfigReader();
            _runner = null;
            _alerter = new EmailAlerter();
        }

        /// <summary>
        /// Public Contructor of Deployer
        /// </summary>
        /// <param name="runnerToUser"></param>
        /// <param name="reader"></param>
        public Deployer(IRunner runnerToUser,IConfigurationReader reader)
        {
            _runner = runnerToUser;
            _reader = reader;
        }

        /// <summary>
        /// The main execution method for TFS Deployer it is this
        /// method that does all the work
        /// </summary>
        /// <param name="statusChanged"></param>
        public void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged)
        {
            try
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Build Status Changed: Team Project {0}  Team Build Version: {1} From {2} : {3}",
                    statusChanged.TeamProject, statusChanged.Id, statusChanged.StatusChange.OldValue, statusChanged.StatusChange.NewValue);
                var info = new BuildInformation(GetBuildDetail(statusChanged));
                DeploymentMappings mappings = ConfigurationReader.Read(statusChanged.TeamProject, info.Data);
                if (mappings != null)
                {
                    foreach (Mapping mapping in mappings.Mappings)
                    {
                        TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Processing Mapping: Computer:{0}, Script:{1}",mapping.Computer,mapping.Script);
                        if (IsInterestedStatusChange(statusChanged, mapping, statusChanged.StatusChange))
                        {
                            IRunner runner = DetermineRunner(mapping);
                            runner.Execute(ConfigurationReader.WorkingDirectory, mapping, info);
                            ApplyRetainBuild(mapping, runner, info.Detail);
                            Alerter.Alert(mapping, info.Data, runner);
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
            bool isComputerMatch = string.Compare(Environment.MachineName, mapping.Computer, true) == 0;

            string wildcardQuality = Properties.Settings.Default.BuildQualityWildcard;
            bool isOldValueMatch = IsQualityMatch(statusChange.OldValue, mapping.OriginalQuality, wildcardQuality);
            bool isNewValueMatch = IsQualityMatch(statusChange.NewValue, mapping.NewQuality, wildcardQuality);
            bool isUserPermitted = IsUserPermitted(changeEvent, mapping);

            return isComputerMatch && isOldValueMatch && isNewValueMatch && isUserPermitted;
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

        private IRunner DetermineRunner(Mapping map)
        {
            IRunner runner;
            if (_runner == null)
            {
                switch (map.RunnerType)
                {
                    case RunnerType.PowerShell:
                        runner = new PowerShellRunner();
                        break;
                    case RunnerType.BatchFile:
                        runner = new BatchFileRunner();
                        break;
                    default:
                        runner = new PowerShellRunner();
                        break;
                }
            }
            else
            {
                runner = _runner;
            }
            return runner;
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
