using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Tests.TfsDeployer
{
    class TemporaryFile : IDisposable
    {
        public static TemporaryFile FromResource(string extension, string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) return null;
            using (var reader = new StreamReader(stream))
            {
                return new TemporaryFile(extension, reader.ReadToEnd());
            }
        }

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