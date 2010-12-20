using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using Tests.TfsDeployer.Resources;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.DeployAgent
{
    [TestClass]
    public class LocalPowerShellDeployAgentTests
    {
        [TestMethod]
        public void LocalPowerShellDeployAgent_should_unload_assemblies_loaded_by_scripts()
        {
            // Arrange
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", Resource.AsString("LoadSystemWebAssemblyScript.ps1")))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "Test script failed.");

            var systemWebAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "System.Web").SingleOrDefault();
            Assert.IsNull(systemWebAssembly, "Assembly was not unloaded.");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_marshal_build_detail_across_AppDomains()
        {
            // Arrange
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", "$TfsDeployerBuildDetail | Format-List"))
            {
                var buildDetail = new BuildDetail();

                var mapping = new Mapping
                                  {
                                      NewQuality = "Released",
                                      OriginalQuality = null,
                                      ScriptParameters = new ScriptParameter[0],
                                      Script = scriptFile.FileInfo.Name
                                  };

                var buildStatusChangeEvent = new BuildStatusChangeEvent { StatusChange = new Change() };

                var testDeployData = (new DeployAgentDataFactory()).Create(scriptFile.FileInfo.DirectoryName, mapping, buildDetail, buildStatusChangeEvent);

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);
            }

            // Assert
            Assert.IsFalse(result.HasErrors, "Test script failed.");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_return_output_prior_to_a_script_error()
        {
            // Arrange
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", "'Output this first'\nthrow 'fail'"))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsTrue(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Output this first");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_expose_build_process_server_path_to_scripts()
        {
            // Arrange
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", @"$TfsDeployerBuildDetail.BuildDefinition.Process.ServerPath"))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail { BuildDefinition = { Process = { ServerPath = "$/foo.xaml"}}}
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "$/foo.xaml");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_support_spaces_in_script_path()
        {
            // Arrange
            DeployAgentResult result;
            const string subDirectory = "white space";
            using (var scriptFile = new TemporaryFile(".ps1", @"'Script path is ' + $MyInvocation.MyCommand.Path", subDirectory))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Script path is", "Output prefix");
            StringAssert.Contains(result.Output, subDirectory, "Output subdirectory");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_support_apostrophes_in_script_path()
        {
            // Arrange
            DeployAgentResult result;
            const string subDirectory = "isn't";
            using (var scriptFile = new TemporaryFile(".ps1", @"'Script path is ' + $MyInvocation.MyCommand.Path", subDirectory))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "HasErrors");
            StringAssert.Contains(result.Output, "Script path is", "Output prefix");
            StringAssert.Contains(result.Output, subDirectory, "Output subdirectory");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_support_dollars_in_script_path()
        {
            // Arrange
            DeployAgentResult result;
            const string subDirectory = "not$home$null";
            using (var scriptFile = new TemporaryFile(".ps1", @"'Script path is ' + $MyInvocation.MyCommand.Path", subDirectory))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "HasErrors: {0}", result.Output);
            StringAssert.Contains(result.Output, "Script path is", "Output prefix");
            StringAssert.Contains(result.Output, subDirectory, "Output subdirectory");
        }

        [TestMethod]
        public void LocalPowerShellDeployAgent_should_support_backticks_in_script_path()
        {
            // Arrange
            DeployAgentResult result;
            const string subDirectory = "back`tick";
            using (var scriptFile = new TemporaryFile(".ps1", @"'Script path is ' + $MyInvocation.MyCommand.Path", subDirectory))
            {
                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    TfsBuildDetail = new BuildDetail()
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);

            }

            // Assert
            Assert.IsFalse(result.HasErrors, "HasErrors: {0}", result.Output);
            StringAssert.Contains(result.Output, "Script path is", "Output prefix");
            StringAssert.Contains(result.Output, subDirectory, "Output subdirectory");
        }

    }
}
