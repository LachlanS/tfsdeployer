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
using System.Text;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common;
using System.IO;

namespace TfsDeployer
{
    /// <summary>
    /// Helper class to get files out of source code control
    /// </summary>
    public static class SourceCodeControlHelper
    {

        
        public static void GetLatestFromSourceCodeControl(string serverPath, string localPath, GetRequest[] filesToRetrieve)
        {
            var versionControlServer = ServiceHelper.GetService<VersionControlServer>();
            string workspaceName = GetWorkspaceName();

            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting files from Source code control. RootFolder:{0}, Workspace Directory:{1}", serverPath, localPath);
            try
            {

                Workspace workspace = GetWorkspace(serverPath, versionControlServer, workspaceName, localPath);
                workspace.Get(filesToRetrieve, GetOptions.Overwrite);
            }
            finally
            {
                RemoveWorkspace(workspaceName, versionControlServer);
            }
        }

        
        public static void RemoveWorkspaceDirectory(string workspaceDirectory)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Removing Workspace Directory {0}", workspaceDirectory);
            if (Directory.Exists(workspaceDirectory))
            {
                string[] files = System.IO.Directory.GetFiles(workspaceDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                Directory.Delete(workspaceDirectory, true);
            }
        }

        private static void RemoveWorkspace(string workspaceName, VersionControlServer server)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Removing Workspace{0}", workspaceName);
            if (server.QueryWorkspaces(workspaceName, server.AuthenticatedUser, Environment.MachineName).Length > 0)
            {
                server.DeleteWorkspace(workspaceName, server.AuthenticatedUser);
            }
        }

        private static Workspace GetWorkspace(string serverPath, VersionControlServer versionControlServer, string workspaceName, string localPath)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting Workspace:{0} RootFolder:{1}", workspaceName, serverPath);
            Workspace workspace = versionControlServer.CreateWorkspace(workspaceName, versionControlServer.AuthenticatedUser);
            
            workspace.Map(serverPath, localPath);
            return workspace;
        }

        public static string GetWorkspaceName()
        {
            string workspaceName = string.Format("{0}", Guid.NewGuid());
            return workspaceName;
        }

        public static string GetWorkspaceDirectory(string workspaceName)
        {
            string workingDirectory = Path.Combine(Path.GetTempPath(), workspaceName);
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            return workingDirectory;
        }
          
    }
}
