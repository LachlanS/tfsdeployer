using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using Readify.Useful.TeamFoundation.Common.Notification;

namespace Tests.TfsDeployer.MappingEvaluatorTests
{
    [TestClass]
    public class DoesMappingApplyTests
    {
        [TestMethod]
        public void ShouldMatchOriginalQualityWhenNullInConfigAndNullInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = TestNewQuality,
                                  PermittedUsers = null,
                                  OriginalQuality = null
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = TestNewQuality,
                                               OldValue = null
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenEmptyInConfigAndEmptyInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = TestNewQuality,
                                  PermittedUsers = null,
                                  OriginalQuality = string.Empty
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = TestNewQuality,
                                               OldValue = string.Empty
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenEmptyInConfigButNullInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = TestNewQuality,
                                  PermittedUsers = null,
                                  OriginalQuality = string.Empty
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = TestNewQuality,
                                               OldValue = null
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityWhenNullInConfigButEmptyInTfs()
        {
            const string TestNewQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = TestNewQuality,
                                  PermittedUsers = null,
                                  OriginalQuality = null
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = TestNewQuality,
                                               OldValue = string.Empty
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMatchNewQualityWhenEmptyInConfigButNullInTfs()
        {
            const string TestOriginalQuality = "Pass";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = string.Empty,
                                  PermittedUsers = null,
                                  OriginalQuality = TestOriginalQuality
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = null,
                                               OldValue = TestOriginalQuality
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);


            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMatchOriginalQualityOnWildcard()
        {
            const string TestWildcard = "*";

            var changeEvent = new BuildStatusChangeEvent();

            var mapping = new Mapping
                              {
                                  Computer = Environment.MachineName,
                                  NewQuality = "SomeNewQuality",
                                  PermittedUsers = null,
                                  OriginalQuality = TestWildcard
                              };

            changeEvent.StatusChange = new Change
                                           {
                                               NewValue = "SomeNewQuality",
                                               OldValue = "DefinitelyNotTheWildCard"
                                           };

            var mappingEvaluator = new MappingEvaluator();
            var result = mappingEvaluator.DoesMappingApply(mapping, changeEvent);

            Assert.IsTrue(result);
        }
    }
}