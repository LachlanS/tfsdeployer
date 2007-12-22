using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.Build.Proxy;

namespace TfsDeployer.Configuration
{
    public interface IConfigurationReader
    {
        DeploymentMappings Read(string teamProject, BuildData teamBuild);
        string WorkingDirectory
        {
            get;
        }
    }
}
