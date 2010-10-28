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
using TfsDeployer.DeployAgent;

namespace TfsDeployer
{
    public class Deployer : IDeployer
    {
        private readonly IDeployAgentProvider _deployAgentProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly IDeploymentFolderSource _deploymentFolderSource;
        private readonly IAlert _alerter;
        private readonly IMappingEvaluator _mappingEvaluator;
        private readonly IBuildServer _buildServer;

        public Deployer(IDeploymentFileSource deploymentFileSource, IDeploymentFolderSource deploymentFolderSource, IBuildServer buildServer)
            : this(new DeployAgentProvider(), new ConfigurationReader(deploymentFileSource), deploymentFolderSource, new EmailAlerter(), new MappingEvaluator(), buildServer)
        {
        }

        public Deployer(IDeployAgentProvider deployAgentProvider, IConfigurationReader reader, IDeploymentFolderSource deploymentFolderSource, IAlert alert,
                        IMappingEvaluator mappingEvaluator, IBuildServer buildServer)
        {
            _deployAgentProvider = deployAgentProvider;
            _configurationReader = reader;
            _deploymentFolderSource = deploymentFolderSource;
            _alerter = alert;
            _mappingEvaluator = mappingEvaluator;
            _buildServer = buildServer;
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
                var mappings = _configurationReader.ReadMappings(info.Detail);
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

                        // default to "happy; did nothing" if there's no deployment agent.
                        DeployAgentResult deployResult = new DeployAgentResult { HasErrors = false, Output = string.Empty };

                        if (deployAgent != null)
                        {
                            using (var workingDirectory = new WorkingDirectory())
                            {
                                var deployAgentDataFactory = new DeployAgentDataFactory();
                                var deployData = deployAgentDataFactory.Create(workingDirectory.DirectoryInfo.FullName,
                                                                               mapping, info);

                                _deploymentFolderSource.DownloadDeploymentFolder(info.Detail, workingDirectory.DirectoryInfo.FullName);
                                deployResult = deployAgent.Deploy(deployData);
                            }
                        }

                        ApplyRetainBuild(mapping, deployResult, info.Detail);
                        _alerter.Alert(mapping, info.Data, deployResult);
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }

        private static void ApplyRetainBuild(Mapping mapping, DeployAgentResult deployAgentResult, IBuildDetail detail)
        {
            // bad state?
            if (!mapping.RetainBuildSpecified) return;
            if (deployAgentResult.HasErrors) return;

            // no change to setting?
            if (detail.KeepForever == mapping.RetainBuild) return;

            detail.KeepForever = mapping.RetainBuild;
            detail.Save();
        }

        private IBuildDetail GetBuildDetail(BuildStatusChangeEvent statusChanged)
        {
            var buildSpec = _buildServer.CreateBuildDefinitionSpec(statusChanged.TeamProject);
            var detail = _buildServer.GetBuild(buildSpec, statusChanged.Id, null, QueryOptions.All);
            return detail;
        }
    }
}
