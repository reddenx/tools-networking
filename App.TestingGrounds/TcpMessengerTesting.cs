//using SMT.Networking.Messages;
//using SMT.Networking.Tcp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace App.TestingGrounds
//{
//    class TcpMessengerTesting
//    {
//        public static void Run()
//        {
//            var tcpMessenger = new TcpMessenger("127.0.0.1", 37012);
//            var listener = new TcpListener(37012);
//            listener.Start();

//            Console.WriteLine("Connecting");

//            tcpMessenger.OnConnected += (o, e) =>
//            {
//                Console.WriteLine("B connected");
//            };

//            tcpMessenger.OnDisconnected += (o, e) =>
//            {
//                Console.WriteLine("B disconnected");
//            };

//            tcpMessenger.OnError += (o, e) =>
//            {
//                Console.WriteLine("B error: " + e);
//            };

//            tcpMessenger.OnMessageReceived += (o, e) =>
//            {
//                var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                Console.WriteLine("B received: " + messageStr);
//            };

//            tcpMessenger.OnMessageSent += (o, e) =>
//            {
//                var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                Console.WriteLine("B sent: " + messageStr);
//            };

//            TcpMessenger tcpIncoming = null;
//            var thread = new Thread(new ThreadStart(() =>
//            {
//                var client = listener.AcceptTcpClient();
//                listener.Stop();
//                tcpIncoming = new TcpMessenger(client);
//                Console.WriteLine("Connected");

//                tcpIncoming.OnConnected += (o, e) =>
//                {
//                    Console.WriteLine("A connected");
//                };

//                tcpIncoming.OnDisconnected += (o, e) =>
//                {
//                    Console.WriteLine("A disconnected");
//                };

//                tcpIncoming.OnError += (o, e) =>
//                {
//                    Console.WriteLine("A error: " + e);
//                };

//                tcpIncoming.OnMessageReceived += (o, e) =>
//                {
//                    var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                    Console.WriteLine("A received: " + messageStr);
//                };

//                tcpIncoming.OnMessageSent += (o, e) =>
//                {
//                    var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                    Console.WriteLine("A sent: " + messageStr);
//                };
//            }));
//            thread.IsBackground = true;
//            thread.Start();

//            Console.WriteLine("B trying to connect");
//            tcpMessenger.Connect();

//            Thread.Sleep(1000);
//            Console.WriteLine("Connection wait over");

//            if (tcpIncoming == null)
//            {
//                Console.WriteLine("Connection didn't get here in time");
//            }

//            tcpMessenger.Send(ASCIIEncoding.ASCII.GetBytes("hi!"));
//            tcpMessenger.Send(ASCIIEncoding.ASCII.GetBytes("i'm testing some stuff"));
//            tcpIncoming.Send(ASCIIEncoding.ASCII.GetBytes("hi back"));
//            tcpIncoming.Send(ASCIIEncoding.ASCII.GetBytes("hi more stuff"));

//            Thread.Sleep(1000);
//            tcpIncoming.Disconnect();
//            Thread.Sleep(1000);
//            tcpMessenger.Dispose();
//            tcpIncoming.Dispose();

//            Console.ReadLine();
//        }

//        public static void RunMessageBus()
//        {
//            var aConnection = new TcpMessenger("localhost", 37016);
//            SetupHandlers(aConnection, "A");
//            TcpMessenger bConnection = null;

//            var listener = new TcpListener(37016);
//            listener.Start();
//            DoAsync(() =>
//                {
//                    var client = listener.AcceptTcpClient();
//                    bConnection = new TcpMessenger(client);
//                    SetupHandlers(bConnection, "B");
//                });
//            aConnection.Connect();
//            Thread.Sleep(1000);

//            var busA = new MessageBus(aConnection);
//            var busB = new MessageBus(bConnection);

//            busA.RegisterPollQueue(1);
//            busA.RegisterPushQueue(2, (m) =>
//            {
//                var message = ASCIIEncoding.ASCII.GetString(m);
//                Console.WriteLine("busA recv push ch2: " + message);
//            });

//            busB.SendMessage(1, ASCIIEncoding.ASCII.GetBytes("testing one"));
//            busB.SendMessage(2, ASCIIEncoding.ASCII.GetBytes("testing two"));

//            Thread.Sleep(1000);

//            Console.ReadLine();
//        }

//        public static void SetupHandlers(TcpMessenger messenger, string prefix)
//        {
//            messenger.OnConnected += (o, e) =>
//            {
//                Console.WriteLine(prefix + " connected");
//            };

//            messenger.OnDisconnected += (o, e) =>
//            {
//                Console.WriteLine(prefix + " disconnected");
//            };

//            messenger.OnError += (o, e) =>
//            {
//                Console.WriteLine(prefix + " error: " + e);
//            };

//            messenger.OnMessageReceived += (o, e) =>
//            {
//                var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                Console.WriteLine(prefix + " received: " + messageStr);
//            };

//            messenger.OnMessageSent += (o, e) =>
//            {
//                var messageStr = ASCIIEncoding.ASCII.GetString(e);
//                Console.WriteLine(prefix + " sent: " + messageStr);
//            };
//        }

//        private static void DoAsync(Action thing)
//        {
//            var thread = new Thread(new ThreadStart(thing));
//            thread.IsBackground = true;
//            thread.Start();
//        }
//    }
//}
