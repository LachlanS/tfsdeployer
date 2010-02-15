using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer
{

    [TestClass]
    public class DeploymentMappingsTests
    {

        [TestMethod]
        public void ShouldInterpretAbsentOriginalQualityAttributeAsNull()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentOriginalQuality))
            {
                var serializer = new XmlSerializer(typeof(DeployerConfiguration));
                var mappings = (DeployerConfiguration)serializer.Deserialize(textReader);
                Assert.IsNull(mappings.Mappings[0].OriginalQuality, "OriginalQuality");
            }
        }

        [TestMethod]
        public void ShouldInterpretAbsentNewQualityAttributeAsNull()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentNewQuality))
            {
                var serializer = new XmlSerializer(typeof(DeployerConfiguration));
                var mappings = (DeployerConfiguration)serializer.Deserialize(textReader);
                Assert.IsNull(mappings.Mappings[0].NewQuality, "NewQuality");
            }
        }

        [TestMethod]
        public void ShouldInterpretAbsentRetainBuildAttributeAsNotSpecified()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentRetainBuild))
            {
                var serializer = new XmlSerializer(typeof(DeployerConfiguration));
                var mappings = (DeployerConfiguration)serializer.Deserialize(textReader);
                Assert.IsFalse(mappings.Mappings[0].RetainBuildSpecified, "RetainBuild");
            }
        }

    }
}
