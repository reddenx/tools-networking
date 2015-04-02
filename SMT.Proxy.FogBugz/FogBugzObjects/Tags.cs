using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    [XmlRoot("tags")]
    public class Tags
    {
        [XmlElement("tag")]
        public string[] Tag;
    }
}
