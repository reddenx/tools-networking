using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Configuration
{
    public class ConnectionStringAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public ConnectionStringAttribute(string name)
        {
            Name = name;
        }
    }
}
