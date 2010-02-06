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

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Configuration
{
    public class TfsHelper
    {
        private readonly IBuildServer _buildServer;
        private readonly SourceCodeControlHelper _sourceCodeControlHelper;

        public TfsHelper(IBuildServer buildServer, SourceCodeControlHelper sourceCodeControlHelper)
        {
            _buildServer = buildServer;
            _sourceCodeControlHelper = sourceCodeControlHelper;
        }

        public void GetDeploymentItems(string teamProjectName, string buildType, IWorkingDirectory workingDirectory)
        {
            var localPath = workingDirectory.DirectoryInfo.FullName;
            var serverPath = GetConfigurationFileLocation(teamProjectName, buildType);
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting files from {0} to {1}", serverPath, localPath);

            var serverItemSpec = new ItemSpec(serverPath, RecursionType.Full);
            var request = new[] { new GetRequest(serverItemSpec, VersionSpec.Latest) };
            _sourceCodeControlHelper.GetLatestFromSourceCodeControl(serverPath, localPath, request);
        }

        private string GetConfigurationFileLocation(string teamProjectName, string buildType)
        {
            var buildDefinition = _buildServer.GetBuildDefinition(teamProjectName, buildType);
            return VersionControlPath.Combine(buildDefinition.ConfigurationFolderPath, "Deployment/");
        }

        public void GetSharedResources(IWorkingDirectory workingDirectory)
        {
            var localPath = workingDirectory.DirectoryInfo.FullName;
            var serverPath = Properties.Settings.Default.SharedResourceServerPath;
            if (string.IsNullOrEmpty(serverPath)) return;

            var serverItemSpec = new ItemSpec(serverPath, RecursionType.Full);
            var request = new[] { new GetRequest(serverItemSpec, VersionSpec.Latest) };
            _sourceCodeControlHelper.GetLatestFromSourceCodeControl(serverPath, localPath, request);
        }

    }
}
