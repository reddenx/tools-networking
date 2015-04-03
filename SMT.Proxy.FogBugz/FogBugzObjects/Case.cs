using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    [XmlRootAttribute("case")]
    public class Case
    {
        [XmlElement("ixBug")]
        public int CaseId;
        [XmlElement("ixBugParent")]
        public int? Parent;
        [XmlElement("ixBugChildren")]
        public string _children;
        public int[] Children { get { return _children.Split(',').Select(c => int.Parse(c)).ToArray(); } }

        [XmlElement("tags")]
        public Tags _tags;
        public string[] Tags { get { return _tags.Tag; } }

        [XmlElement("fOpen")]
        public bool IsOpen;
        [XmlElement("sTitle")]
        public string Title;
        [XmlElement("ixProject")]
        public int ProjectId;
        [XmlElement("sProject")]
        public string ProjectName;
        [XmlElement("ixStatus")]
        public int StatusId;
        [XmlElement("ixFixFor")]
        public int MilestoneId;
        [XmlElement("dtOpened")]
        public DateTime DateOpened;
        [XmlElement("dtResolved")]
        public DateTime DateResolved;
        [XmlElement("dtClosed")]
        public DateTime DateClosed;

        //TODO-SM get story points
    }
}
