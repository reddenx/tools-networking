using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Utilities.Configuration;

namespace SMT.Proxy.FogBugz
{
    public class FogBugzConfiguration : ConfigurationBase
    {
        [AppSettings("FogBugzSecurityToken")]
        public readonly string SecurityToken;

        [AppSettings("FobugzApiUrl")]
        public readonly string ApiBaseUrl;
    }
}
