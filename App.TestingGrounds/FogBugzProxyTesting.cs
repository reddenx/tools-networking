using System;
using System.Collections.Generic;
using System.IO;
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
            var secureTestData = File.ReadAllLines("C:/Dev/DataSources/FBTesting.txt");
            var values = secureTestData.ToDictionary<string, string, string>(str => str.Split('=')[0].Trim(), str => str.Split('=')[1].Trim());

            var repo = new FogBugzRepository(values["token"], values["baseAddress"]);

            Report(repo.SearchCases("23339", 100));
            Report(repo.GetIntervalsForCase(23339));
            Report(repo.GetIntervalsForCase(23339, 56));
            Report(repo.GetIntervalsForDates(DateTime.Now - TimeSpan.FromDays(30), DateTime.Now));
            Report(repo.GetIntervalsForDates(DateTime.Now - TimeSpan.FromDays(30), DateTime.Now, 56));
            Report(repo.GetAllMilestones());
            Report(repo.GetUsers());

            Console.ReadLine();
        }

        static void Report(object obj)
        {
            var ser = new JavaScriptSerializer();
            Console.WriteLine(ser.Serialize(obj));
        }
    }
}
