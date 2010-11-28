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
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class VersionControlDeploymentFolderSource : IDeploymentFolderSource
    {
        private readonly VersionControlServer _versionControlServer;

        public VersionControlDeploymentFolderSource(VersionControlServer versionControlServer)
        {
            _versionControlServer = versionControlServer;
        }

        public void DownloadDeploymentFolder(BuildDetail buildDetail, string destination)
        {
            var serverPath = VersionControlPath.GetDeploymentFolderServerPath(buildDetail);
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting files from {0} to {1}", serverPath, destination);

            var serverItemSpec = new ItemSpec(serverPath, RecursionType.Full);
            var request = new[] { new GetRequest(serverItemSpec, VersionSpec.Latest) };
            GetLatestFromSourceCodeControl(serverPath, destination, request);
        }

        private void GetLatestFromSourceCodeControl(string serverPath, string localPath, GetRequest[] filesToRetrieve)
        {
            var workspaceName = GetWorkspaceName();

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting files from Source code control. RootFolder:{0}, Workspace Directory:{1}", serverPath, localPath);
            try
            {
                var workspace = GetWorkspace(serverPath, workspaceName, localPath);
                workspace.Get(filesToRetrieve, GetOptions.Overwrite);
            }
            finally
            {
                RemoveWorkspace(workspaceName);
            }
        }

        private void RemoveWorkspace(string workspaceName)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Removing Workspace: {0}", workspaceName);
            if (_versionControlServer.QueryWorkspaces(workspaceName, _versionControlServer.AuthorizedUser, Environment.MachineName).Length > 0)
            {
                _versionControlServer.DeleteWorkspace(workspaceName, _versionControlServer.AuthorizedUser);
            }
        }

        private Workspace GetWorkspace(string serverPath, string workspaceName, string localPath)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting Workspace: {0} RootFolder: {1}", workspaceName, serverPath);
            var workspace = _versionControlServer.CreateWorkspace(workspaceName, _versionControlServer.AuthorizedUser);
            workspace.Map(serverPath, localPath);
            return workspace;
        }

        private static string GetWorkspaceName()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
