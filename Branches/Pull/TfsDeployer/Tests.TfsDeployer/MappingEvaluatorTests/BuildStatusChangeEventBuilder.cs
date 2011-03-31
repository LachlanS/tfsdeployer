using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;

namespace Tests.TfsDeployer.MappingEvaluatorTests
{
    /// <summary>
    /// Quickly builds a default BuildStatusChangeEvent and populates it with a set of default values.
    /// </summary>
    internal class BuildStatusChangeEventBuilder
    {
        readonly BuildStatusChangeEvent _event;

        public BuildStatusChangeEventBuilder()
        {
            _event = new BuildStatusChangeEvent
            {
                TeamFoundationServerUrl = "https://tfs.example.com/tfs",
                TeamProject = "Test Project",
                Title = "Test Project Build Test_Project_20101024.2 Quality Changed To NewStatus",
                Id = "Test_Project_20101024.2",
                Url = "http://tfs.example.com/Build/Build.aspx",
                TimeZoneOffset = "+10:00:00",
                ChangedTime = DateTime.Now.ToString(),
                ChangedBy = "EXAMPLE\\CaramelloKoala",
                StatusChange = new Change { OldValue = "OriginalQuality", NewValue = "NewQuality" },
            };
        }

        public string OriginalQuality { set { _event.StatusChange.OldValue = value; } }
        public string NewQuality { set { _event.StatusChange.NewValue = value; } }
        public DateTime ChangedTime { set { _event.ChangedTime = value.ToString(); } }
        public string TimeZoneOffset { set { _event.TimeZone = value; } }

        public BuildStatusChangeEvent BuildStatusChangeEvent { get { return _event; } }
    }
}
