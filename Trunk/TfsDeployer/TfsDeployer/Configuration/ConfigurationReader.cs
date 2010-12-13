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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        private static readonly XmlSerializer DeploymentMappingsSerializer = new XmlSerializer(typeof(DeploymentMappings));

        private readonly IDeploymentFileSource _deploymentFileSource;
        private readonly string _signingKeyFile;

        public ConfigurationReader(IDeploymentFileSource deploymentFileSource, string signingKeyFile)
        {
            _deploymentFileSource = deploymentFileSource;
            _signingKeyFile = signingKeyFile;
        }

        public IEnumerable<Mapping> ReadMappings(BuildDetail buildDetail)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration for Team Project: {0} Team Build: {1}", buildDetail.TeamProject, buildDetail.BuildDefinition.Name);

            DeploymentMappings configuration = null;
            using (var localFile = new TemporaryFile())
            {
                if (_deploymentFileSource.DownloadDeploymentFile(buildDetail, localFile.FileInfo.FullName))
                {
                    configuration = Read(localFile.FileInfo.FullName);
                }
            }

            if (configuration == null)
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "No configuration found for this team project.");
                return Enumerable.Empty<Mapping>();
            }

            if (configuration.Mappings == null || configuration.Mappings.Length == 0)
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Configuration did not contain any Mappings.");
                return Enumerable.Empty<Mapping>();
            }

            return configuration.Mappings
                .Where(m => string.IsNullOrEmpty(m.BuildDefinitionPattern) 
                    || Regex.IsMatch(buildDetail.BuildDefinition.Name, m.BuildDefinitionPattern))
                .ToArray(); 
        }

        private DeploymentMappings Read(Stream deployerConfiguration)
        {
            if (!string.IsNullOrEmpty(_signingKeyFile))
            {
                if (!Encrypter.VerifyXml(deployerConfiguration, _signingKeyFile))
                {
                    TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Verification Failed for the deployment mapping using key file {0}", Properties.Settings.Default.KeyFile);
                    return null;
                }
                deployerConfiguration.Seek(0, SeekOrigin.Begin);
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Verification Succeeded for the deployment mapping");
            }

            using (var reader = new StreamReader(deployerConfiguration))
            {
                return (DeploymentMappings)DeploymentMappingsSerializer.Deserialize(reader);
            }
        }

        private DeploymentMappings Read(string configFilename)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration File:{0}", configFilename);

            if (File.Exists(configFilename))
            {
                using (var stream = File.OpenRead(configFilename))
                {
                    return Read(stream);
                }
            }

            TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Reading Configuration File:{0} failed.", configFilename);
            return null;
        }
    }
}
