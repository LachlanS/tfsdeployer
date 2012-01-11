using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using TfsDeployer.PowerShellAgent;

namespace TfsDeployer.DeployAgent
{
    public class OutOfProcessPowerShellAgent
    {
        private readonly AgentRequest _request;
        private readonly string _workingDirectory;
        private readonly TimeSpan _timeout;
        private readonly StringBuilder _outputBuilder;

        public OutOfProcessPowerShellAgent(AgentRequest request, string workingDirectory, TimeSpan timeout)
        {
            _outputBuilder = new StringBuilder();
            _request = request;
            _workingDirectory = workingDirectory;
            _timeout = timeout;
        }

        public int Run()
        {
            _outputBuilder.Remove(0, _outputBuilder.Length);

            var process = StartProcessWithRequest(_request, _workingDirectory);

            using (var processReader = new AsyncProcessOutputAndErrorReader(process, _outputBuilder))
            {
                processReader.BeginRead();

                WaitForExit(process, _timeout);
            }

            return process.ExitCode;
        }

        public string Output { get { return _outputBuilder.ToString(); } }

        private static void WaitForExit(Process process, TimeSpan timeOut)
        {
            var timeoutMilliseconds = (int) timeOut.TotalMilliseconds;
            if (timeoutMilliseconds > 0)
            {
                var hasExited = process.WaitForExit(timeoutMilliseconds);
                if (!hasExited)
                {
                    process.Kill(); //Recursive();?
                }
            }
            else
            {
                process.WaitForExit();
            }
        }

        private Process StartProcessWithRequest(AgentRequest request, string workingDirectory)
        {
            var agentPath = request.GetType().Assembly.Location;

            var startInfo = new ProcessStartInfo(agentPath)
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            var namedPipeName = string.Format("{0}.{1}", GetType().FullName, Guid.NewGuid());
            startInfo.Arguments = namedPipeName;
            using (var pipeServer = new NamedPipeServerStream(namedPipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                var messagePipe = new MessagePipe(pipeServer);

                var ar = pipeServer.BeginWaitForConnection(null, pipeServer);

                var process = Process.Start(startInfo);

                pipeServer.EndWaitForConnection(ar);

                messagePipe.WriteMessage(request);

                pipeServer.WaitForPipeDrain();

                return process;
            }
        }
    }
}
