using Microsoft.VisualStudio.TestTools.UnitTesting;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer.MappingEvaluatorTests
{
    public class MappingEvaluatorTests
    {

        [TestClass]
        public class WhenBuildQualityHasNotChangedAndAllCriteriaMatch
        {
            IMappingEvaluator _evaluator;
            Mapping _mapping;
            BuildStatusChangeEvent _changeEvent;

            [TestInitialize]
            public void Setup()
            {
                _evaluator = new MappingEvaluator();
                _mapping = new MappingBuilder().Mapping;
                _changeEvent = new BuildStatusChangeEventBuilder
                 {
                     OriginalQuality = "BoringQuality",
                     NewQuality = "BoringQuality",
                 }
                .BuildStatusChangeEvent;
            }

            [TestMethod]
            public void TheMappingShouldNotMatch()
            {
                Assert.IsFalse(_evaluator.DoesMappingApply(_mapping, _changeEvent, "Succeeded"));
            }

        }

        [TestClass]
        public class WhenBuildQualityHasChangedAndAllCriteriaMatch
        {
            IMappingEvaluator _evaluator;
            Mapping _mapping;
            BuildStatusChangeEvent _changeEvent;

            [TestInitialize]
            public void Setup()
            {
                _evaluator = new MappingEvaluator();
                _mapping = new MappingBuilder().Mapping;
                _changeEvent = new BuildStatusChangeEventBuilder().BuildStatusChangeEvent;
            }

            [TestMethod]
            public void TheMappingShouldMatch()
            {
                Assert.IsTrue(_evaluator.DoesMappingApply(_mapping, _changeEvent, "Succeeded"));
            }
        }
    }
}
