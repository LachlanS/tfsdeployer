using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.VersionControl.Client;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Configuration
{
    static class TfsHelper
    {
        const string ConfigurationFileLocation = "{0}/TeamBuildTypes/{1}/Deployment/";
        public static string GetDeploymentItems(string teamProjectName, string buildType)
        {
            string workspaceDirectory = SourceCodeControlHelper.GetWorkspaceDirectory("TFSDeployerConfiguration2");
            TeamProject teamProject = GetTeamProject(teamProjectName);
            ItemSpec configurationFileItemSpec = new ItemSpec(string.Format(ConfigurationFileLocation,teamProject.ServerItem,buildType),RecursionType.Full);
            VersionSpec version = VersionSpec.Latest;
            GetRequest[] request = new GetRequest[] { new GetRequest(configurationFileItemSpec, version) };
            string directoryToPlaceFiles  = string.Format(ConfigurationFileLocation, teamProject.ServerItem, buildType);
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Getting file from {0} to {1}",workspaceDirectory,directoryToPlaceFiles);
            SourceCodeControlHelper.GetLatestFromSourceCodeControl(directoryToPlaceFiles, workspaceDirectory, request);
            return workspaceDirectory;
        }

        private static TeamProject GetTeamProject(string teamProjectName)
        {
            VersionControlServer versionControlServer = ServiceHelper.GetService<VersionControlServer>();
            return versionControlServer.GetTeamProject(teamProjectName);
        }

    }
}
