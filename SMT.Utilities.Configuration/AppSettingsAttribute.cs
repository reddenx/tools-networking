using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Configuration
{
    public class AppSettingsAttribute : System.Attribute
    {
        public readonly string Name;

        public AppSettingsAttribute(string name)
        {
            Name = name;
        }
    }
}
