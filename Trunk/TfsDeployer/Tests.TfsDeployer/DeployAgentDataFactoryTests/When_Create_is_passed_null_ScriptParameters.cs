using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeployAgentDataFactoryTests
{
    [TestClass]
    public class When_Create_is_passed_null_ScriptParameters : DeployAgentDataFactoryContext
    {
        private DeployAgentData CreateDeployAgentData()
        {
            var mapping = CreateMapping();
            mapping.ScriptParameters = null;
            var buildInfo = CreateBuildInformation();
            var factory = new DeployAgentDataFactory();
            return factory.Create(DeployScriptRoot, mapping, buildInfo);
        }

        [TestMethod]
        public void Should_have_empty_DeployScriptParameters()
        {
            var data = CreateDeployAgentData();
            Assert.IsNotNull(data.DeployScriptParameters);
            Assert.AreEqual(0, data.DeployScriptParameters.Count());
        }
    }
}