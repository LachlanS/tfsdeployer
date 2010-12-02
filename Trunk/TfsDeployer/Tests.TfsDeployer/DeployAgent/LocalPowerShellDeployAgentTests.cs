using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void LocalPowerShellDeployAgent_should_marshal_build_data_across_AppDomains()
        {
            // Arrange
            DeployAgentResult result;
            using (var scriptFile = new TemporaryFile(".ps1", "$TfsDeployerBuildData | Format-List"))
            {
                var buildDetail = new BuildDetail();

                var mapping = new Mapping
                                  {
                                      NewQuality = "Released",
                                      OriginalQuality = null,
                                      ScriptParameters = new ScriptParameter[0],
                                      Script = scriptFile.FileInfo.Name
                                  };

                var testDeployData = (new DeployAgentDataFactory()).Create(scriptFile.FileInfo.DirectoryName, mapping, buildDetail);

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

    }
}
