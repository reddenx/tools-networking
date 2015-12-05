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
        private class AsciiSerializer : INetworkConnectionSerializer<string>
        {
            public byte[] Serialize(string message) { return ASCIIEncoding.ASCII.GetBytes(message); }
            public string Deserialize(byte[] data) { return ASCIIEncoding.ASCII.GetString(data); }
        }

        public static void Run()
        {
            INetworkConnectionListener<string> listener = NetworkConnectionFactory.GetTcpNetworkConnectionListener<string>(new AsciiSerializer(), 2048);
            listener.Start(37123);

            INetworkConnection<string> connectionA = NetworkConnectionFactory.GetTcpNetworkConnection<string>(new AsciiSerializer(), 2048);
            connectionA.OnConnected += (o, ip) => { Console.WriteLine("ACON: " + ip.ToString()); };
            connectionA.OnDisconnected += (o, e) => { Console.WriteLine("ADIS:"); };
            connectionA.OnError += (o, e) => { Console.WriteLine("AERR: " + e.ToString()); };
            connectionA.OnMessageReceived += (o, m) => { Console.WriteLine("AREC: " + m); };
            connectionA.OnMessageSent += (o, m) => { Console.WriteLine("ASEN: " + m); };

            INetworkConnection<string> connectionB = null;


            DoCheapAsync(() =>
            {
                listener.OnClientConnected += (s, c) => { connectionB = c; };
                while (connectionB == null)
                {
                    Thread.Sleep(100);
                }
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
