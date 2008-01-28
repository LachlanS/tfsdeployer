using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;

namespace Tests.TfsDeployer.DeployerTests
{

    [TestClass]
    public class GetBuildUriFromArtifactUrlTests
    {

        [TestMethod]
        public void ShouldTranslateSampleCorrectly()
        {
            const string TestArtifactUrl = @"http://tfs:8080/Build/Build.aspx?artifactMoniker=839";
            const string ExpectedResult = @"vstfs:///Build/Build/839";

            var result = Deployer.GetBuildUriFromArtifactUrl(TestArtifactUrl);
            Assert.AreEqual(ExpectedResult, result.ToString(), true);
        }
    }
}
