using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Proxy.FogBugz;

namespace App.TestingGrounds
{
    public static class FogBugzProxyTesting
    {
        public static void Start()
        {
            var config = new FogBugzConfiguration();
            var repo = new FogBugzRepository(config.SecurityToken, config.ApiBaseUrl);

            var card = repo.GetCaseById(13367);

            Console.WriteLine();
        }
    }
}
