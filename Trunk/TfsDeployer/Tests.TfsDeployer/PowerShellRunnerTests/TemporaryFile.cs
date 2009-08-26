using System;
using System.IO;
using System.Text;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    class TemporaryFile : IDisposable
    {
        private readonly FileInfo _fileInfo;
        public TemporaryFile(string extension, string content)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;
            var fileName = Path.GetRandomFileName() + extension;
            _fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));
            using (var stream = _fileInfo.OpenWrite())
            using (var writer = new StreamWriter(stream, Encoding.ASCII))
            {
                writer.Write(content);
            }
        }

        public FileInfo FileInfo
        {
            get { return _fileInfo;  }
        }

        public void Dispose()
        {
            _fileInfo.Delete();
        }
    }
}