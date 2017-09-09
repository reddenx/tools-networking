//using SMT.Networking.Interfaces.SimpleMessaging;
//using SMT.Networking.Udp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace App.TestingGrounds
//{
//    internal class UdpConsoleFeedbackListener
//    {
//        private readonly ISimpleMessenger<string> Messenger;

//        public UdpConsoleFeedbackListener()
//        {
//            Messenger = new SimpleUdpMessenger<string>();
//            Messenger.OnMessageReceived += Messenger_OnMessageReceived;
//            Messenger.Connect(null, 37019);
//        }

//        void Messenger_OnMessageReceived(object sender, string e)
//        {
//            Console.WriteLine(e);
//        }
//    }
//}
