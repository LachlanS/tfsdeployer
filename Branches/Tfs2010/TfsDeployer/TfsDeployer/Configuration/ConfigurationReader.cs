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
using Readify.Useful.TeamFoundation.Common;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IConfigurationSource _configurationSource;

        public ConfigurationReader(IConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
        }

        const string ConfigurationFileName = "DeployerConfiguration.xml";
        
        public IEnumerable<Mapping> ReadMappings(string teamProjectName, IBuildData teamBuild, string workingDirectory)
        {
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "Reading Configuration for Team Project: {0} Team Build: {1}", teamProjectName, teamBuild.BuildType);
            _configurationSource.CopyTo(workingDirectory);
            var configuration = ConfigurationReaderHelper.Read(Path.Combine(workingDirectory, ConfigurationFileName));
            if (configuration == null)
            {
                return new Mapping[0];
            }
            return configuration.Mappings.Where(m => Regex.IsMatch(teamBuild.BuildType, m.BuildDefinitionPattern)).ToArray(); 
        }
      
    }
}
