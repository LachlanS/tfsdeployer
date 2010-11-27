﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    public class LocalPowerShellScriptExecutor : MarshalByRefObject
    {
        public DeployAgentResult Execute(string scriptPath, IDictionary<string, object> variables)
        {
            var hasErrors = true;
            string output;

            try
            {
                var ui = new DeploymentHostUI();
                var host = new DeploymentHost(ui);
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
                        pipeline.StateChanged += (s, e) =>
                                                     {
                                                         if (e.PipelineStateInfo.State == PipelineState.Failed)
                                                             hasErrors = true;
                                                     };

                        var scriptCommand = new Command(scriptPath, true);
                        scriptCommand.MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                        pipeline.Commands.Add(scriptCommand);

                        pipeline.Commands.Add("Out-Default");

                        pipeline.Invoke();
                        hasErrors = ui.HasErrors;
                        output = ui.Output;
                    }
                }
            }
            catch (RuntimeException ex)
            {
                var record = ex.ErrorRecord;
                var sb = new StringBuilder();
                sb.AppendLine(record.Exception.ToString());
                sb.AppendLine(record.InvocationInfo.PositionMessage);
                output = sb.ToString();
            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }

            return new DeployAgentResult { HasErrors = hasErrors, Output = output };
        }
    }
}