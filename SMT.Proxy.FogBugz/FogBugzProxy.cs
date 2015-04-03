using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Proxy.FogBugz
{
    public class FogBugzProxy
    {
        private readonly string SecurityToken;

        private FogBugzProxy(string securityToken)
        {
            SecurityToken = securityToken;
        }

        public static IFogBugzProxy Get()
        {

        }
    }
}
