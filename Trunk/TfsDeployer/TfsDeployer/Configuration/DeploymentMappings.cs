using System;
using System.Xml.Serialization;

namespace TfsDeployer.Configuration
{
    [Serializable]
    [XmlRoot(Namespace = "http://www.readify.net/TfsDeployer/DeploymentMappings20100214")]
    public class DeploymentMappings
    {
        [XmlElement("Mapping")]
        public Mapping[] Mappings { get; set; }
    }

    public class Mapping
    {
        [XmlElement("ScriptParameter")]
        public ScriptParameter[] ScriptParameters { get; set; }

        [XmlAttribute]
        public string BuildDefinitionPattern { get; set; }

        [XmlAttribute]
        public string Computer { get; set; }

        [XmlAttribute]
        public string OriginalQuality { get; set; }

        [XmlAttribute]
        public string NewQuality { get; set; }

        [XmlAttribute]
        public string Script { get; set; }

        [XmlAttribute]
        public string NotificationAddress { get; set; }

        [XmlAttribute]
        public string PermittedUsers { get; set; }
        
        [XmlAttribute]
        public RunnerType RunnerType { get; set; }

        [XmlIgnore]
        public bool RunnerTypeSpecified { get; set; }

        [XmlAttribute]
        public bool RetainBuild { get; set; }
        
        [XmlIgnore]
        public bool RetainBuildSpecified { get; set; }

        [XmlAttribute]
        public string Status { get; set; }
    }

    public class ScriptParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public enum RunnerType
    {
        PowerShell,
        BatchFile,
    }

}
