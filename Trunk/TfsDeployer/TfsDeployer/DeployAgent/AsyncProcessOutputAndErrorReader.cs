using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TfsDeployer.DeployAgent
{
    public class AsyncProcessOutputAndErrorReader : IDisposable
    {
        private readonly SynchronizedStringBuilder _combinedOutput;
        private readonly Thread[] _threads;

        public AsyncProcessOutputAndErrorReader(Process process, StringBuilder combinedOutput)
        {
            _combinedOutput = new SynchronizedStringBuilder(combinedOutput);
            _threads = new[]
                           {
                               new Thread(() => CopyReaderToOutput(process.StandardOutput, _combinedOutput)),
                               new Thread(() => CopyReaderToOutput(process.StandardError, _combinedOutput))
                           };
        }

        public void BeginRead()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        public void EndRead()
        {
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }

        private void CopyReaderToOutput(TextReader reader, SynchronizedStringBuilder outputBuilder)
        {
            const int bufferSize = 100;
            var buffer = new char[bufferSize];
            int charsRead;
            do
            {
                charsRead = reader.Read(buffer, 0, buffer.Length);
                outputBuilder.Append(buffer, 0, charsRead);
            } while (charsRead > 0);
        }

        public void Dispose()
        {
            EndRead();
        }
    }
}