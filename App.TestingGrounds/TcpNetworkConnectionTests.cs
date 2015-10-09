using SMT.Networking.Interfaces;
using SMT.Networking.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    static class TcpNetworkConnectionTests
    {
        public static void Run()
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 37123));
            listener.Start();

            INetworkConnection<string> connectionA = new TcpNetworkConnection<string>(ASCIIEncoding.ASCII.GetString, ASCIIEncoding.ASCII.GetBytes);
            connectionA.OnConnected += (o, ip) => { Console.WriteLine("ACON: " + ip.ToString()); };
            connectionA.OnDisconnected += (o, e) => { Console.WriteLine("ADIS:"); };
            connectionA.OnError += (o, e) => { Console.WriteLine("AERR: " + e.ToString()); };
            connectionA.OnMessageReceived += (o, m) => { Console.WriteLine("AREC: " + m); };
            connectionA.OnMessageSent += (o, m) => { Console.WriteLine("ASEN: " + m); };

            INetworkConnection<string> connectionB = null;

            DoCheapAsync(() =>
            {
                connectionB = new TcpNetworkConnection<string>(listener.AcceptTcpClient(), ASCIIEncoding.ASCII.GetString, ASCIIEncoding.ASCII.GetBytes);
                connectionB.OnConnected += (o, ip) => { Console.WriteLine("BCON: " + ip.ToString()); };
                connectionB.OnDisconnected += (o, e) => { Console.WriteLine("BDIS:"); };
                connectionB.OnError += (o, e) => { Console.WriteLine("BERR: " + e.ToString()); };
                connectionB.OnMessageReceived += (o, m) => { Console.WriteLine("BREC: " + m); };
                connectionB.OnMessageSent += (o, m) => { Console.WriteLine("BSEN: " + m); };

                listener.Stop();
            });

            connectionA.Connect("127.0.0.1:37123");

            Console.ReadLine();

            connectionA.Send("hello");
            connectionB.Send("hi :)");
            connectionA.Send("goodbye");
            connectionB.Send("buh bye");

            Console.ReadLine();


            connectionA.Dispose();
        }


        private static Thread DoCheapAsync(Action asyncAction)
        {
            var thread = new Thread(new ThreadStart(asyncAction));
            thread.IsBackground = true;
            thread.Start();

            return thread;
        }

    }
}
