using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.PowerShellRunnerTests
{
    [TestClass]
    public class When_overriding_system_ExecutionPolicy
    {

        [TestMethod]
        public void Should_successfully_deploy_a_script_from_the_Internet_zone()
        {
            var controlResult = DeployInternetZoneScript(PowerShellExecutionPolicyBehaviour.SystemExecutionPolicy);
            if (!controlResult.HasErrors)
            {
                Assert.Inconclusive("PowerShell ExecutionPolicy is Unrestricted on this system. This test cannot run.");
                // Automatically changing the policy for the test for would administrator privileges.
                // Tests should not require administrator privileges.
            }

            var testDeployResult = DeployInternetZoneScript(PowerShellExecutionPolicyBehaviour.Unrestricted);
            Assert.IsFalse(testDeployResult.HasErrors, "System ExecutionPolicy was not ignored.");
        }

        private DeployAgentResult DeployInternetZoneScript(PowerShellExecutionPolicyBehaviour executionPolicyBehaviour)
        {
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", PowerShellScripts.GetExecutionPolicyScript))
            {

                ApplyInternetZoneIdentifier(scriptFile.FileInfo.FullName);

                var testDeployData = new DeployAgentData
                                         {
                                             NewQuality = "Released",
                                             OriginalQuality = null,
                                             DeployScriptFile = scriptFile.FileInfo.FullName,
                                             DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                                             DeployScriptParameters = new List<DeployScriptParameter>(),
                                             Tfs2008BuildDetail = new StubBuildDetail()
                                         };

                var psAgent = new LocalPowerShellDeployAgent();
                psAgent.ExecutionPolicyBehaviour = executionPolicyBehaviour;

                result = psAgent.Deploy(testDeployData);
            }
            return result;
        }

        private void ApplyInternetZoneIdentifier(string fileName)
        {
            //TODO find a way to utilise the IZoneIdentifier interface from managed code
            // http://msdn.microsoft.com/en-us/library/ms537032(VS.85).aspx
            using (var applyScript = new TemporaryFile(".cmd", PowerShellScripts.ApplyInternetZoneIdentifier))
            {
                var processTimeout = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
                var startInfo = new ProcessStartInfo(applyScript.FileInfo.FullName, fileName);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                var process = Process.Start(startInfo);
                if (!process.WaitForExit(processTimeout))
                {
                    throw new TimeoutException("Process exceeded execution time limit.");
                }
                if (process.ExitCode != 0)
                {
                    throw new ApplicationException("Processed returned non-zero exit code.");
                }
            }
        }
    }
}