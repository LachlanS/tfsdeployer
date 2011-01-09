using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    public class LocalPowerShellScriptExecutor : MarshalByRefObject
    {
        private DeploymentHostUI _ui;

        public DeployAgentResult Execute(string commandText, IDictionary<string, object> variables)
        {
            var hasErrors = true;
            string output;

            _ui = new DeploymentHostUI();
            try
            {
                var host = new DeploymentHost(_ui);
                using (var space = RunspaceFactory.CreateRunspace(host))
                {
                    space.Open();

                    if (null != variables)
                    {
                        foreach (var key in variables.Keys)
                        {
                            space.SessionStateProxy.SetVariable(key, variables[key]);
                        }
                    }

                    using (var pipeline = space.CreatePipeline())
                    {
                        var scriptCommand = new Command(commandText, true);
                        scriptCommand.MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                        pipeline.Commands.Add(scriptCommand);

                        pipeline.Commands.Add("Out-Default");

                        pipeline.Invoke();
                        hasErrors = _ui.HasErrors;
                        output = _ui.Output;
                    }
                }
            }
            catch (RuntimeException ex)
            {
                var record = ex.ErrorRecord;
                var sb = new StringBuilder();
                sb.AppendLine(record.Exception.ToString());
                sb.AppendLine(record.InvocationInfo.PositionMessage);
                output = string.Format("{0}\n{1}", _ui.Output, sb);
            }
            catch (Exception ex)
            {
                output = string.Format("{0}\n{1}", _ui.Output, ex);
            }

            return new DeployAgentResult { HasErrors = hasErrors, Output = output };
        }

        public string LiveOutput { 
            get
            {
                if (_ui == null || _ui.Output == null)
                {
                    return string.Empty;
                }
                return _ui.Output;
            }
        }
    }
}