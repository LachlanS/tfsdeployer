using System;
using System.Xml.Serialization;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    /// <summary>
    /// Event raised when a Build Completes
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class BuildCompletionEvent
    {
        [XmlElementAttribute(DataType = "anyURI")]
        public string TeamFoundationServerUrl { get; set; }

        public string TeamProject { get; set; }
        public string Id { get; set; }

        [XmlElementAttribute(DataType = "anyURI")]
        public string Url { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        public string CompletionStatus { get; set; }
        public string Subscriber { get; set; }
        public string Configuration { get; set; }
        public string RequestedBy { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOffset { get; set; }
        public string BuildStartTime { get; set; }
        public string BuildCompleteTime { get; set; }
        public string BuildMachine { get; set; }
    }
}