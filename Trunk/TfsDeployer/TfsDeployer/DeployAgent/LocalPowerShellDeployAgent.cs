// Copyright (c) 2007 Readify Pty. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    public class LocalPowerShellDeployAgent : IDeployAgent
    {
        private bool _errorOccurred = true;

        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            var variables = CreateVariables(deployAgentData);
            var scriptPath = Path.Combine(deployAgentData.DeployScriptRoot, deployAgentData.DeployScriptFile);

            ExecuteCommand(scriptPath, variables);

            var result = new DeployAgentResult
                             {
                                 HasErrors = _errorOccurred,
                                 Output = Output
                             };

            return result;
        }

        private static IDictionary<string, object> CreateCommonVariables(DeployAgentData deployAgentData)
        {
            var dict = new Dictionary<string, object>
                           {
                               {"TfsDeployerComputer", deployAgentData.DeployServer},
                               {"TfsDeployerNewQuality", deployAgentData.NewQuality},
                               {"TfsDeployerOriginalQuality", deployAgentData.OriginalQuality},
                               {"TfsDeployerScript", deployAgentData.DeployScriptFile},
                               {"TfsDeployerBuildData", deployAgentData.Tfs2005BuildData},
                               {"TfsDeployerBuildDetail", deployAgentData.Tfs2008BuildDetail}
                           };
            return dict;
        }

        private static IDictionary<string, object> CreateVariables(DeployAgentData deployAgentData)
        {
            var dict = CreateCommonVariables(deployAgentData);

            foreach (var parameter in deployAgentData.DeployScriptParameters)
            {
                dict.Add(parameter.Name, parameter.Value);
            }

            return dict;
        }

        public void ExecuteCommand(string scriptPath, IDictionary<string, object> variables)
        {
            try
            {
                _errorOccurred = true;

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
                        pipeline.StateChanged += PipelineStateChanged;

                        var scriptCommand = new Command(scriptPath, true);
                        scriptCommand.MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                        pipeline.Commands.Add(scriptCommand);

                        pipeline.Commands.Add("Out-Default");

                        pipeline.Invoke();
                        _errorOccurred = ui.HasErrors;
                        Output = ui.Output;
                    }
                }
            }
            catch (RuntimeException ex)
            {
                var record = ex.ErrorRecord;
                var sb = new StringBuilder();
                sb.AppendLine(record.Exception.ToString());
                sb.AppendLine(record.InvocationInfo.PositionMessage);
                Output = sb.ToString();
            }
            catch (Exception ex)
            {
                Output = ex.ToString();
            }
        }

        void PipelineStateChanged(object sender, PipelineStateEventArgs e)
        {
            if (e.PipelineStateInfo.State == PipelineState.Failed) _errorOccurred = true;
        }

        public bool ErrorOccurred
        {
            get
            {
                return _errorOccurred;
            }
        }

        public string Output { get; private set; }
    }
}