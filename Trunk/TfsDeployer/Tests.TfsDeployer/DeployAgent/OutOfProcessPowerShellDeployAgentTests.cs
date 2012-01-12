using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
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

                var agent = new OutOfProcessPowerShellDeployAgent();
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

                var agent = new OutOfProcessPowerShellDeployAgent();
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

                var agent = new OutOfProcessPowerShellDeployAgent();
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

                var agent = new OutOfProcessPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);
            }

            // Assert
            StringAssert.Contains(result.Output, "Description:My Process Template");
        }

    }
}
