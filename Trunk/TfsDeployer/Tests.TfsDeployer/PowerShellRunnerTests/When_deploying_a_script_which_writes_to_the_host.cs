using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TfsDeployer.DeployAgent;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    [TestClass]
    public class When_deploying_a_script_which_writes_to_the_host
    {
        [TestMethod]
        public void Should_include_written_text_in_output()
        {
            var deploymentEventRecorder = MockRepository.GenerateStub<IDeploymentEventRecorder>();
            
            using (var scriptFile = TemporaryFile.FromResource(".ps1", "Tests.TfsDeployer.PowerShellRunnerTests.WriteHostScript.ps1"))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var psAgent = new LocalPowerShellDeployAgent(deploymentEventRecorder);
                var result = psAgent.Deploy(testDeployData);

                Assert.AreEqual("Hello world\n", result.Output);
            }
        }
    }
}