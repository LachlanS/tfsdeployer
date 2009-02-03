using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;
using System.IO;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    [TestClass]
    public class ExecuteTests
    {

        [TestMethod]
        public void ShouldOutputSufficientFailureDetailsWhenScriptStops()
        {
            string TestScriptFileName = Path.GetRandomFileName() + ".ps1";
            string TestDirectory = Path.GetTempPath();

            var ScriptFile = new FileInfo(Path.Combine(TestDirectory, TestScriptFileName));
            using (var stream = ScriptFile.OpenWrite())
            using (var writer = new StreamWriter(stream, Encoding.ASCII))
            {
                writer.Write(PowerShellScripts.FailingPowerShellScript);
            }

            var TestDeployData = new DeployAgentData
                                  {
                                      NewQuality = "Released",
                                      OriginalQuality = null,
                                      DeployScriptFile = TestScriptFileName,
                                      DeployScriptRoot = TestDirectory,
                                      DeployScriptParameters = new List<DeployScriptParameter>(),
                                      Tfs2008BuildDetail = new BuildDetail()
                                  };

            var psAgent = new LocalPowerShellDeployAgent();
            DeployAgentResult result;
            try
            {
                result = psAgent.Deploy(TestDeployData);
            }
            finally
            {
                ScriptFile.Delete();
            }

            Assert.IsTrue(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "<<<<", "Output"); // <<<< is pointer to error position
        }

        [TestMethod]
        public void ShouldReturnValueOfEnvironmentVariable()
        {
            var pr = new LocalPowerShellDeployAgent();
            pr.ExecuteCommand("$Env:TEMP", null);

            Assert.IsFalse(pr.ErrorOccurred, "ErrorOccurred");
            StringAssert.Contains(pr.Output, Environment.GetEnvironmentVariable("TEMP"));
        }

        [TestMethod]
        public void ShouldReturnFormattedObjects()
        {
            var pr = new LocalPowerShellDeployAgent();
            pr.ExecuteCommand("Get-ChildItem Env:", null);

            Assert.IsFalse(pr.ErrorOccurred, "ErrorOccurred");
            StringAssert.Contains(pr.Output, Environment.GetEnvironmentVariable("TEMP"));
        }

    }
}
