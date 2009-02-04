using System;
using System.IO;

namespace TfsDeployer
{
    public class WorkingDirectory : IWorkingDirectory
    {
        private readonly DirectoryInfo _info;
        private readonly ILog _log;

        public WorkingDirectory(ILog log)
        {
            _log = log;
            var tempRoot = Path.GetTempPath();
            _info = new DirectoryInfo(Path.Combine(tempRoot, Guid.NewGuid().ToString()));
            _info.Create();
        }

        public DirectoryInfo DirectoryInfo
        {
            get { return _info; }
        }

        public void Dispose()
        {
            try
            {
                var allFiles = _info.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (var file in allFiles)
                {
                    file.Attributes = FileAttributes.Normal;
                }
                _info.Delete(true);
            }
            catch (IOException ex)
            {
                _log.Error(ex);
            }
        }

    }
}