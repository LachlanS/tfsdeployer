using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class BatchFileDeployAgentTests
    {
        [TestMethod]
        public void AgentShouldKillScriptsThatExceedTimeoutConstraints()
        {
            // Arrange
            var agent = new BatchFileDeployAgent();
            const string deployScriptFilename = "SlowDeployment.bat";
            var deployScriptDirectory = Directory.GetCurrentDirectory();

            File.WriteAllText(deployScriptFilename, @"ping -n 100 127.0.0.1");    // script that just takes about nn seconds to execute

            // required because the deploy agent builds its own parameter list
            var tfsBuildDetail = new BuildDetail
            {
                DropLocation = string.Empty,
                BuildNumber = string.Empty,
            };

            var deployAgentData = new DeployAgentData
            {
                DeployScriptRoot = deployScriptDirectory,
                DeployScriptFile = deployScriptFilename,
                Timeout = TimeSpan.FromSeconds(5),
                TfsBuildDetail = tfsBuildDetail,
                DeployScriptParameters = Enumerable.Empty<DeployScriptParameter>().ToList(),
            };

            // Act
            var deployAgentResult = agent.Deploy(deployAgentData);

            // Assert
            Assert.IsTrue(deployAgentResult.Output.Contains(@"Pinging 127.0.0.1"));
            Assert.IsFalse(deployAgentResult.Output.Contains(@"Ping statistics"));

            // Clean up
            File.Delete(deployScriptFilename);
        }

        [TestMethod]
        public void AgentShouldNotCauseScriptsWithMassiveOutputToStall()
        {
            // Arrange
            const string massiveOutputScript = "@echo off\nfor /L %%i in (1000, -1, 0) do echo %%i green bottles, hanging on the wall";

            var agent = new BatchFileDeployAgent();
            const string deployScriptFilename = "MassiveOutputDeployment.bat";
            var deployScriptDirectory = Directory.GetCurrentDirectory();
            File.WriteAllText(deployScriptFilename, massiveOutputScript);

            // required because the deploy agent builds its own parameter list
            var tfsBuildDetail = new BuildDetail
            {
                DropLocation = string.Empty,
                BuildNumber = string.Empty,
            };

            var deployAgentData = new DeployAgentData
            {
                DeployScriptRoot = deployScriptDirectory,
                DeployScriptFile = deployScriptFilename,
                Timeout = TimeSpan.FromSeconds(5),
                TfsBuildDetail = tfsBuildDetail,
                DeployScriptParameters = Enumerable.Empty<DeployScriptParameter>().ToList(),
            };

            // Act
            var deployAgentResult = agent.Deploy(deployAgentData);

            // Assert
            Assert.IsTrue(deployAgentResult.Output.Contains(@"0 green bottles, hanging on the wall"));

            // Clean up
            File.Delete(deployScriptFilename);
        }
    }
}
