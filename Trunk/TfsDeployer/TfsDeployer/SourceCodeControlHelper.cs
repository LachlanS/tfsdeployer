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

        
        public static void GetLatestFromSourceCodeControl(string rootFolder,string workspaceDirectory,  GetRequest[] filesToRetrieve)
        {
            VersionControlServer versionControlServer = ServiceHelper.GetService<VersionControlServer>();
            string workspaceName = GetWorkspaceName();
            string workingDirectory = workspaceDirectory;
            
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting files from Source code control. RootFolder{0}, Workspace Directory:{1}, Working Directory: {2}",rootFolder,workspaceDirectory,workingDirectory);
            try
            {

                Workspace workspace = GetWorkspace(rootFolder, versionControlServer, workspaceName, workingDirectory);
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

        private static Workspace GetWorkspace(string rootFolder, VersionControlServer versionControlServer, string workspaceName, string workingDirectory)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting Workspace:{0} RootFolder:{1}", workspaceName,rootFolder);
            Workspace workspace = versionControlServer.CreateWorkspace(workspaceName, versionControlServer.AuthenticatedUser);
            
            workspace.Map(rootFolder, workingDirectory);
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
