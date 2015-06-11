using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SMT.Networking.Udp;
using System.Net;
using System.Net.Sockets;

namespace App.PhoneRemoteBase.Models
{
    /// <summary>
    /// this is the udp broadcaster object
    /// it has two states, on and off
    /// while on, sends a message to the router broadcast looking for a phone
    /// while off, it does nothing
    /// </summary>
    class Broadcaster
    {
        private readonly SimpleUdpMessageSender<string> NetworkSender;
        private readonly string BroadcastMessage = "[BROADCAST:{0}]";
        private readonly int Port;
        private Thread BroadcastThread;
        private string LocalHostname;

        public Broadcaster(int port)
        {
            NetworkSender = new SimpleUdpMessageSender<string>();
            BroadcastThread = null;
            LocalHostname = GetLocalHostname();
            Port = port;
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
                    var message = string.Format(BroadcastMessage, LocalHostname);
                    NetworkSender.SendMessage(IPAddress.Broadcast.ToString(), Port, message);
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            { }
        }

        private string GetLocalHostname()
        {
            var name = Dns.GetHostName();
            var hosts = Dns.GetHostEntry(name);

            return hosts.AddressList
                .First(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
        }
    }
}
