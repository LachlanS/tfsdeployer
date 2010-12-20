using System.Diagnostics;
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
        public void Encrypter_should_not_add_another_signature_to_a_signed_document()
        {
            // Arrange
            var keyFile = Path.GetTempFileName();
            Encrypter.CreateKey(keyFile);

            var mappingsPath = Path.GetTempFileName();
            File.WriteAllText(mappingsPath, @"<root><element /></root>");

            Encrypter.Sign(mappingsPath, keyFile);
            var signedMappingsBackupPath = Path.GetTempFileName();
            File.Copy(mappingsPath, signedMappingsBackupPath, true);

            // Act
            Encrypter.Sign(mappingsPath, keyFile);
            var result = FilesEqual(mappingsPath, signedMappingsBackupPath);

            // Absterge
            File.Delete(keyFile);
            File.Delete(mappingsPath);
            File.Delete(signedMappingsBackupPath);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Encrypter_should_verify_unchanged_signed_document_with_persisted_key()
        {
            // Arrange
            var keyFile = Path.GetTempFileName();
            Encrypter.CreateKey(keyFile);
            var newKey = Encrypter.ReadKey(keyFile);

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
            var keyFile = Path.GetTempFileName();
            Encrypter.CreateKey(keyFile);
            var newKey = Encrypter.ReadKey(keyFile);

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

        [TestMethod]
        public void Encrypter_should_verify_and_not_close_input_stream()
        {
            // Arrange
            var keyFile = Path.GetTempFileName();
            Encrypter.CreateKey(keyFile);
            var newKey = Encrypter.ReadKey(keyFile);

            var doc = new XmlDocument();
            doc.LoadXml(@"<root><element /></root>");

            Encrypter.SignXml(doc, newKey);
            var docFile = Path.GetTempFileName();
            doc.Save(docFile);

            // Act
            try
            {
                using (var docStream = new FileStream(docFile, FileMode.Open))
                {
                    Encrypter.VerifyXml(docStream, keyFile);
                    docStream.Seek(0, SeekOrigin.Begin);
                    docStream.ReadByte(); // throws an exception if stream is closed
                }
            } 
            finally
            {
                // Absterge
                File.Delete(keyFile);
                File.Delete(docFile);
            }

            // Assert
            // no exception
        }

        [TestMethod]
        public void Encrypter_should_read_key_created_by_PowerShell_script()
        {
            // Arrange
            var createSigningKeyScriptPath = Path.Combine(Path.GetDirectoryName(typeof(Encrypter).Assembly.Location), "Create-SigningKey.ps1");
            var keyPath = Path.GetTempFileName();
            var arguments = string.Format("-command & '{0}' -KeyPath '{1}'", createSigningKeyScriptPath, keyPath);
            var process = Process.Start("powershell.exe", arguments);
            if (!process.WaitForExit(5000)) process.Kill();

            // Act
            var key = Encrypter.ReadKey(keyPath);

            // Absterge
            File.Delete(keyPath);

            // Assert
            Assert.IsNotNull(key);
        }

        [TestMethod]
        public void Encrypter_should_verify_document_signed_by_PowerShell_script()
        {
            // Arrange
            var signScriptPath = Path.Combine(Path.GetDirectoryName(typeof(Encrypter).Assembly.Location), "Sign-DeploymentMappings.ps1");
            var mappingsPath = Path.GetTempFileName();
            File.WriteAllText(mappingsPath, @"<root><element /></root>");
            var keyPath = Path.GetTempFileName();
            Encrypter.CreateKey(keyPath);
            var arguments = string.Format("-command & '{0}' -DeploymentMappingsPath '{1}' -KeyPath '{2}'", signScriptPath, mappingsPath, keyPath);
            var process = Process.Start("powershell.exe", arguments);
            if (!process.WaitForExit(5000)) process.Kill();

            // Act
            bool result;
            using (var stream = File.OpenRead(mappingsPath))
            {
                result = Encrypter.VerifyXml(stream, keyPath);
            }

            // Absterge
            File.Delete(mappingsPath);
            File.Delete(keyPath);

            // Assert
            Assert.IsTrue(result);
        }

        private static bool FilesEqual(string pathA, string pathB)
        {
            var infoA = new FileInfo(pathA);
            var infoB = new FileInfo(pathB);
            if (!infoA.Exists || !infoB.Exists) return false;
            if (infoA.Length != infoB.Length) return false;
            const int bufferSize = 0x1000;
            var bufferA = new byte[bufferSize];
            var bufferB = new byte[bufferA.Length];

            using (var streamA = infoA.OpenRead())
            using (var streamB = infoB.OpenRead())
            {
                int byteCount;
                do
                {
                    byteCount = streamA.Read(bufferA, 0, bufferA.Length);
                    streamB.Read(bufferB, 0, bufferB.Length);
                    for (var byteIndex = 0; byteIndex < byteCount; byteIndex++)
                    {
                        if (bufferA[byteIndex] != bufferB[byteIndex])
                        {
                            return false;
                        }
                    }
                } while (byteCount > 0);
            }

            return true;
        }
    }
}
