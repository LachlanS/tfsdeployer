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
using Microsoft.TeamFoundation.Build.Client;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IDeploymentFileSource _deploymentFileSource;

        public ConfigurationReader(IDeploymentFileSource deploymentFileSource)
        {
            _deploymentFileSource = deploymentFileSource;
        }

        public IEnumerable<Mapping> ReadMappings(IBuildDetail buildDetail)
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

        private static DeploymentMappings Read(Stream deployerConfiguration)
        {
            var tempFileName = Path.GetTempFileName();
            using (var tempFile = File.OpenWrite(tempFileName))
            {
                int bytesRead;
                var buffer = new byte[0x1000];
                while ((bytesRead = deployerConfiguration.Read(buffer, 0, buffer.Length)) != 0)
                {
                    tempFile.Write(buffer, 0, bytesRead);
                }
            }
            var config = Read(tempFileName);
            File.Delete(tempFileName);
            return config;
        }

        private static DeploymentMappings Read(string configFileName)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration File:{0}", configFileName);
            if (Properties.Settings.Default.SignDeploymentMappingFile)
            {
                if (!Encrypter.VerifyXml(configFileName, Properties.Settings.Default.KeyFile))
                {
                    TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Verification Failed for the deployment mapping file:{0} and key file {1}", configFileName, Properties.Settings.Default.KeyFile);
                    return null;
                }
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Verification Succeeded for the deployment mapping file:{0}", configFileName);
            }

            if (File.Exists(configFileName))
            {
                var serializer = new XmlSerializer(typeof(DeploymentMappings));
                using (TextReader reader = new StreamReader(configFileName))
                {
                    var config = (DeploymentMappings)serializer.Deserialize(reader);
                    return config;
                }
            }
            
            TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "Reading Configuration File:{0} failed.", configFileName);
            return null;
        }

    }
}
