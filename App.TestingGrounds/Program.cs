using System;
using App.TestingGrounds;

namespace SMT.Utilities.Configuration
{
    public class Program
    {
        static void Main(string[] args)
        {
            var listener = new UdpConsoleFeedbackListener();
            Console.ReadLine();

            //TcpSelfConnectionLifecycleTests.RunTest(Console.WriteLine);
        }
    }
}