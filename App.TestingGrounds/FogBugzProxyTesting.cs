using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SMT.Utilities.FogBugz;

namespace App.TestingGrounds
{
    public static class FogBugzProxyTesting
    {
        public static void Start()
        {
            var repo = new FogBugzRepository("", "");

            //var cards = repo.SearchCases("23339", 100);
            //var intervals = repo.GetIntervalsForCase(23339);
            //var intervals2 = repo.GetIntervalsForCase(23339, 56);
            //var intervals3 = repo.GetIntervalsForDates(DateTime.Now - TimeSpan.FromDays(30), DateTime.Now);
            //var intervals4 = repo.GetIntervalsForDates(DateTime.Now - TimeSpan.FromDays(30), DateTime.Now, 56);
            //var milestones = repo.GetAllMilestones();

            //var jsonSerializer = new JavaScriptSerializer();
            //Console.WriteLine(jsonSerializer.Serialize(intervals));

            Console.ReadLine();
        }
    }
}
