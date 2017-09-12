//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using SMT.Networking.Interfaces.SimpleMessaging;
//using SMT.Networking.Tcp;

//namespace App.TestingGrounds
//{
//    class TcpSelfConnectionLifecycleTests
//    {
//        public static void RunTest(Action<string> report)
//        {
//            report("Started");
            
//            var host = new SimpleTcpMessageHost<TestMessage>();
//            ISimpleMessenger<TestMessage> inboundClient = null;
//            host.OnClientConnect += (sender, client) =>
//            {
//                inboundClient = client;
//                inboundClient.OnMessageReceived += (clientSender, message) =>
//                    {
//                        report("Recv " + message.Message);
//                    };
//            };
//            host.StartHosting(37020);
//            report("Hosting");

//            var outboundClient = new SimpleTcpMessenger<TestMessage>();
//            outboundClient.Connect("127.0.0.1", 37020);

//            while (inboundClient == null)
//            {
//                report("waiting on local connection...");
//                Thread.Sleep(1000);
//            }

//            var message1 = new TestMessage("first message");
//            report("Send " + message1.Message);
//            outboundClient.Send(message1);

//            inboundClient.Disconnect();
//            outboundClient.Disconnect();
//            host.StopHosting();
//        }

//        [Serializable]
//        private class TestMessage
//        {
//            private static int MessageCounter;
//            public readonly string Message;

//            public TestMessage(string message)
//            {
//                Message = MessageCounter++ + ":" + message;
//            }
//        }
//    }
//}
