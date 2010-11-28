using System.Collections.Generic;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    public class ConfigurationReaderContext
    {
        protected static IEnumerable<Mapping> ReadMappings(string buildDefinitionName, string configurationXml)
        {
            var buildDetail = new BuildDetail
                                  {
                                      BuildDefinition = {Name = buildDefinitionName}
                                  };

            var deploymentFileSource = new StubDeploymentFileSource(configurationXml);
            var configReader = new ConfigurationReader(deploymentFileSource);

            return configReader.ReadMappings(buildDetail);
        }
    }
}