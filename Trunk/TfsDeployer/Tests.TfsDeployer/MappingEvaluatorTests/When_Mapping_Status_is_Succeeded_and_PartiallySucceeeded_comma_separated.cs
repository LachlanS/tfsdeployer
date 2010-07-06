using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer.MappingEvaluatorTests
{
    [TestClass]
    public class When_Mapping_Status_is_Succeeded_and_PartiallySucceeeded_comma_separated
    {
        private Mapping SimpleMapping
        {
            get
            {
                return new Mapping
                {
                    Computer = Environment.MachineName,
                    NewQuality = "Same",
                    PermittedUsers = null,
                    OriginalQuality = null,
                    Status = "Succeeded,PartiallySucceeded"
                };
            }
        }

        private BuildStatusChangeEvent SimpleChangeEvent
        {
            get
            {
                var changeEvent = new BuildStatusChangeEvent();
                changeEvent.StatusChange = new Change
                {
                    NewValue = "Same",
                    OldValue = null
                };
                return changeEvent;
            }
        }

        private bool DoesMappingApply(string buildStatus)
        {
            var mappingEvaluator = new MappingEvaluator();
            return mappingEvaluator.DoesMappingApply(SimpleMapping, SimpleChangeEvent, buildStatus);
        }

        [TestMethod]
        public void Should_match_Succeeded_build_status()
        {
            Assert.IsTrue(DoesMappingApply(BuildStatus.Succeeded.ToString()));
        }

        [TestMethod]
        public void Should_match_PartiallySucceeded_build_status()
        {
            Assert.IsTrue(DoesMappingApply(BuildStatus.PartiallySucceeded.ToString()));
        }

        [TestMethod]
        public void Should_not_match_Failed_build_status()
        {
            Assert.IsFalse(DoesMappingApply(BuildStatus.Failed.ToString()));
        }

    }
}