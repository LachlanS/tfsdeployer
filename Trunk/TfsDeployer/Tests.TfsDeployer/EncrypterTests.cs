using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class EncrypterTests
    {
        [TestMethod]
        public void Encrypter_should_verify_unchanged_signed_document_with_persisted_key()
        {
            // Arrange
            var newKey = Encrypter.GenerateKey();
            var keyFile = Path.GetTempFileName();
            Encrypter.SaveKey(keyFile, newKey);

            var doc = new XmlDocument();
            doc.LoadXml(@"<root><element /></root>");

            Encrypter.SignXml(doc, newKey);
            var docFile = Path.GetTempFileName();
            doc.Save(docFile);

            // Act
            bool result;
            using (var docStream = new FileStream(docFile, FileMode.Open))
            {
                result = Encrypter.VerifyXml(docStream, keyFile);
            }

            // Absterge
            File.Delete(keyFile);
            File.Delete(docFile);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Encrypter_should_fail_verification_of_a_modified_signed_document()
        {
            // Arrange
            var newKey = Encrypter.GenerateKey();
            var keyFile = Path.GetTempFileName();
            Encrypter.SaveKey(keyFile, newKey);

            var doc = new XmlDocument();
            doc.LoadXml(@"<root><element /></root>");

            Encrypter.SignXml(doc, newKey);
            doc.DocumentElement.AppendChild(doc.CreateElement("Foo")); // change document after signing
            var docFile = Path.GetTempFileName();
            doc.Save(docFile);

            // Act
            bool result;
            using (var docStream = new FileStream(docFile, FileMode.Open))
            {
                result = Encrypter.VerifyXml(docStream, keyFile);
            }

            // Absterge
            File.Delete(keyFile);
            File.Delete(docFile);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
