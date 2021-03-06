using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                var serializer = new XmlSerializer(typeof(DeploymentMappings));
                var mappings = (DeploymentMappings)serializer.Deserialize(textReader);
                Assert.IsNull(mappings.Mappings[0].OriginalQuality, "OriginalQuality");
            }
        }

        [TestMethod]
        public void ShouldInterpretAbsentNewQualityAttributeAsNull()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentNewQuality))
            {
                var serializer = new XmlSerializer(typeof(DeploymentMappings));
                var mappings = (DeploymentMappings)serializer.Deserialize(textReader);
                Assert.IsNull(mappings.Mappings[0].NewQuality, "NewQuality");
            }
        }

        [TestMethod]
        public void ShouldInterpretAbsentRetainBuildAttributeAsNotSpecified()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentRetainBuild))
            {
                var serializer = new XmlSerializer(typeof(DeploymentMappings));
                var mappings = (DeploymentMappings)serializer.Deserialize(textReader);
                Assert.IsFalse(mappings.Mappings[0].RetainBuildSpecified, "RetainBuild");
            }
        }

    }
}
