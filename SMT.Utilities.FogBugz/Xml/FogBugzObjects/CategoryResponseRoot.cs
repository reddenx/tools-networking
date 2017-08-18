using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class CategoryResponseRoot
    {
        [XmlElement("categories")]
        public CategoryCollection CategoryList { get; set; }
    }

    [XmlRoot("categories")]
    public class CategoryCollection
    {
        [XmlElement("category")]
        public Category[] Categories { get; set; }
    }
}
