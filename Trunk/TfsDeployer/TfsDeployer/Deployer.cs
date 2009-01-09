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
        private readonly IRunnerProvider _runnerProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly IAlert _alerter;
        private readonly IMappingEvaluator _mappingEvaluator;

        public Deployer()
            : this(new RunnerProvider(), new TfsConfigReader(), new EmailAlerter(), new MappingEvaluator())
        {
        }

        public Deployer(IRunnerProvider runnerProvider, IConfigurationReader reader, IAlert alert,
                        IMappingEvaluator mappingEvaluator)
        {
            _runnerProvider = runnerProvider;
            _configurationReader = reader;
            _alerter = alert;
            _mappingEvaluator = mappingEvaluator;
        }

        public void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged)
        {
            try
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                             "Build Status Changed: Team Project {0}  Team Build Version: {1} From {2} : {3}",
                                             statusChanged.TeamProject,
                                             statusChanged.Id,
                                             statusChanged.StatusChange.OldValue,
                                             statusChanged.StatusChange.NewValue);

                var info = new BuildInformation(GetBuildDetail(statusChanged));
                using (var workingDirectory = new WorkingDirectory())
                {
                    var mappings = _configurationReader.ReadMappings(statusChanged.TeamProject, info.Data, workingDirectory);

                    foreach (var mapping in mappings)
                    {
                        TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                                     "Processing Mapping: Computer:{0}, Script:{1}",
                                                     mapping.Computer,
                                                     mapping.Script);

                        if (_mappingEvaluator.DoesMappingApply(mapping, statusChanged))
                        {
                            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                                         "Matching mapping found, running script {0}",
                                                         mapping.Script);

                            IRunner runner = _runnerProvider.GetRunner(mapping);
                            runner.Execute(workingDirectory.DirectoryInfo.FullName, mapping, info);
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

        private static IBuildDetail GetBuildDetail(BuildStatusChangeEvent statusChanged)
        {
            var buildServer = ServiceHelper.GetService<IBuildServer>();
            var buildSpec = buildServer.CreateBuildDefinitionSpec(statusChanged.TeamProject);
            var detail = buildServer.GetBuild(buildSpec, statusChanged.Id, null, QueryOptions.All);
            return detail;
        }
    }
}