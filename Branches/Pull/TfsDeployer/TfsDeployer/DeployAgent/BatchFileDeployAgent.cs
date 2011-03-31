using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Readify.Useful.TeamFoundation.Common;
using System;

namespace TfsDeployer.DeployAgent
{
    public class BatchFileDeployAgent : IDeployAgent
    {
        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            //FIXME this method does way too much now. refactor.  -andrewh 27/10/2010

            var scriptToRun = Path.Combine(deployAgentData.DeployScriptRoot, deployAgentData.DeployScriptFile);

            if (!File.Exists(scriptToRun))
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "BatchRunner - Could not find script: {0}", scriptToRun);

                return new DeployAgentResult
                {
                    HasErrors = true,
                    Output = string.Format("BatchRunner - Could not find script: {0}", scriptToRun),
                };
            }

            var psi = new ProcessStartInfo(scriptToRun)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                WorkingDirectory = deployAgentData.DeployScriptRoot,
                CreateNoWindow = true,
                Arguments = CreateArguments(deployAgentData),
            };
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "BatchRunner - Executing Scripts: {0} with arguments {1} in working directory {2}", scriptToRun, psi.Arguments, psi.WorkingDirectory);

            // Start the process
            var proc = Process.Start(psi);

            using (var sOut = proc.StandardOutput)
            {
                using (var sErr = proc.StandardError)
                {
                    var errorOccurred = false;
                    var output = string.Empty;

                    // The calls to .ReadToEnd() below will block so we can't make those calls and also check timeouts in the same thread.
                    new Thread(() =>
                    {
                        // FIXME we should extract this to an instance method but this class needs to be refactored first so that it's a single-use,
                        // throw-away class.  -andrewh 22/12/2010

                        var timeoutMilliseconds = int.MaxValue;
                        if (deployAgentData.Timeout != TimeSpan.MaxValue) {
                            timeoutMilliseconds = (int)Math.Floor(deployAgentData.Timeout.TotalMilliseconds);
                        }
                        var hasExited = proc.WaitForExit(timeoutMilliseconds);

                        if (hasExited) return;

                        try
                        {
                            errorOccurred = true;
                            proc.KillRecursive();
                        }
                        catch (InvalidOperationException)
                        {
                            // swallow. This occurs if the process exits or is killed between when we decide to kill it and when we actually make the call.
                        }
                    })
                    {
                        IsBackground = true,
                        Name = "StalledDeploymentMonitor",
                    }.Start();

                    // try to read from the process's output stream - even if we've killed it. This is an entirely legitimate
                    // thing to do and allows us to capture at least partial output and send it back with any alerts.
                    try
                    {
                        output += sOut.ReadToEnd().Trim();
                        output += sErr.ReadToEnd().Trim();
                    }
                    catch
                    {
                        // swallow. grab what we can, but don't complain if the streams are nuked already.
                    }

                    if (errorOccurred)
                    {
                        output += "The deployment process failed to complete within the specified time limit and was terminated.\n";
                    }

                    TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "BatchRunner - Output From Command: {0}", output);

                    return new DeployAgentResult
                    {
                        HasErrors = errorOccurred,
                        Output = output
                    };
                }
            }
        }

        private static string EscapeArgument(string argument)
        {
            if (argument.Contains("\""))
            {
                argument = argument.Replace("\"", "\\\"");
            }
            if (argument.Contains(" "))
            {
                argument = "\"" + argument + "\"";
            }
            return argument;
        }

        private static string CreateArguments(DeployAgentData deployAgentData)
        {
            var buildDetail = deployAgentData.TfsBuildDetail;

            var defaultArguments = new[] { buildDetail.DropLocation, buildDetail.BuildNumber };

            var extraArguments = deployAgentData.DeployScriptParameters.Select(p => p.Value);

            var escapedArguments = defaultArguments.Concat(extraArguments).Select(EscapeArgument);

            return string.Join(" ", escapedArguments.ToArray());
        }

    }
}
