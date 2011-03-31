using System;
using System.IO;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public class TemporaryFile : IDisposable
    {
        private readonly FileInfo _info;

        public TemporaryFile()
        {
            _info = new FileInfo(Path.GetTempFileName());
        }

        public FileInfo FileInfo
        {
            get { return _info; }
        }

        public void Dispose()
        {
            try
            {
                _info.Delete();
            }
            catch (IOException ex)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, ex);
            }
        }
        
    }   
}