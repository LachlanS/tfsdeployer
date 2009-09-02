using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    [TestClass]
    public class ExecuteTests
    {

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
