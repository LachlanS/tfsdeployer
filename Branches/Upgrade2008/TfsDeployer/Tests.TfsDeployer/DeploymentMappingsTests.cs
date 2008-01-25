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
        public void ShouldInterpretAbsentOriginalValueAttributeAsNull()
        {
            using (var textReader = new StringReader(SerializedDeploymentMappings.AbsentOriginalQuality))
            {
                var serializer = new XmlSerializer(typeof(DeploymentMappings));
                var mappings = (DeploymentMappings)serializer.Deserialize(textReader);
                Assert.IsNull(mappings.Mappings[0].OriginalQuality, "OriginalQuality");
            }
        }

    }
}
