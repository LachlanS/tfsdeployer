using System;
using System.IO.Pipes;

namespace TfsDeployer.PowerShellAgent
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("This program is designed to be used by TFS Deployer only and not used directly");
                return -1;
            }

            var pipeName = args[0];
            var request = ReadRequest(pipeName);

            var shell = new Shell();
            return shell.Run(request);
        }

        private static AgentRequest ReadRequest(string pipeName)
        {
            using (var pipe = new NamedPipeClientStream(pipeName))
            {
                pipe.Connect();
                var messagePipe = new MessagePipe(pipe);
                return messagePipe.ReadMessage<AgentRequest>();
            }
        }


        //private static void RunPowerShell(string requestData)
        //{
        //    var powerShellPath = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\PowerShell\1\PowerShellEngine").GetValue("ApplicationBase").ToString();

        //    var arguments = new StringBuilder();
        //    arguments.Append(" -version 2.0"); // TODO switch version
        //    arguments.Append(" -nologo");
        //    arguments.Append(" -inputformat none");
        //    arguments.Append(" -noninteractive");
        //    arguments.Append(" -noprofile"); // TODO toggle
        //    arguments.Append(" -executionpolicy remotesigned"); // TODO switch policy
        //    //arguments.Append(" -sta"); // TODO toggle MTA/STA
        //    arguments.AppendFormat(" -command (get-date);write-host '{0}'", requestData);

        //    var startInfo = new ProcessStartInfo(Path.Combine(powerShellPath, "powershell.exe"), arguments.ToString())
        //                        {
        //                            UseShellExecute = false,
        //                            WorkingDirectory = Path.GetTempPath(),
        //                            RedirectStandardOutput = true,
        //                            RedirectStandardError = true
        //                        };
        //    var process = Process.Start(startInfo);
        //    process.OutputDataReceived += (s, e) => Console.WriteLine("PS> " + e.Data);
        //    process.ErrorDataReceived += (s, e) => Console.Error.WriteLine("PSERR> " + e.Data);
        //    process.BeginOutputReadLine();
        //    process.BeginErrorReadLine();
        //    process.WaitForExit();
        //}
    }

}
