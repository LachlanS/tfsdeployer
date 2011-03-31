using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.DeployAgent;

namespace Tests.TfsDeployer.DeployAgentDataFactoryTests
{
    [TestClass]
    public class When_Create_is_passed_all_source_data : DeployAgentDataFactoryContext
    {
        private DeployAgentData CreateDeployAgentData()
        {
            var mapping = CreateMapping();
            var buildInfo = CreateBuildDetail();
            var buildStatusChangeEvent = new BuildStatusChangeEvent{StatusChange = new Change()};
            var factory = new DeployAgentDataFactory();
            return factory.Create(DeployScriptRoot, mapping, buildInfo, buildStatusChangeEvent);
        }

        [TestMethod]
        public void Should_have_a_DeployScriptRoot()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual(@"c:\deploy_script_root\", data.DeployScriptRoot);
        }

        [TestMethod]
        public void Should_have_a_DeployServer()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual("deploy_server", data.DeployServer);
        }

        [TestMethod]
        public void Should_have_a_DeployScriptFile()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual("deploy_script_file", data.DeployScriptFile);
        }

        [TestMethod]
        public void Should_have_TfsBuildDetail()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual("test_build_number", data.TfsBuildDetail.BuildNumber);
        }

        [TestMethod]
        public void Should_have_first_DeployScriptParameter()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual("first_parameter_name", data.DeployScriptParameters.First().Name, "Name");
            Assert.AreEqual("first_parameter_value", data.DeployScriptParameters.First().Value, "Value");
        }

        [TestMethod]
        public void Should_have_second_DeployScriptParameter()
        {
            var data = CreateDeployAgentData();
            Assert.AreEqual("second_parameter_name", data.DeployScriptParameters.Skip(1).First().Name, "Name");
            Assert.AreEqual("second_parameter_value", data.DeployScriptParameters.Skip(1).First().Value, "Value");
        }



    }
}

