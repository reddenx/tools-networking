using SMT.Networking;
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
using SMT.Networking.Udp;

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
            RunUdp();
            return;

            var networkFactory = new NetworkConnectionFactory();
            var listener = networkFactory.GetTcpNetworkConnectionListener<string>(new AsciiSerializer());
            listener.Start(37123);

            var connectionA = networkFactory.GetTcpNetworkConnection<string>(new AsciiSerializer());
            connectionA.OnConnected += (o, ip) => { Console.WriteLine("ACON: " + ip.ToString()); };
            connectionA.OnDisconnected += (o, e) => { Console.WriteLine("ADIS:"); };
            connectionA.OnError += (o, e) => { Console.WriteLine("AERR: " + e.ToString()); };
            connectionA.OnMessageReceived += (o, m) => { Console.WriteLine("AREC: " + m); };
            connectionA.OnMessageSent += (o, m) => { Console.WriteLine("ASEN: " + m); };

            ITcpNetworkConnection<string> connectionB = null;


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

            Thread.Sleep(800);

            connectionA.Send("hello");
            connectionB.Send("hi :)");
            connectionA.Send("goodbye");
            connectionB.Send("buh bye");

            Thread.Sleep(800);

            connectionA.Dispose();
            connectionB.Dispose();
            listener.Stop();

            Thread.Sleep(800);

            Console.WriteLine("test complete...");
            Console.ReadLine();
        }

        private static Thread DoCheapAsync(Action asyncAction)
        {
            var thread = new Thread(new ThreadStart(asyncAction));
            thread.IsBackground = true;
            thread.Start();

            return thread;
        }


        public static void RunUdp()
        {
            var udpA = new UdpNetworkConnection<string>(new AsciiSerializer());
            var udpB = new UdpNetworkConnection<string>(new AsciiSerializer());

            udpA.OnError += (sender, exception) => Console.WriteLine("AERR: " + exception.Message);
            udpB.OnError += (sender, exception) => Console.WriteLine("BERR: " + exception.Message);

            udpA.OnMessageReceived += (sender, s) => Console.WriteLine("AREC: " + s);
            udpB.OnMessageReceived += (sender, s) => Console.WriteLine("BREC: " + s);

            udpA.OnMessageSent += (sender, s) => Console.WriteLine("ASEN: " + s);
            udpB.OnMessageSent += (sender, s) => Console.WriteLine("BSEN: " + s);

            udpA.Target("127.0.0.1", 37123);
            udpA.Send("miss");

            Thread.Sleep(200);

            udpB.StartListening(37123);
            udpA.Send("hit");

            Thread.Sleep(200);

            udpB.Target("127.0.0.1", 37123);
            udpB.Send("echo");

            Thread.Sleep(200);
            
            udpA.Dispose();
            udpB.Dispose();

            Console.ReadLine();
        }

    }
}
