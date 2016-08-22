using SMT.Utilities.DynamicApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    [DynamicApi("SomeTest")]
    public interface IManager
    {
        float GetRandom(float low, float high);
    }

    public static class DynamicApiTesting
    {
        public static void Run()
        {
            var client = new SMT.Utilities.DynamicApi.DynamicApiClient<IManager>("http://localhost:56983/", "derp");
            for (int i = 1; i < 100; ++i)
            {
                Console.WriteLine(client.Call(c => c.GetRandom(0, i)));
                Thread.Sleep(100);
            }

            Console.ReadLine();
        }
    }
}
