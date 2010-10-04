using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    [TestClass]
    public class When_ReadMappings_processes_DeployerConfigurationWithBlankMappingNamespace : ConfigurationReaderContext
    {
        [TestMethod]
        public void Should_return_zero_mappings()
        {
            var mappings = ReadMappings("MyBuildDefA", SerializedDeploymentMappings.DeployerConfigurationWithBlankMappingNamespace);
            Assert.AreEqual(0, mappings.Count());
        }
    }
}