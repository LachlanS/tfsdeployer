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

        private bool _errorOccured = true;
        public bool ErrorOccured
        {
            get { return _errorOccured; }
        }

        private string GeneratePipelineCommand(string directory, Mapping mapToRun)
        {
            string command = Path.Combine(directory, mapToRun.Script);
            command = string.Format(".\"{0}\"", command);
            return command;
        }

        private void PopulateCommonVariables(Runspace space, Mapping mapToRun, BuildData buildData)
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
                buildData
                );
        }

        private void PopulateVariables(Runspace space, Mapping mapToRun, BuildData buildData)
        {
            this.PopulateCommonVariables(space, mapToRun, buildData);

            foreach (ScriptParameter parameter in mapToRun.ScriptParameters)
            {
                space.SessionStateProxy.SetVariable(parameter.name, parameter.value);
            }
        }

        private void EnsureExecutionPolicy(Runspace space)
        {
			Pipeline executionPolicyPipeline = space.CreatePipeline("Set-ExecutionPolicy Unrestricted");
			executionPolicyPipeline.Invoke();
        }

        public bool Execute(string directory, Mapping mapToRun, BuildData buildData)
        {
            try
            {
                DeploymentHost host = new DeploymentHost();
                Runspace space = RunspaceFactory.CreateRunspace(host);
                space.Open();

                this.PopulateVariables(space, mapToRun, buildData);

                string command = this.GeneratePipelineCommand(directory, mapToRun);
                this._scriptRun = command;

				this.EnsureExecutionPolicy(space);
                Pipeline pipeline = space.CreatePipeline(command);
                Collection<PSObject> outputObjects = pipeline.Invoke();

                if (pipeline.PipelineStateInfo.State != PipelineState.Failed)
                {
                    this._errorOccured = false;
                }

                string output = this.GenerateOutputFromObjects(outputObjects);
                this._output = output;

                space.Close();
            }
            catch (Exception ex)
            {
                this._errorOccured = true;
                this._output = ex.ToString();
            }

            return this.ErrorOccured;
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
