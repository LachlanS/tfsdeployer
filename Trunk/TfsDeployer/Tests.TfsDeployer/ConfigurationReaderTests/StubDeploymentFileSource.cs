using System.IO;
using System.Text;
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

        public Stream DownloadDeploymentFile(BuildDetail buildDetail)
        {
            var stream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(_configurationXml);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}