using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
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
            var buildInfo = CreateBuildDetail();
            var buildStatusChangeEvent = new BuildStatusChangeEvent { StatusChange = new Change() };
            var factory = new DeployAgentDataFactory();
            return factory.Create(DeployScriptRoot, mapping, buildInfo, buildStatusChangeEvent);
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