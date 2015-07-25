using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    public interface IUser
    {
        int UserId { get; }
        string Name { get; }
        string Email { get; }
        string Phone { get; }
        bool IsAdmin { get; }
        bool Removed { get; }
    }

    [XmlRoot("")]
    public class User : IUser
    {
        [XmlElement("ixPerson")]
        public int UserId { get; set; }
        
        [XmlElement("sFullName")]
        public string Name { get; set; }

        [XmlElement("sEmail")]
        public string Email { get; set; }

        [XmlElement("sPhone")]
        public string Phone { get; set; }

        [XmlElement("fAdministrator")]
        public bool IsAdmin { get; set; }

        [XmlElement("fDeleted")]
        public bool Removed { get; set; }
    }
}
