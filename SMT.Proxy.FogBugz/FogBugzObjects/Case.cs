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
        public int Id;
        
        [XmlElement("ixBugParent")]
        public int? Parent;

        [XmlElement("ixBugChildren")]
        public string _children;
        public int[] Children { get { return _children.Split(',').Select(c => int.Parse(c)).ToArray(); } }

        [XmlElement("tags")]
        public Tags _tags;
        public string[] Tags { get { return _tags.Tag; } }


    }
}
