using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    [XmlRootAttribute("response")]
    public class CaseResponseRoot
    {
        [XmlElement("cases")]
        public CaseCollection CaseList;
    }

    [XmlRootAttribute("cases")]
    public class CaseCollection
    {
        [XmlElement("case")]
        public Case[] Cases;
    }
}
