using System;
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

    }
}
