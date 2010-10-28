using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TfsDeployer.Resources;
using TfsDeployer;
using TfsDeployer.DeployAgent;

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
                    Tfs2008BuildDetail = new StubBuildDetail()
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
                var buildInformation = new BuildInformation(new StubBuildDetail());

                var testDeployData = new DeployAgentData
                {
                    NewQuality = "Released",
                    OriginalQuality = null,
                    DeployScriptFile = scriptFile.FileInfo.Name,
                    DeployScriptRoot = scriptFile.FileInfo.DirectoryName,
                    DeployScriptParameters = new List<DeployScriptParameter>(),
                    Tfs2005BuildData = buildInformation.Data,
                    Tfs2008BuildDetail = buildInformation.Detail
                };

                var agent = new LocalPowerShellDeployAgent();

                // Act
                result = agent.Deploy(testDeployData);
            }

            // Assert
            Assert.IsFalse(result.HasErrors, "Test script failed.");
        }

    }
}
