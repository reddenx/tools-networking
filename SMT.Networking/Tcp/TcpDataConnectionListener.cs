using SMT.Networking.Interfaces.SimpleMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    public class TcpDataConnectionListener<TMessageType>
        where TMessageType : class
    {
        public event EventHandler<TcpDataConnection<TMessageType>> OnClientConnect;

        private Thread ListenThread;
        private TcpListener Listener;
        private bool IsListening;

        private Func<byte[], TMessageType> FromBytes;
        private Func<TMessageType, byte[]> ToBytes;

        public TcpDataConnectionListener(Func<byte[], TMessageType> fromBytes, Func<TMessageType, byte[]> toBytes)
        {
            IsListening = false;

            this.FromBytes = fromBytes;
            this.ToBytes = toBytes;
        }

        public bool StartHosting(int port)
        {
            if (!IsListening)
            {
                Listener = new TcpListener(IPAddress.Any, port);
                Listener.Start();
                StartListenLoop();
                IsListening = true;
                return true;
            }

            return false;
        }

        public void StopHosting()
        {
            Listener.Stop();
            Listener = null;

            if (ListenThread.IsAlive)
            {
                ListenThread.Abort();
            }

            IsListening = false;
        }

        private void StartListenLoop()
        {
            ListenThread = new Thread(new ThreadStart(ListenLoop));
            ListenThread.IsBackground = true;
            ListenThread.Start();
        }

        private void ListenLoop()
        {
            try
            {
                while (IsListening && Listener != null)
                {
                    HandleClientConnection(Listener.AcceptTcpClient());
                }
            }
            catch
            { }
        }

        private void HandleClientConnection(TcpClient client)
        {
            if (OnClientConnect != null)
            {
                OnClientConnect(this, new TcpDataConnection<TMessageType>(client, FromBytes, ToBytes));
            }
        }
    }
}
