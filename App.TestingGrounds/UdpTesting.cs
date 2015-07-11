using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    static class UdpTesting
    {
        public static void Start()
        {
            var sendClient = new SMT.Networking.Udp.UdpDataConnection<string>(ASCIIEncoding.ASCII.GetBytes, ASCIIEncoding.ASCII.GetString);
            sendClient.DirectOutput("127.0.0.1", 37095);

            var listenClient = new SMT.Networking.Udp.UdpDataConnection<string>(ASCIIEncoding.ASCII.GetBytes, ASCIIEncoding.ASCII.GetString);
            listenClient.Listen(37095);
            listenClient.MessageReceived += listenClient_MessageReceived;

            Console.WriteLine("Sending: 123456TEST");
            sendClient.SendMessage("123456TEST");

            Thread.Sleep(2000);

            sendClient.Dispose();
            listenClient.Dispose();
        }

        static void listenClient_MessageReceived(object sender, string e)
        {
            Console.WriteLine("message received: {0}", e);
        }
    }
}
