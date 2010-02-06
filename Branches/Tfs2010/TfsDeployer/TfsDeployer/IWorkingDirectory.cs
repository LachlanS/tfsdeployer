using System;
using System.IO;

namespace TfsDeployer
{
    public interface IWorkingDirectory : IDisposable
    {
        DirectoryInfo DirectoryInfo { get; }
    }
}