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
using Microsoft.TeamFoundation.Build.Client;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Notifier;

namespace TfsDeployer
{
    internal class Deployer
    {
        private readonly IDeployAgentProvider _deployAgentProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly IAlert _alerter;
        private readonly IMappingEvaluator _mappingEvaluator;

        public Deployer()
            : this(new DeployAgentProvider(), new TfsConfigReader(), new EmailAlerter(), new MappingEvaluator())
        {
        }

        public Deployer(IDeployAgentProvider deployAgentProvider, IConfigurationReader reader, IAlert alert,
                        IMappingEvaluator mappingEvaluator)
        {
            _deployAgentProvider = deployAgentProvider;
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

                        if (_mappingEvaluator.DoesMappingApply(mapping, statusChanged, info.Detail.Status.ToString()))
                        {
                            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                                         "Matching mapping found, running script {0}",
                                                         mapping.Script);

                            var deployAgent = _deployAgentProvider.GetDeployAgent(mapping);

                            var deployData = CreateDeployAgentData(workingDirectory.DirectoryInfo.FullName, mapping, info);
                            var deployResult = deployAgent.Deploy(deployData);

                            ApplyRetainBuild(mapping, deployResult, info.Detail);
                            _alerter.Alert(mapping, info.Data, deployResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }

        private static DeployAgentData CreateDeployAgentData(string directory, Mapping mapping, BuildInformation buildInfo)
        {
            var data = new DeployAgentData
                           {
                               NewQuality = mapping.NewQuality,
                               OriginalQuality = mapping.OriginalQuality,
                               DeployServer = mapping.Computer,
                               DeployScriptFile = mapping.Script,
                               DeployScriptRoot = directory,
                               DeployScriptParameters = CreateParameters(mapping.ScriptParameters),
                               Tfs2005BuildData = buildInfo.Data,
                               Tfs2008BuildDetail = buildInfo.Detail
                           };
            return data;
        }

        private static ICollection<DeployScriptParameter> CreateParameters(IEnumerable<ScriptParameter> parameters)
        {
            var collection = new List<DeployScriptParameter>();
            foreach (var p in parameters)
            {
                collection.Add(new DeployScriptParameter { Name = p.name, Value = p.value });
            }
            return collection;
        }

        private static void ApplyRetainBuild(Mapping mapping, DeployAgentResult deployAgentResult, IBuildDetail detail)
        {
            if (!mapping.RetainBuildSpecified) return;
            if (deployAgentResult.HasErrors) return;
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