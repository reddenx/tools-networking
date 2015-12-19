using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class AreaResponseRoot
    {
        [XmlElement("areas")]
        public AreaCollection AreaList;
    }

    [XmlRoot("areas")]
    public class AreaCollection
    {
        [XmlElement("area")]
        public Area[] Areas;
    }
}
