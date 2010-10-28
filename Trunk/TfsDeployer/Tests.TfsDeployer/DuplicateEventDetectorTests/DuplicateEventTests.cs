using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;
using TfsDeployer.Configuration;
using Readify.Useful.TeamFoundation.Common.Notification;
using System.Threading;

namespace Tests.TfsDeployer.MappingEvaluatorTests
{
    public class DuplicateEventTests
    {
        [TestClass]
        public class WhenThereHaveBeenNoPreceedingEvents
        {
            IDuplicateEventDetector _duplicateEventDetector;
            BuildStatusChangeEvent _event;

            [TestInitialize]
            public void Setup()
            {
                _duplicateEventDetector = new DuplicateEventDetector();

                _event = new BuildStatusChangeEventBuilder().BuildStatusChangeEvent;
            }

            public void The_first_event_should_be_unique()
            {
                bool isUnique = _duplicateEventDetector.IsUnique(_event);
                Assert.AreEqual(true, isUnique);
            }

            [TestMethod]
            public void Subsequent_events_within_the_timeout_period_should_not_be_unique()
            {
                // I know, I know.. multiple asserts in a single test. This one's a bit of a pain, though, as it
                // takes nn seconds for the duplicate detector to flush events from its event record, and I don't
                // want to introduce that sort of lag for multiple tests when one will do.  -andrewh 25/10/10
                bool isUnique;

                DateTime startTime = DateTime.Now;

                isUnique = _duplicateEventDetector.IsUnique(_event);
                Assert.AreEqual(true, isUnique);

                // one second before the timeout should expire...
                DateTime waitUntil = DateTime.Now.Add(_duplicateEventDetector.TimeoutPeriod).Subtract(TimeSpan.FromSeconds(1));

                while (DateTime.Now < waitUntil)
                {
                    isUnique = _duplicateEventDetector.IsUnique(_event);
                    Assert.AreEqual(false, isUnique);

                    Thread.Sleep(100);
                }

                // ensure _duplicateEventDetector.TimeoutPeriod seconds between
                Thread.Sleep(1 * 1000);

                isUnique = _duplicateEventDetector.IsUnique(_event);
                Assert.AreEqual(true, isUnique);
            }
        }

    }
}
