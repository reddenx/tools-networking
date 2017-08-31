using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SMT.Networking.Interfaces;

namespace SMT.Networking.Tcp
{
    //TODO implement disposable and clean up channels and threads
    public class TcpTcpNetworkConnectionListener<T> : ITcpNetworkConnectionListener<T>
    {
        public event EventHandler<ITcpNetworkConnection<T>> OnClientConnected;
        public event EventHandler<Exception> OnError;

        private Thread ListenThread;
        private TcpListener Listener;

        private readonly INetworkConnectionSerializer<T> Serializer;

        public TcpTcpNetworkConnectionListener(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;
        }

        public void Start(int port)
        {
            if (Listener == null)
            {
                Listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                Listener.Start();

                ListenThread.DisposeOfThread();
                ListenThread = new Thread(ListenLoop).StartBackground();
            }
        }

        public void Stop()
        {
            if (Listener != null)
            {
                Listener.Stop();
                Listener = null;

                ListenThread.DisposeOfThread();
            }
        }

        private void ListenLoop()
        {
            while (Listener != null)
            {
                try
                {
                    var client = Listener.AcceptTcpClient();

                    if (OnClientConnected != null)
                    {
                        var connection = new TcpNetworkConnection<T>(client, Serializer);
                        OnClientConnected(this, connection);
                    }
                    else //no handler, close it
                    {
                        client.Close();
                    }
                }
                catch(Exception e)
                {
                    OnError.SafeExecute(this, e);
                }
            }
        }
    }
}
