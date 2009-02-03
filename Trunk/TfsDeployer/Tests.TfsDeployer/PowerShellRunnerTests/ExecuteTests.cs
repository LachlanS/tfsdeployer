using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;
using TfsDeployer.Runner;
using System.IO;

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

            var TestMapping = new Mapping
                                  {
                                      Computer = Environment.MachineName,
                                      NewQuality = "Released",
                                      OriginalQuality = null,
                                      Script = TestScriptFileName,
                                      ScriptParameters = new List<ScriptParameter>()
                                  };

            var TestBuildDetail = new StubBuildDetail();

            var TestBuildInfo = new global::TfsDeployer.BuildInformation(TestBuildDetail);


            IRunner pr = new RunnerToDeployAgentAdapter(new LocalPowerShellDeployAgent());
            bool result;
            try
            {
                result = pr.Execute(TestDirectory, TestMapping, TestBuildInfo);
            }
            finally
            {
                ScriptFile.Delete();
            }

            Assert.IsTrue(result, "bool IRunnerExecute( , , )");
            Assert.IsTrue(pr.ErrorOccurred, "IRunner.ErrorOccurred");
            StringAssert.Contains(pr.Output, "<<<<", "IRunner.Output"); // <<<< is pointer to error position

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
