using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace App.Phone.PhoneRemote.Models
{
    internal class UdpSimpleMessenger
    {
        private const int UDP_PORT = 37019;

        public event EventHandler<string> MessageReceived;

        private DatagramSocket SendClient;
        private DatagramSocket ListenClient;
        private DataWriter Writer;

        public UdpSimpleMessenger()
        { }

        public void StartListening()
        { }

        public void Connect(string host)
        { }

        public void SendMessage(string message)
        { }
    }
}
