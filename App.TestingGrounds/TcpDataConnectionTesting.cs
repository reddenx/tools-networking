//using SMT.Networking.Interfaces.SimpleMessaging;
//using SMT.Networking.Tcp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace App.TestingGrounds
//{
//    public static class TcpDataConnectionTesting
//    {
//        public static void Start()
//        {
//            int arbitraryPort = 37264;
//            var conn1 = new TcpDataConnection<string>(ASCIIEncoding.ASCII.GetString, ASCIIEncoding.ASCII.GetBytes);
//            TcpDataConnection<string> conn2 = null;

//            var connectionListener = new TcpDataConnectionListener<string>(ASCIIEncoding.ASCII.GetString, ASCIIEncoding.ASCII.GetBytes);
//            connectionListener.StartHosting(arbitraryPort);//arbitrary yaaay
//            connectionListener.OnClientConnect += (sender, client) => 
//            {
//                conn2 = client;
//                Console.WriteLine("conn2 received connection");
//            };

//            conn1.MessageReceived += ConsoleMessageHandler;
//            conn1.Connect("127.0.0.1", arbitraryPort);
//            //conn2 should be set

//            Thread.Sleep(500);//make sure it gets connected before continueing on
//            conn2.MessageReceived += ConsoleMessageHandler;

//            conn1.SendData("conn1 => conn2, hello");
//            conn2.SendData("conn2 => conn1, hi back");

//            Thread.Sleep(500);//make sure the message gets through before closing
//            conn1.Disconnect();
//            conn2.Disconnect();

//            //Should READ

//            //conn2 received connection
//            //conn1 => conn2, hello
//            //conn2 => conn1, hi back
//        }

//        public static void ConsoleMessageHandler(object sender, string message)
//        {
//            Console.WriteLine(message);
//        }
//    }
//}
