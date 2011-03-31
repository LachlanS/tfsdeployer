using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
    /// <summary>
    /// Type which holds information about the Team Foundation Server which raised the event
    /// </summary>
    [GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "TeamFoundationServer")]
    public class TfsIdentity
    {
        private string _url;

        [XmlAttributeAttribute("url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}