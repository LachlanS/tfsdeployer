using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    public class ConfigurationReaderContext
    {
        protected static IEnumerable<Mapping> ReadMappings(string buildDefinitionName, string configurationXml)
        {
            IBuildDetail buildDetail = new StubBuildDetail();
            buildDetail.BuildDefinition.Name = buildDefinitionName;

            var deploymentFileSource = new StubDeploymentFileSource(configurationXml);
            var configReader = new ConfigurationReader(deploymentFileSource);

            return configReader.ReadMappings(buildDetail);
        }
    }
}