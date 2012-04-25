using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class LocalPowerShellScriptExecutorTests
    {
        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_value_of_environment_variable()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var result = executor.Execute("$Env:TEMP", null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, Environment.GetEnvironmentVariable("TEMP"));
        }

        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_formatted_objects()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var result = executor.Execute("Get-ChildItem Env:", null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, Environment.GetEnvironmentVariable("TEMP"));
        }

        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_output_from_Console_WriteLine()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var result = executor.Execute("[Console]::WriteLine('written to the console')", null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "written to the console");
        }

        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_output_from_Console_Error_WriteLine()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var result = executor.Execute("[Console]::Error.WriteLine('written to console error')", null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "written to console error");
        }

        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_output_from_native_executable()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var result = executor.Execute("cmd /c echo Written by native executable", null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Written by native executable");
        }

        [TestMethod]
        public void LocalPowerShellScriptExecutor_should_return_output_from_CLR_executable()
        {
            var executor = new LocalPowerShellScriptExecutor();
            var MSBuildPath = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "msbuild.exe");
            var command = string.Format("& \"{0}\" /version", MSBuildPath);
            var result = executor.Execute(command, null);

            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Build Engine");
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void LocalPowerShellScriptExecutor_verify_that_a_script_can_change_the_working_directory_of_future_scripts()
        {
            var workingDirectory = Environment.CurrentDirectory;
            var testPath = Path.Combine(Path.GetTempPath(), GetType().Name);
            Directory.CreateDirectory(testPath);

            var setExecutor = new LocalPowerShellScriptExecutor();
            setExecutor.Execute(string.Format("[Environment]::CurrentDirectory = '{0}'", testPath), null);

            var getExecutor = new LocalPowerShellScriptExecutor();

            try
            {
                var result = getExecutor.Execute("[Environment]::CurrentDirectory", null);

                Assert.IsFalse(result.HasErrors, "HasErrors");
                StringAssert.Contains(result.Output, testPath);
            }
            finally
            {
                Environment.CurrentDirectory = workingDirectory;
                Directory.Delete(testPath);
            }
        }


    }
}
