// Copyright (c) 2007 Readify Pty. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Security.Cryptography.Xml;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public static class Encrypter
    {

        private static readonly XmlSerializer RsaParametersSerializer = new XmlSerializer(typeof(RSAParameters));

        public static AsymmetricAlgorithm ReadKey(string keyPath)
        {
            if (string.IsNullOrEmpty(keyPath)) throw new ArgumentNullException("keyPath");
            if (!File.Exists(keyPath)) throw new ArgumentException("File not found.", "keyPath");

            using (var reader = File.OpenRead(keyPath))
            {
                var parameters = (RSAParameters)RsaParametersSerializer.Deserialize(reader);

                var key = new RSACryptoServiceProvider();
                key.ImportParameters(parameters);
                return key;
            }
        }

        public static Boolean VerifyXml(Stream xml, string rsaKeyFilename)
        {
            if (!File.Exists(rsaKeyFilename))
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Cannot find key file {0} in order to verify the deployment mappings file.", rsaKeyFilename);
                return false;
            }
            var doc = new XmlDocument();
            doc.Load(xml);
            var key = ReadKey(rsaKeyFilename);
            return VerifyXml(doc, key);
        }

        // Verify the signature of an XML file against an asymmetric 
        // algorithm and return the result.
        private static Boolean VerifyXml(XmlDocument document, AsymmetricAlgorithm key)
        {
            // Check arguments.
            if (document == null)
                throw new ArgumentNullException("document");
            if (key == null)
                throw new ArgumentNullException("key");

            // Create a new SignedXml object and pass it
            // the XML document class.
            var signedXml = new SignedXml(document);

            // Find the "Signature" node 
            var signatureElements = document.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl).Cast<XmlElement>().ToArray();

            // Throw an exception if no signature was found.
            if (signatureElements.Length == 0)
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer,"Verification failed: No Signature was found in the document.");
                return false;
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (signatureElements.Length > 1)
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Verification failed: More that one signature was found for the document.");
                return false;
            }

            // Load the first <signature> node.  
            signedXml.LoadXml(signatureElements.First());

            // Check the signature and return the result.
            return signedXml.CheckSignature(key);
        }

        // Sign an XML file. 
        // This document cannot be verified unless the verifying 
        // code has the key with which it was signed.
        public static void SignXml(XmlDocument document, AsymmetricAlgorithm key)
        {
            if (document == null) throw new ArgumentException("document");
            if (key == null) throw new ArgumentException("key");

            var signatureElements = document.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl).Cast<XmlElement>().ToArray();
            foreach (var signature in signatureElements)
            {
                signature.ParentNode.RemoveChild(signature);
            }

            var signedXml = new SignedXml(document) {SigningKey = key};

            // Create a reference to be signed.
            var reference = new Reference("");

            // Add an enveloped transformation to the reference.
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            var xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            document.DocumentElement.AppendChild(document.ImportNode(xmlDigitalSignature, true));
        }

        public static void Sign(string deploymentMappingsPath, string keyPath)
        {
            var key = ReadKey(keyPath);
            var doc = new XmlDocument();
            doc.Load(deploymentMappingsPath);
            SignXml(doc, key);
            doc.Save(deploymentMappingsPath);
        }

        public static void CreateKey(string keyPath)
        {
            var key = new RSACryptoServiceProvider();
            var parameters = key.ExportParameters(true);
            using (var writer = File.CreateText(keyPath))
            {
                RsaParametersSerializer.Serialize(writer, parameters);
            }
        }

    }
}
