using System;
using System.Xml.Serialization;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    [Serializable]
    public class BuildStatusChangeEvent
    {
        [XmlElement(DataType = "anyURI")]
        public string BuildUri { get; set; }

        public string ChangedBy { get; set; }
        public string ChangedTime { get; set; }
        public string Id  { get; set; }
        public Change StatusChange  { get; set; }
        public string Subscriber { get; set; }

        [XmlElement(DataType = "anyURI")]
        public string TeamFoundationServerUrl  { get; set; }

        public string TeamProject { get; set; }
        public string TimeZone  { get; set; }
        public string TimeZoneOffset  { get; set; }
        public string Title { get; set; }

        [XmlElement(DataType = "anyURI")]
        public string Url { get; set; }
    }


}
