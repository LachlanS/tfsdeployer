using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TfsDeployer.ConfigurationReaderTests;
using TfsDeployer;
using TfsDeployer.Configuration;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.Configuration
{
    [TestClass]
    public class ConfigurationReaderTests
    {
        [TestMethod]
        public void ConfigurationReader_should_read_mappings_from_a_signed_xml_document()
        {
            // Arrange
            var newKey = Encrypter.GenerateKey();
            var keyFile = Path.GetTempFileName();
            Encrypter.SaveKey(keyFile, newKey);

            var doc = new XmlDocument();
            doc.LoadXml(SerializedDeploymentMappings.CompleteDeployerConfiguration);

            Encrypter.SignXml(doc, newKey);
            string signedXml;
            using (var signedXmlStream = new MemoryStream())
            {
                doc.Save(signedXmlStream);
                signedXml = Encoding.UTF8.GetString(signedXmlStream.ToArray());
            }

            var reader = new ConfigurationReader(new StubDeploymentFileSource(signedXml), keyFile);

            var buildDetail = new BuildDetail {BuildDefinition = {Name = "MyBuildDefA"}};

            IEnumerable<Mapping> mappings;
            // Act
            try
            {
                mappings = reader.ReadMappings(buildDetail);
            }            
            finally
            {
                // Absterge
                File.Delete(keyFile);
            }

            // Assert
            Assert.IsTrue(mappings.Any());
            
        }
    }
}
