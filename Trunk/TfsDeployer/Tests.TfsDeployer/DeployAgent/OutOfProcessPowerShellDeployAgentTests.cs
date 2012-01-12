using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class OutOfProcessPowerShellDeployAgentTests
    {
        [TestMethod]
        public void OutOfProcessPowerShellDeployAgent_should_pass_a_DeployScriptParameter_with_spaces_as_a_PowerShell_script_parameter()
        {
            using (var scriptFile = new TemporaryFile(".ps1", "param($Foo) \"Foo=$Foo\" "))
            {
                var data = new DeployAgentData
                               {
                                   DeployScriptFile = scriptFile.FileInfo.Name,
                                   DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                                   DeployScriptParameters = new[]
                                                                {
                                                                    new DeployScriptParameter {Name = "Foo", Value = "Bar None"}
                                                                },
                                   TfsBuildDetail = new BuildDetail()
                               };

                var agent = new OutOfProcessPowerShellDeployAgent(null, ClrVersion.Version2);
                var result = agent.Deploy(data);

                StringAssert.Contains(result.Output, "Foo=Bar None");
            }
            
        }

        [TestMethod]
        public void OutOfProcessPowerShellDeployAgent_should_pass_a_DeployScriptParameter_with_special_characters_as_a_PowerShell_script_parameter()
        {
            using (var scriptFile = new TemporaryFile(".ps1", "param($Foo) \"Foo=$Foo\" "))
            {
                var data = new DeployAgentData
                {
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new[]
                                                                {
                                                                    new DeployScriptParameter {Name = "Foo", Value = "Who's going to pay $15 for a \"good\" beer?"}
                                                                },
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new OutOfProcessPowerShellDeployAgent(null, ClrVersion.Version2);
                var result = agent.Deploy(data);

                StringAssert.Contains(result.Output, "Foo=Who's going to pay $15 for a \"good\" beer?");
            }

        }

        [TestMethod]
        public void OutOfProcessPowerShellDeployAgent_should_pass_a_DeployScriptParameter_as_a_PowerShell_script_switch_parameter()
        {
            using (var scriptFile = new TemporaryFile(".ps1", "param([switch]$Foo) \"Foo=$Foo\" "))
            {
                var data = new DeployAgentData
                {
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new[]
                                                                {
                                                                    new DeployScriptParameter {Name = "Foo", Value = "False"}
                                                                },
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new OutOfProcessPowerShellDeployAgent(null, ClrVersion.Version2);
                var result = agent.Deploy(data);

                StringAssert.Contains(result.Output, "Foo=False");
            }

        }

        [TestMethod]
        public void OutOfProcessPowerShellDeployAgent_should_serialize_build_detail_across_processes()
        {
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", "'Description:' + $TfsDeployerBuildDetail.BuildDefinition.Process.Description"))
            {
                var buildDetail = new BuildDetail {BuildDefinition = {Process = {Description = "My Process Template"}}};

                var mapping = new Mapping
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    ScriptParameters = new ScriptParameter[0],
                    Script = scriptFile.FileInfo.Name
                };

                var buildStatusChangeEvent = new BuildStatusChangeEvent { StatusChange = new Change() };

                var testDeployData = (new DeployAgentDataFactory()).Create(scriptFile.FileInfo.DirectoryName, mapping, buildDetail, buildStatusChangeEvent);

                var agent = new OutOfProcessPowerShellDeployAgent(null, ClrVersion.Version2);

                // Act
                result = agent.Deploy(testDeployData);
            }

            // Assert
            StringAssert.Contains(result.Output, "Description:My Process Template");
        }

        [TestMethod]
        public void OutOfProcessPowerShellDeployAgent_should_expose_live_output_to_the_deployment_event_recorder()
        {
            var deploymentEventRecorder = new StubDeploymentEventRecorder();

            using (var scriptFile = new TemporaryFile(".ps1", "'hello there'"))
            {
                var data = new DeployAgentData
                {
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName
                };

                var agent = new OutOfProcessPowerShellDeployAgent(deploymentEventRecorder, ClrVersion.Version2);
                agent.Deploy(data);
            }

            StringAssert.Contains(deploymentEventRecorder.OutputDelegate(), "hello there");
        }

        class StubDeploymentEventRecorder : IDeploymentEventRecorder
        {
            public Func<string> OutputDelegate;

            public int RecordTriggered(string buildNumber, string teamProject, string teamProjectCollectionUri, string triggeredBy, string originalQuality, string newQuality)
            {
                throw new NotImplementedException();
            }

            public int RecordQueued(int eventId, string script, string queue)
            {
                throw new NotImplementedException();
            }

            public void RecordStarted(int deploymentId)
            {
                throw new NotImplementedException();
            }

            public void RecordFinished(int deploymentId, bool hasErrors, string finalOutput)
            {
                throw new NotImplementedException();
            }

            public void SetDeploymentOutputDelegate(int deploymentId, Func<string> outputDelegate)
            {
                OutputDelegate = outputDelegate;
            }
        }

    }
}
