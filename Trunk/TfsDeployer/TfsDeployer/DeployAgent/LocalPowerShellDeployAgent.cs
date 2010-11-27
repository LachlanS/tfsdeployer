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
using System.Security.Policy;

namespace TfsDeployer.DeployAgent
{
    public class LocalPowerShellDeployAgent : IDeployAgent
    {
        public DeployAgentResult Deploy(DeployAgentData deployAgentData)
        {
            var variables = CreateVariables(deployAgentData);
            var scriptPath = Path.Combine(deployAgentData.DeployScriptRoot, deployAgentData.DeployScriptFile);
            var result = ExecuteCommand(scriptPath, variables);

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
                               {"TfsDeployerBuildDetail", deployAgentData.TfsBuildDetail}
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

        private DeployAgentResult ExecuteCommand(string scriptPath, IDictionary<string, object> variables)
        {
            AppDomain scriptDomain = null;
            try
            {
                scriptDomain = AppDomain.CreateDomain(string.Format("{0}:{1}", GetType(), Guid.NewGuid()),
                                                      new Evidence(AppDomain.CurrentDomain.Evidence),
                                                      AppDomain.CurrentDomain.SetupInformation);

                var proxy = (LocalPowerShellScriptExecutor) scriptDomain.CreateInstanceAndUnwrap(
                    typeof(LocalPowerShellScriptExecutor).Assembly.FullName,
                    typeof(LocalPowerShellScriptExecutor).FullName);

                var result = proxy.Execute(scriptPath, variables);

                return result;
            }
            finally
            {
                if (scriptDomain != null)
                {
                    AppDomain.Unload(scriptDomain);
                }
            }
        }

    }
}