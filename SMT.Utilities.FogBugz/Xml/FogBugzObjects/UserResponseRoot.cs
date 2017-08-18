using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class UserResponseRoot
    {
        [XmlElement("people")]
        public UserCollection UserList;
    }

    [XmlRoot("people")]
    public class UserCollection
    {
        [XmlElement("person")]
        public User[] Users;
    }
}
