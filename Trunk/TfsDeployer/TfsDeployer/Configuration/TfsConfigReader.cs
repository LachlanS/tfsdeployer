using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Configuration
{
    /// <summary>
    /// This class reads configuration information
    /// fromt the TeamBuild section of TFS and returns.
    /// </summary>
    public class TfsConfigReader : IConfigurationReader
    {

        string _workingDirectory;

        #region IConfigurationReader Members
        const string ConfigurationFileName = "DeploymentMappings.xml";
        
        public DeploymentMappings Read(string teamProjectName, Microsoft.TeamFoundation.Build.Proxy.BuildData teamBuild)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration for Team Projet:{0} Team Build:{1}", teamProjectName, teamBuild.BuildType);
            _workingDirectory = TfsHelper.GetDeploymentItems(teamProjectName, teamBuild.BuildType);
            return ConfigurationReaderHelper.Read(Path.Combine(_workingDirectory, ConfigurationFileName)); ;
        }

      
        public string WorkingDirectory
        {
            get 
            {
                return _workingDirectory;
            }
        }

        #endregion
    }
}
