using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer.TeamFoundation
{
    [TestClass]
    public class PropertyAdapterTests
    {
        [TestMethod]
        public void PropertyAdapter_should_copy_BuildDetail_BuildDefinition_Name()
        {
            // Arrange
            var tfsBuildDetail = new StubBuildDetail();
            tfsBuildDetail.BuildDefinition.Name = "foo";

            var buildDetail = new BuildDetail();

            // Act
            PropertyAdapter.CopyProperties(typeof(IBuildDetail), tfsBuildDetail, typeof(BuildDetail), buildDetail);

            // Assert
            Assert.AreEqual("foo", buildDetail.BuildDefinition.Name);
        }

        [TestMethod]
        public void PropertyAdapter_should_copy_BuildDetail_Status()
        {
            // Arrange
            var tfsBuildDetail = new StubBuildDetail();
            tfsBuildDetail.Status = Microsoft.TeamFoundation.Build.Client.BuildStatus.PartiallySucceeded;

            var buildDetail = new BuildDetail();

            // Act
            PropertyAdapter.CopyProperties(typeof(IBuildDetail), tfsBuildDetail, typeof(BuildDetail), buildDetail);

            // Assert
            Assert.AreEqual(global::TfsDeployer.TeamFoundation.BuildStatus.PartiallySucceeded, buildDetail.Status);
        }
    }
}
