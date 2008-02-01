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
using System.Text;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Management.Automation;
using Microsoft.TeamFoundation.Build.Proxy;
using System.Collections.ObjectModel;

namespace TfsDeployer.Runner
{
    public class PowerShellRunner : IRunner
    {
        #region IRunner Members

        private string _scriptRun;
        public string ScriptRun
        {
            get { return _scriptRun; }
        }

        private bool _errorOccurred = true;
        public bool ErrorOccurred
        {
            get { return _errorOccurred; }
        }

        private string GeneratePipelineCommand(string directory, Mapping mapToRun)
        {
            string command = Path.Combine(directory, mapToRun.Script);
            command = string.Format(".\"{0}\"", command);
            return command;
        }

        private void PopulateCommonVariables(Runspace space, Mapping mapToRun, BuildInformation buildInfo)
        {
            space.SessionStateProxy.SetVariable( 
                "TfsDeployerComputer",
                mapToRun.Computer
                );
            space.SessionStateProxy.SetVariable(
                "TfsDeployerNewQuality",
                mapToRun.NewQuality
                );
            space.SessionStateProxy.SetVariable(
                "TfsDeployerOriginalQuality",
                mapToRun.OriginalQuality
                );
            space.SessionStateProxy.SetVariable(
                "TfsDeployerScript",
                mapToRun.Script
                );
            space.SessionStateProxy.SetVariable(
                "TfsDeployerBuildData",
                buildInfo.Data 
                );
            space.SessionStateProxy.SetVariable(
                "TfsDeployerBuildDetail",
                buildInfo.Detail 
                );
        }

        private void PopulateVariables(Runspace space, Mapping mapToRun, BuildInformation buildInfo)
        {
            this.PopulateCommonVariables(space, mapToRun, buildInfo);

            foreach (ScriptParameter parameter in mapToRun.ScriptParameters)
            {
                space.SessionStateProxy.SetVariable(parameter.name, parameter.value);
            }
        }

        //private void EnsureExecutionPolicy(Runspace space)
        //{
        //    Pipeline executionPolicyPipeline = space.CreatePipeline("Set-ExecutionPolicy Unrestricted");
        //    executionPolicyPipeline.Invoke();
        //}

        public bool Execute(string directory, Mapping mapToRun, BuildInformation buildInfo)
        {
            try
            {
                DeploymentHost host = new DeploymentHost();
                Runspace space = RunspaceFactory.CreateRunspace(host);
                space.Open();

                this.PopulateVariables(space, mapToRun, buildInfo);

                string command = this.GeneratePipelineCommand(directory, mapToRun);
                this._scriptRun = command;

                // this prevents TfsDeployer running as a non-admin user.
                // installation documentation should describe setting execution policy and possibly using signed scripts
                //this.EnsureExecutionPolicy(space); 

                Pipeline pipeline = space.CreatePipeline(command);
                Collection<PSObject> outputObjects = pipeline.Invoke();
                if (pipeline.PipelineStateInfo.State != PipelineState.Failed)
                {
                    this._errorOccurred = false;
                }

                string output = this.GenerateOutputFromObjects(outputObjects);
                this._output = output;

                space.Close();
            }
            catch (RuntimeException ex)
            {
                this._errorOccurred = true;
                ErrorRecord record = ex.ErrorRecord;
                var sb = new StringBuilder();
                sb.AppendLine(record.Exception.ToString());
                sb.AppendLine(record.InvocationInfo.PositionMessage);
                this._output = sb.ToString();
            }
            catch (Exception ex)
            {
                this._errorOccurred = true;
                this._output = ex.ToString();
            }

            return this.ErrorOccurred;
        }

        private string GenerateOutputFromObjects(Collection<PSObject> outputObjects)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\n");

            int lineCount = 0;
            foreach (PSObject outputObject in outputObjects)
            {
                builder.AppendFormat("{0}:{1}\n", lineCount, outputObject);
                lineCount++;
            }

            return builder.ToString();
        }

        private string _output;

        public string Output
        {
            get
            {
                return this._output;
            }
        }

        #endregion
    }
}
