using System.Diagnostics;
using System.IO;
using System.Linq;
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
                Arguments = CreateArguments(deployAgentData),
            };
            TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "BatchRunner - Executing Scripts: {0} with arguments {1} in working directory {2}", scriptToRun, psi.Arguments, psi.WorkingDirectory);

            // Start the process
            var proc = Process.Start(psi);
            if (proc == null)
            {
                TraceHelper.TraceError(TraceSwitches.TfsDeployer, "Process.Start(...) returned null");

                return new DeployAgentResult
                {
                    HasErrors = true,
                    Output = "Process.Start(...) returned null. Could not create deployment process.",
                };
            }

            using (var sOut = proc.StandardOutput)
            {
                using (var sErr = proc.StandardError)
                {
                    var errorOccurred = true;
                    string output = string.Empty;

                    if (proc.WaitForExit((int)deployAgentData.Timeout.TotalMilliseconds))
                    {
                        errorOccurred = false;
                    }
                    else
                    {
                        try
                        {
                            proc.KillRecursive();
                        }
                        catch (InvalidOperationException)
                        {
                            // swallow. This occurs if the process is killed between when we decide to kill it and when we actually make the call.
                        }

                        output += "The deployment process failed to complete within the specified time limit and was terminated.\n";
                    }

                    // try to read from the process's output stream even if we've killed it. This is an entirely legitimate
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
