using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var TestMapping = new Mapping() 
            {
                Computer = Environment.MachineName,
                NewQuality = "Released",
                OriginalQuality = null,
                Script = TestScriptFileName,
                ScriptParameters = new List<ScriptParameter>()
            };

            var TestBuildDetail = new StubBuildDetail();

            var TestBuildInfo = new global::TfsDeployer.BuildInformation(TestBuildDetail);


            IRunner pr = new PowerShellRunner();
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


    }
}
