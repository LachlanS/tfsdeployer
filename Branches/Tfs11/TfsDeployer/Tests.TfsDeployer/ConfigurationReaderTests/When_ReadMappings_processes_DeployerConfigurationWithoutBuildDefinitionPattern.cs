using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    [TestClass]
    public class When_ReadMappings_processes_DeployerConfigurationWithoutBuildDefinitionPattern : ConfigurationReaderContext
    {
        [TestMethod]
        public void Should_return_one_mapping()
        {
            var mappings = ReadMappings("MyBuildDefA", SerializedDeploymentMappings.DeployerConfigurationWithoutBuildDefinitionPattern);
            Assert.AreEqual(1, mappings.Count());
        }
    }
}