using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class MilestoneResponseRoot
    {
        [XmlElement("fixfors")]
        public MilestoneCollection MilestoneList;
    }

    [XmlRoot("fixfors")]
    public class MilestoneCollection
    {
        [XmlElement("fixfor")]
        public Milestone[] Milestones;
    }
}
