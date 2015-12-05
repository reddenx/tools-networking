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
    public class TcpNetworkConnectionListener<T> : INetworkConnectionListener<T>
    {
        public event EventHandler<INetworkConnection<T>> OnClientConnected;
        public event EventHandler<Exception> OnError;

        private Thread ListenThread;
        private TcpListener Listener;

        private readonly INetworkConnectionSerializer<T> Serializer;

        public TcpNetworkConnectionListener(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;
        }

        public void Start(int port)
        {
            if (Listener == null)
            {
                Listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                Listener.Start();

                CleanUpCheapThread(ListenThread);
                ListenThread = DoCheapAsync(ListenLoop);
            }
        }

        public void Stop()
        {
            if (Listener != null)
            {
                Listener.Stop();
                Listener = null;

                CleanUpCheapThread(ListenThread);
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
                    if (OnError != null) OnError(this, e);
                }
            }
        }

        private Thread DoCheapAsync(Action asyncAction)
        {
            var thread = new Thread(new ThreadStart(asyncAction));
            thread.IsBackground = true;
            thread.Start();

            return thread;
        }

        private void CleanUpCheapThread(Thread cleanup)
        {
            if (cleanup != null)
            {
                if (cleanup.IsAlive)
                {
                    if (!cleanup.Join(100))
                    {
                        cleanup.Abort();
                    }
                }
                cleanup = null;
            }
        }
    }
}
