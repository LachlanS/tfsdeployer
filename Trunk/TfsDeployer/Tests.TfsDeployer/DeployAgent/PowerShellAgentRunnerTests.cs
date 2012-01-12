using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;
using TfsDeployer.PowerShellAgent;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class PowerShellAgentRunnerTests
    {
        [TestMethod]
        public void PowerShellAgentRunner_should_support_CLR_4()
        {
            var request = new AgentRequest
                              {
                                  NoProfile = true,
                                  Command = "'CLR:' + $PSVersionTable.CLRVersion"
                              };

            var workingDirectory = Path.GetTempPath();

            var runner = new PowerShellAgentRunner(request, workingDirectory, TimeSpan.Zero, ClrVersion.Version4);
            runner.Run();

            StringAssert.Contains(runner.Output, "CLR:4.0");

        }

        [TestMethod]
        public void PowerShellAgentRunner_should_support_CLR_2()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "'CLR:' + $PSVersionTable.CLRVersion"
            };

            var workingDirectory = Path.GetTempPath();

            var runner = new PowerShellAgentRunner(request, workingDirectory, TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "CLR:2.0");

        }

        [TestMethod]
        public void PowerShellAgentRunner_should_support_PowerShell_v3()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "'PS:' + $PSVersionTable.PSVersion"
            };

            var workingDirectory = Path.GetTempPath();

            var runner = new PowerShellAgentRunner(request, workingDirectory, TimeSpan.Zero, ClrVersion.Version4);
            runner.Run();

            StringAssert.Contains(runner.Output, "PS:3.0");

        }

        [TestMethod]
        public void PowerShellAgentRunner_should_support_PowerShell_v2()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "'PS:' + $PSVersionTable.PSVersion"
            };

            var workingDirectory = Path.GetTempPath();

            var runner = new PowerShellAgentRunner(request, workingDirectory, TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "PS:2.0");

        }
    }
}