using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    public interface IArea
    {
        int AreaId { get; }
        string Name { get; }
        int ProjectId { get; }
        string DefaultUser { get; }
        string User { get; }
        AreaTypes AreaType { get; }
        int DocCount { get; }
    }

    [XmlRootAttribute("area")]
    public class Area : IArea
    {
        [XmlElement("ixArea")]
        public int AreaId { get; set; }

        [XmlElement("sArea")]
        public string Name { get; set; }

        [XmlElement("ixProject")]
        public int ProjectId { get; set; }

        [XmlElement("ixPersonOwner")]
        public string DefaultUser { get; set; }

        [XmlElement("sPersonOwner")]
        public string User { get; set; }

        [XmlElement("nType")]
        public int _typeId;
        public AreaTypes AreaType { get { return (AreaTypes)_typeId; } }

        [XmlElement("cDoc")]
        public int DocCount { get; set; }
    }

    public enum AreaTypes
    {
        Normal = 0,
        NotSpam = 1,
        Undecided = 2,
        Spam = 3
    }
}
