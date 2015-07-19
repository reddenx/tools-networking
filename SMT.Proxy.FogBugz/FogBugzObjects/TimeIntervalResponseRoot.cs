using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class TimeIntervalResponseRoot
    {
        [XmlElement("intervals")]
        public TimeIntervalCollection IntervalList;
    }

    [XmlRoot("intervals")]
    public class TimeIntervalCollection
    {
        [XmlElement("interval")]
        public TimeInterval[] Intervals;
    }
}
