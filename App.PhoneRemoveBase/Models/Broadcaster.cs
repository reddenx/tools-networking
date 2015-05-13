using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SMT.Networking.Udp;

namespace App.PhoneRemoveBase.Models
{
    /// <summary>
    /// this is the udp broadcaster object
    /// it has two states, on and off
    /// while on, sends a message to the router broadcast looking for a phone
    /// while off, it does nothing
    /// </summary>
    class Broadcaster
    {
        private const string BROADCAST_HOST = "255.255.255.255";
        private readonly SimpleUdpMessageSender<string> NetworkSender;
        private readonly string BroadcastMessage;
        private Thread BroadcastThread;

        public Broadcaster()
        {
            BroadcastThread = null;
            BroadcastMessage = "TEST12345";
        }

        public void Start()
        {
            if (BroadcastThread == null)
            {
                BroadcastThread = new Thread(new ThreadStart(BroadcastLoop));
                BroadcastThread.IsBackground = true;
                BroadcastThread.Start();
            }
        }

        public void Stop()
        {
            if (BroadcastThread != null)
            {
                BroadcastThread.Abort();
                BroadcastThread = null;
            }
        }

        private void BroadcastLoop()
        {
            try
            {
                while (true)
                {
                    NetworkSender.SendMessage(BROADCAST_HOST, 37015, BroadcastMessage);
                }
            }
            catch (ThreadAbortException)
            { }
        }
    }
}
