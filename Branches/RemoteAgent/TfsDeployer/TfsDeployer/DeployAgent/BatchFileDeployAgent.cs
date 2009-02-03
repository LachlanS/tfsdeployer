using System.Diagnostics;
using System.IO;
using System.Text;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer.DeployAgent
{
    public class BatchFileDeployAgent : IDeployAgent
    {
        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            var errorOccurred = true;
            string output = string.Empty;

            var scriptToRun = Path.Combine(deployAgentData.DeployScriptRoot, deployAgentData.DeployScriptFile);

            if (!File.Exists(scriptToRun))
            {
                TraceHelper.TraceWarning(TraceSwitches.TfsDeployer, "BatchRunner - Could not find script: {0}", scriptToRun);
                output = string.Format("BatchRunner - Could not find script: {0}", scriptToRun);
            }
            else
            {
                var psi = new ProcessStartInfo(scriptToRun);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardError = true;
                psi.WorkingDirectory = deployAgentData.DeployScriptRoot;
                psi.Arguments = CreateArguments(deployAgentData);

                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "BatchRunner - Executing Scripts: {0} with arguments {1} in working directory {2}", scriptToRun, psi.Arguments, psi.WorkingDirectory);

                // Start the process
                var proc = Process.Start(psi);
                if (proc == null)
                {
                    TraceHelper.TraceError(TraceSwitches.TfsDeployer, "Process.Start(...) returned null");
                }
                else 
                {

                    using (var sOut = proc.StandardOutput)
                    {
                        proc.WaitForExit();
                        output = sOut.ReadToEnd().Trim();
                        TraceHelper.TraceInformation(TraceSwitches.TfsDeployer, "BatchRunner - Output From Command: {0}", output);
                    }

                    errorOccurred = false;
                }

            }

            var result = new DeployAgentResult
                             {
                                 HasErrors = errorOccurred,
                                 Output = output
                             };

            return result;
        }

        private static string CreateArguments(DeployAgentData deployAgentData)
        {
            var buildData = deployAgentData.Tfs2008BuildDetail;
            var arguments = new StringBuilder();
            arguments.AppendFormat("{0}, {1} ", buildData.DropLocation, buildData.BuildNumber);
            foreach (var param in deployAgentData.DeployScriptParameters)
            {
                arguments.AppendFormat(", {0}", param.Value);
            }
            return arguments.ToString();
        }

    }
}