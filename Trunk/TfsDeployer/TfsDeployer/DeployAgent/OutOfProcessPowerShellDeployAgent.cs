using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using TfsDeployer.Journal;
using TfsDeployer.PowerShellAgent;

namespace TfsDeployer.DeployAgent
{
    public class OutOfProcessPowerShellDeployAgent : IDeployAgent
    {
        private readonly IDeploymentEventRecorder _deploymentEventRecorder;

        public OutOfProcessPowerShellDeployAgent(IDeploymentEventRecorder deploymentEventRecorder)
        {
            _deploymentEventRecorder = deploymentEventRecorder;
        }

        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            var request = new AgentRequest
                              {
                                  NoProfile = true,
                                  Command = PrepareImportGlobalVariablesScript(deployAgentData) + PrepareDeploymentScript(deployAgentData)
                              };

            var runner = new PowerShellAgentRunner(request, deployAgentData.DeployScriptRoot, deployAgentData.Timeout, ClrVersion.Version2);

            if (_deploymentEventRecorder != null)
            {
                _deploymentEventRecorder.SetDeploymentOutputDelegate(deployAgentData.DeploymentId, () => runner.Output);
            }
            
            var exitCode = runner.Run();
            var result = new DeployAgentResult {HasErrors = exitCode != 0, Output = runner.Output};
            return result;
        }

        private string PrepareDeploymentScript(DeployAgentData deployAgentData)
        {
            var fullDeployScriptPath = Path.Combine(deployAgentData.DeployScriptRoot, deployAgentData.DeployScriptFile);

            var commandBuilder = new StringBuilder();
            commandBuilder.AppendFormat("& '{0}'", fullDeployScriptPath.Replace("'", "''"));

            if (deployAgentData.DeployScriptParameters != null && deployAgentData.DeployScriptParameters.Any())
            {
                CommandInfo commandInfo;
                using (var shell = PowerShell.Create())
                {
                    shell.AddCommand("Get-Command").AddParameter("Name", fullDeployScriptPath);
                    commandInfo = shell.Invoke<CommandInfo>().FirstOrDefault();
                }

                foreach (var deployParam in deployAgentData.DeployScriptParameters)
                {
                    if (commandInfo.Parameters.ContainsKey(deployParam.Name))
                    {
                        var commandParam = commandInfo.Parameters[deployParam.Name];
                        if (commandParam.SwitchParameter)
                        {
                            var value = "$false";
                            if (Convert.ToBoolean(deployParam.Value))
                            {
                                value = "$true";
                            }
                            commandBuilder.AppendFormat(" -{0}:{1}", commandParam.Name, value);
                        }
                        else
                        {
                            commandBuilder.AppendFormat(" -{0} '{1}'", commandParam.Name, deployParam.Value.Replace("'","''"));
                        }
                        //TODO aliases, shortest unique name
                    }
                }
            }

            return commandBuilder.ToString();
        }

        private string PrepareImportGlobalVariablesScript(DeployAgentData deployAgentData)
        {
            const int serializationDepth = 5;
            const string clixmlFile = "TfsDeployer.variables.clixml";
            const string scriptTemplate = "(Import-Clixml -Path {0}).GetEnumerator() | ForEach-Object {{ Set-Variable -Name $_.Key -Value $_.Value -Scope global }};";

            var globalVariables = LocalPowerShellDeployAgent.CreateCommonVariables(deployAgentData);
            if (deployAgentData.DeployScriptParameters != null)
            {
                foreach (var deployParam in deployAgentData.DeployScriptParameters)
                {
                    globalVariables[deployParam.Name] = deployParam.Value;
                }
            }

            using (var shell = PowerShell.Create())
            {
                var clixmlPath = Path.Combine(deployAgentData.DeployScriptRoot, clixmlFile);
                shell.AddCommand("Export-Clixml").AddParameter("Depth", serializationDepth).AddParameter("Path", clixmlPath);
                shell.Invoke(new[] { globalVariables });
            }

            return String.Format(scriptTemplate, clixmlFile);
        }

    }
}