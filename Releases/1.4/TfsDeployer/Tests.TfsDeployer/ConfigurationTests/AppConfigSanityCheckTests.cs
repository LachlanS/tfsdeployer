using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer.Properties;

namespace Tests.TfsDeployer.ConfigurationTests
{
    public class AppConfigSanityCheckTests
    {
        /// <summary>
        /// These checks are here to allow for local testing against various different TFS instances, but
        /// to prevent the accidental checking-in of development-only changes.
        /// </summary>
        [TestClass]
        public class When_checking_in_code
        {
            [TestMethod]
            public void The_team_project_uri_should_be_an_example_one()
            {
                Assert.AreEqual("https://MyTfs.MyDomain/tfs/MyProjectCollection/", Settings.Default.TeamProjectCollectionUri);
            }

            [TestMethod]
            public void The_base_address_should_be_an_example_one()
            {
                Assert.AreEqual("http://MyHostname/Temporary_Listen_Addresses/TfsDeployer", Settings.Default.BaseAddress);
            }

            public void The_credentials_should_be_empty()
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(Settings.Default.TfsDomain));
                Assert.IsTrue(string.IsNullOrWhiteSpace(Settings.Default.TfsUserName));
                Assert.IsTrue(string.IsNullOrWhiteSpace(Settings.Default.TfsPassword));
            }
        }

    }
}
