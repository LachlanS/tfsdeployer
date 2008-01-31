using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace Tests.TfsDeployer.DeployerTests
{

    [TestClass]
    public class IsInterestedStatusChangeTests
    {

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenNullInConfigAndNullInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping()
            {
                Computer = Environment.MachineName,
                NewQuality = TestNewQuality,
                PermittedUsers = null,
                OriginalQuality = null
            };

            var statusChange = new Change()
            {
                NewValue = TestNewQuality,
                OldValue = null
            };
            
            var deployer = new Deployer();
            var result = deployer.IsInterestedStatusChange(changeEvent, mapping, statusChange);

            Assert.IsTrue(result, "IsInterestedStatusChange()");
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenEmptyInConfigAndEmptyInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping()
            {
                Computer = Environment.MachineName,
                NewQuality = TestNewQuality,
                PermittedUsers = null,
                OriginalQuality = string.Empty 
            };

            var statusChange = new Change()
            {
                NewValue = TestNewQuality,
                OldValue = string.Empty 
            };

            var deployer = new Deployer();
            var result = deployer.IsInterestedStatusChange(changeEvent, mapping, statusChange);

            Assert.IsTrue(result, "IsInterestedStatusChange()");
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenEmptyInConfigButNullInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping()
            {
                Computer = Environment.MachineName,
                NewQuality = TestNewQuality,
                PermittedUsers = null,
                OriginalQuality = string.Empty
            };

            var statusChange = new Change()
            {
                NewValue = TestNewQuality,
                OldValue = null
            };

            var deployer = new Deployer();
            var result = deployer.IsInterestedStatusChange(changeEvent, mapping, statusChange);

            Assert.IsTrue(result, "IsInterestedStatusChange()");
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenNullInConfigButEmptyInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping()
            {
                Computer = Environment.MachineName,
                NewQuality = TestNewQuality,
                PermittedUsers = null,
                OriginalQuality = null
            };

            var statusChange = new Change()
            {
                NewValue = TestNewQuality,
                OldValue = string.Empty 
            };

            var deployer = new Deployer();
            var result = deployer.IsInterestedStatusChange(changeEvent, mapping, statusChange);

            Assert.IsTrue(result, "IsInterestedStatusChange()");
        }

        [TestMethod]
        public void ShouldMatchNewQualityWhenEmptyInConfigButNullInTfs()
        {
            const string TestOriginalQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping()
            {
                Computer = Environment.MachineName,
                NewQuality = string.Empty,
                PermittedUsers = null,
                OriginalQuality = TestOriginalQuality
            };

            var statusChange = new Change()
            {
                NewValue = null,
                OldValue = TestOriginalQuality
            };

            var deployer = new Deployer();
            var result = deployer.IsInterestedStatusChange(changeEvent, mapping, statusChange);

            Assert.IsTrue(result, "IsInterestedStatusChange()");
        }

    }
}
