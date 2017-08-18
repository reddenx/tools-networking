using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    public interface ICategory
    {
        int CategoryId { get; }
        string Name { get; }
        string NamePlural { get; }
        int CaseDefaultStatusId { get; }
        bool IsScheduled { get; }
    }

    [XmlRoot("category")]
    public class Category : ICategory
    {
        [XmlElement("ixCategory")]
        public int CategoryId { get; set; }

        [XmlElement("sCategory")]
        public string Name { get; set; }

        [XmlElement("sPlural")]
        public string NamePlural { get; set; }

        [XmlElement("ixStatusDefault")]
        public int CaseDefaultStatusId { get; set; }

        [XmlElement("fIsScheduleItem")]
        public bool IsScheduled { get; set; }
    }
}
