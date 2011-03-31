using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    [TestClass]
    public class When_ReadMappings_processes_CompleteDeployerConfiguration : ConfigurationReaderContext
    {
        [TestMethod]
        public void Should_return_two_mappings_with_matching_BuildDefinitionPattern()
        {
            var mappings = ReadMappings("MyBuildDefA", SerializedDeploymentMappings.CompleteDeployerConfiguration);
            Assert.AreEqual(2, mappings.Count());
        }
    }
}
