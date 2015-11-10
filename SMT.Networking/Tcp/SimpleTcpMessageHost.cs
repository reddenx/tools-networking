using SMT.Networking.Interfaces;
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
    [Obsolete("Use TcpNetworkConnection")]
    public class SimpleTcpMessageHost<TMessage> : ISimpleMessageHost<TMessage>
        where TMessage : class
    {
        public event EventHandler<ISimpleMessenger<TMessage>> OnClientConnect;

        private Thread ListenThread;
        private TcpListener Listener;
        private bool IsListening;

        public SimpleTcpMessageHost()
        {
            IsListening = false;
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

            if (ListenThread.IsAlive)//really kill it if it's not dead
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
            catch { }//probably a better way to do this, gotta catch that broken block break exception
        }

        private void HandleClientConnection(TcpClient client)
        {
            if (OnClientConnect != null)
            {
                OnClientConnect(this, new SimpleTcpMessenger<TMessage>(client));
            }
        }
    }
}
