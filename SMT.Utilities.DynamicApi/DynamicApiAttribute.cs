using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.DynamicApi
{
    public class DynamicApiAttribute : Attribute
    {
        public readonly string RouteName;

        public DynamicApiAttribute(string route)
        {
            this.RouteName = route;
        }
    }
}
