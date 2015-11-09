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
    public class TcpNetworkConnectionListener<T> : INetworkConnectionListener<T>
    {
        public event EventHandler<INetworkConnection<T>> OnClientConnected;

        private Thread ListenThread;
        private TcpListener Listener;

        private TcpNetworkConnection<T>.Serialize Serializer;
        private TcpNetworkConnection<T>.Deserialize Deserializer;

        public TcpNetworkConnectionListener(TcpNetworkConnection<T>.Serialize serializer, TcpNetworkConnection<T>.Deserialize deserializer)
        {
            Serializer = serializer;
            Deserializer = deserializer;
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
                        var connection = new TcpNetworkConnection<T>(client, Deserializer, Serializer);
                        OnClientConnected(this, connection);
                    }
                    else //no handler, close it
                    {
                        client.Close();
                    }
                }
                catch
                { 
                    //TODO more reporting
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
