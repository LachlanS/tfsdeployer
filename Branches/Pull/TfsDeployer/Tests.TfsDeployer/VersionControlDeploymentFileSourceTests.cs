using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using TfsDeployer.TeamFoundation;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class VersionControlDeploymentFileSourceTests
    {
        [TestCategory("Integration")]
        [TestMethod]
        public void VersionControlDeploymentFileSource_should_return_null_if_file_does_not_exist()
        {
            // Arrange
            var buildDetail = new BuildDetail {BuildDefinition = {Process = {ServerPath = "$/TfsDeployer/path_does_not_exist/build.xaml"}}};
            var tfsCollection = new TfsTeamProjectCollection(new Uri("https://tfs.codeplex.com/tfs/tfs06"));
            var versionControlServer = tfsCollection.GetService<VersionControlServer>();
            var source = new VersionControlDeploymentFileSource(versionControlServer);

            // Act
            var stream = source.DownloadDeploymentFile(buildDetail);

            // Assert
            Assert.IsNull(stream);
        }
    }
}
