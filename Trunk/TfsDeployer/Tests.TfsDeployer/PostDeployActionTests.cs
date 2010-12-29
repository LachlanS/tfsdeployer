using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TfsDeployer;
using TfsDeployer.Alert;
using TfsDeployer.Configuration;
using TfsDeployer.DeployAgent;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class PostDeployActionTests
    {
        [TestMethod]
        public void PostDeployAction_should_set_KeepForever_and_save_build_when_RetainBuild_is_specified()
        {
            // Arrange
            var alert = MockRepository.GenerateStub<IAlert>();
            var buildServer = MockRepository.GenerateStub<IBuildServer>();

            var buildDetail = new BuildDetail();
            
            var tfsBuildDetail = new StubBuildDetail();
            ((IBuildDetail)tfsBuildDetail).KeepForever = false;
            buildServer.Stub(o => o.GetBuild(null, null, null, QueryOptions.None))
                .IgnoreArguments()
                .Return(tfsBuildDetail);

            var mapping = new Mapping { RetainBuildSpecified = true, RetainBuild = true };

            var result = new DeployAgentResult {HasErrors = false, Output = string.Empty};

            var postDeployAction = new PostDeployAction(buildDetail, tfsBuildDetail, alert);

            // Act
            postDeployAction.DeploymentFinished(mapping, result);

            // Assert
            Assert.AreEqual(true, ((IBuildDetail)tfsBuildDetail).KeepForever, "KeepForever");
            Assert.AreEqual(1, tfsBuildDetail.SaveCount, "Save()");
            
        }
    }
}