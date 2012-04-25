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
using AutoMapper;
using Microsoft.TeamFoundation.Build.Client;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class Deployer : IDeployer
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IBuildServer _buildServer;
        private readonly IMappingProcessor _mappingProcessor;
        private readonly Func<BuildDetail, IBuildDetail, IPostDeployAction> _postDeployActionFactory;

        static Deployer()
        {
            Mapper.CreateMap<IProcessTemplate, ProcessTemplate>();
            Mapper.CreateMap<IBuildDefinition, BuildDefinition>();
            Mapper.CreateMap<IBuildDetail, BuildDetail>();
            Mapper.AssertConfigurationIsValid();
        }
        
        public Deployer(IConfigurationReader reader, IBuildServer buildServer, IMappingProcessor mappingProcessor, Func<BuildDetail, IBuildDetail, IPostDeployAction> postDeployActionFactory)
        {
            _configurationReader = reader;
            _buildServer = buildServer;
            _mappingProcessor = mappingProcessor;
            _postDeployActionFactory = postDeployActionFactory;
        }

        public void ExecuteDeploymentProcess(BuildStatusChangeEvent statusChanged, int eventId)
        {
            try
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                             "Build Status Changed: Team Project {0}  Team Build Version: {1} From {2} : {3}",
                                             statusChanged.TeamProject,
                                             statusChanged.Id,
                                             statusChanged.StatusChange.OldValue,
                                             statusChanged.StatusChange.NewValue);

                var tfsBuildDetail = GetBuildDetail(statusChanged);
                var buildDetail = Mapper.Map<IBuildDetail, BuildDetail>(tfsBuildDetail);

                var postDeployAction = _postDeployActionFactory(buildDetail, tfsBuildDetail);
                
                var mappings = _configurationReader.ReadMappings(buildDetail);

                _mappingProcessor.ProcessMappings(mappings, statusChanged, buildDetail, postDeployAction, eventId);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }

        private IBuildDetail GetBuildDetail(BuildStatusChangeEvent statusChanged)
        {
            var buildSpec = _buildServer.CreateBuildDefinitionSpec(statusChanged.TeamProject);
            var detail = _buildServer.GetBuild(buildSpec, statusChanged.Id, null, QueryOptions.All);
            return detail;
        }

        public void Dispose()
        {
            // noop
        }
    }
}