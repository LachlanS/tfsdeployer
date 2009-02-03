using System;
using System.IO;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public class WorkingDirectory : IWorkingDirectory
    {
        private readonly DirectoryInfo _info;

        public WorkingDirectory()
        {
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
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }

    }
}