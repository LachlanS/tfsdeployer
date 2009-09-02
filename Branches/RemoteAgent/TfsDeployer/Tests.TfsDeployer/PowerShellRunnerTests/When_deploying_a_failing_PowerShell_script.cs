using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    [TestClass]
    public class When_deploying_a_failing_PowerShell_script
    {

        private static DeployAgentResult DeployFailingPowerShellScript()
        {
            string testScriptFileName = Path.GetRandomFileName() + ".ps1";
            string testDirectory = Path.GetTempPath();

            var scriptFile = new FileInfo(Path.Combine(testDirectory, testScriptFileName));
            using (var stream = scriptFile.OpenWrite())
            using (var writer = new StreamWriter(stream, Encoding.ASCII))
            {
                writer.Write(PowerShellScripts.FailingPowerShellScript);
            }

            var testDeployData = new DeployAgentData
                                     {
                                         NewQuality = "Released",
                                         OriginalQuality = null,
                                         DeployScriptFile = testScriptFileName,
                                         DeployScriptRoot = testDirectory,
                                         DeployScriptParameters = new List<DeployScriptParameter>(),
                                         Tfs2008BuildDetail = new BuildDetail()
                                     };

            var psAgent = new LocalPowerShellDeployAgent();
            DeployAgentResult result;
            try
            {
                result = psAgent.Deploy(testDeployData);
            }
            finally
            {
                scriptFile.Delete();
            }

            return result;
        }

        [TestMethod]
        public void Should_output_sufficient_failure_details()
        {
            var result = DeployFailingPowerShellScript();
            Assert.IsTrue(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "<<<<", "Output"); // <<<< is pointer to error position
        }

        [TestMethod]
        public void Should_return_output_generated_prior_to_the_failure_point()
        {
            var result = DeployFailingPowerShellScript();
            Assert.IsTrue(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Output this before failing", "Output");
        }

    }
}