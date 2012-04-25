using System;
using System.IO;
using System.Text.RegularExpressions;
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

        [TestMethod]
        public void PowerShellAgentRunner_should_return_output_from_Console_WriteLine()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "[Console]::WriteLine('written to the console')"
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.MaxValue, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "written to the console");
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_output_from_Console_Error_WriteLine()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "[Console]::Error.WriteLine('written to the console error')"
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "written to the console error");
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_output_from_native_executable()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = "cmd /c echo Written by native executable"
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "Written by native executable");
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_output_from_CLR_executable()
        {
            var msBuildPath = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "msbuild.exe");

            var request = new AgentRequest
            {
                NoProfile = true,
                Command = string.Format("& \"{0}\" /version", msBuildPath)
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "Build Engine");
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_output_prior_to_a_script_error()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = string.Format("'Output this first'\nthrow 'fail'")
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "Output this first");
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_readable_errors_instead_of_CliXml()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = string.Format("'Output this first'\nthrow 'my error message'")
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, "my error message");
            StringAssert.DoesNotMatch(runner.Output, new Regex("CLIXML"));
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_return_formatted_objects()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = string.Format("Get-ChildItem -Path Env:")
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.Zero, ClrVersion.Version2);
            runner.Run();

            StringAssert.Contains(runner.Output, Environment.GetEnvironmentVariable("TEMP"));
        }

        [TestMethod]
        public void PowerShellAgentRunner_should_terminate_if_timeout_period_exceeded()
        {
            var request = new AgentRequest
            {
                NoProfile = true,
                Command = string.Format("'Begin'; Start-Sleep -Seconds 10; 'End'")
            };

            var runner = new PowerShellAgentRunner(request, Path.GetTempPath(), TimeSpan.FromSeconds(2), ClrVersion.Version2);
            var exitCode = runner.Run();

            StringAssert.Contains(runner.Output, "Begin");
            StringAssert.DoesNotMatch(runner.Output, new Regex("End"));
            Assert.AreNotEqual(0, exitCode);
        }


    }
}