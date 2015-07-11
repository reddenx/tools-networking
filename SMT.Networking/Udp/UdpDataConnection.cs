using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Udp
{
    public class UdpDataConnection<TMessageType>
    {
        public event EventHandler<TMessageType> MessageReceived;

        private UdpClient ListenClient;
        private int ListenPort;
        private Thread ListenThread;

        private UdpClient SendClient;
        private string SendHostname;
        private int SendPort;

        private Func<TMessageType, byte[]> ToBytes;
        private Func<byte[], TMessageType> FromBytes;

        public UdpDataConnection(Func<TMessageType, byte[]> toBytes, Func<byte[], TMessageType> fromBytes)
        {
            ListenClient = null;
            SendClient = null;

            ToBytes = toBytes;
            FromBytes = fromBytes;

            ListenThread = null;
        }

        public void DirectOutput(string hostname, int port)
        {
            if (SendClient != null)
            {
                CleanupSender();
            }

            SendHostname = hostname;
            SendPort = port;
            SendClient = new UdpClient();
            SendClient.Connect(SendHostname, SendPort);
        }

        public void SendMessage(TMessageType message)
        {
            if (SendClient != null)
            {
                var messageBytes = ToBytes(message);
                SendClient.Send(messageBytes, messageBytes.Length);
            }
        }

        public void Listen(int port)
        {
            if (ListenClient != null)
            {
                CleanupListener();
            }

            ListenPort = port;
            ListenClient = new UdpClient(ListenPort);

            ListenThread = new Thread(new ThreadStart(ListenLoop));
            ListenThread.IsBackground = true;
            ListenThread.Start();
        }

        private void ListenLoop()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, ListenPort);

            try
            {
                while (true)
                {
                    var messageBytes = ListenClient.Receive(ref endPoint);
                    var message = FromBytes(messageBytes);

                    OnMessageReceived(message);
                }
            }
            catch (ThreadAbortException) { }
        }

        public void OnMessageReceived(TMessageType message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, message);
            }
        }

        private void CleanupSender()
        {
            try
            {
                SendClient.Close();
            }
            catch { }

            SendClient = null;
        }

        private void CleanupListener()
        {
            try
            {
                ListenThread.Abort();
            }
            catch { }

            try
            {
                ListenClient.Close();
            }
            catch { }
        }

        public void Dispose()
        {
            if (SendClient != null)
            {
                CleanupSender();
            }
            if (ListenClient != null)
            {
                CleanupListener();
            }
        }
    }
}
