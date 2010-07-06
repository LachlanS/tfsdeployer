using System.IO;
using Microsoft.TeamFoundation.Build.Client;
using TfsDeployer;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    internal class StubDeploymentFileSource : IDeploymentFileSource
    {
        private readonly string _configurationXml;

        public StubDeploymentFileSource(string configurationXml)
        {
            _configurationXml = configurationXml;
        }

        public bool DownloadDeploymentFile(IBuildDetail buildDetail, string destination)
        {
            File.WriteAllText(destination, _configurationXml);
            return true;
        }
    }
}