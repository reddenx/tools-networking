using SMT.Networking.Interfaces;
using SMT.Networking.Interfaces.SimpleMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    public class SimpleTcpMessenger<TMessage> : ISimpleMessenger<TMessage>
        where TMessage : class
    {
        public event EventHandler<TMessage> OnMessageReceived;

        private Thread SendThread;
        private TcpClient Client;
        private bool IsConnected;
        private BinaryFormatter Formatter;

        internal SimpleTcpMessenger(TcpClient client)
        {
            Client = client;
            IsConnected = true;
            Formatter = new BinaryFormatter();
            StartReceiveLoop();
        }

        public SimpleTcpMessenger()
        {
            IsConnected = false;
            Formatter = new BinaryFormatter();
        }

        public void Connect(string host, int port)
        {
            if (!IsConnected)
            {
                try
                {
                    Client = new TcpClient();
                    Client.Connect(host, port);

                    StartReceiveLoop();
                }
                catch
                {
                    IsConnected = false;
                    Client = null;
                }
            }
        }

        public void Send(TMessage message)
        {
            Formatter.Serialize(Client.GetStream(), message);
        }

        private void HandleMessageReceived(TMessage message)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived.Invoke(this, message);
            }
        }

        private void StartReceiveLoop()
        {
            SendThread = new Thread(new ThreadStart(ReceiveLoop));
            SendThread.IsBackground = true;
            SendThread.Start();
        }

        private void ReceiveLoop()
        {
            var netStream = Client.GetStream();
            try
            {
                while (IsConnected)
                {
                    var result = Formatter.Deserialize(netStream);
                    HandleMessageReceived(result as TMessage);
                }
            }
            catch
            {
                IsConnected = false;
            }
        }
    }
}
