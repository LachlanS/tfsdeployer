using System.IO;
using TfsDeployer;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.ConfigurationReaderTests
{
    internal class StubDeploymentFileSource : IDeploymentFileSource
    {
        private readonly string _configurationXml;

        public StubDeploymentFileSource(string configurationXml)
        {
            _configurationXml = configurationXml;
        }

        public bool DownloadDeploymentFile(BuildDetail buildDetail, string destination)
        {
            File.WriteAllText(destination, _configurationXml);
            return true;
        }
    }
}