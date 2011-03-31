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
        public TemporaryFile(string extension, string content) : this(extension, content, string.Empty)
        {
        }

        public TemporaryFile(string extension, string content, string subDirectory)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;
            var fileName = Path.GetRandomFileName() + extension;
            var path = Path.GetTempPath();
            if (!string.IsNullOrEmpty(subDirectory))
            {
                path = Path.Combine(path, subDirectory);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
            _fileInfo = new FileInfo(Path.Combine(path, fileName));
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