using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SMT.Networking.NetworkConnection
{
    public interface ITcpNetworkConnectionListener<T> : IDisposable
    {
        event EventHandler<ITcpNetworkConnection<T>> OnClientConnected;

        void Start(int port);
        void Stop();
    }

    /// <summary>
    /// listens for incoming tcp connections and produces network connection clients
    /// </summary>
    /// <typeparam name="T">type of message to expect</typeparam>
    public class TcpTcpNetworkConnectionListener<T> : ITcpNetworkConnectionListener<T>
    {
        public event EventHandler<ITcpNetworkConnection<T>> OnClientConnected;
        public event EventHandler<Exception> OnError;

        private Thread ListenThread;
        private TcpListener Listener;

        private readonly INetworkConnectionSerializer<T> Serializer;

        /// <summary>
        /// </summary>
        /// <param name="serializer">serializer to be used in produced tcp clients</param>
        public TcpTcpNetworkConnectionListener(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;
        }

        /// <summary>
        /// starts listening to the specified port for incoming tcp connections, this binds the port to this listener
        /// </summary>
        /// <param name="port">port to listen for tcp connections</param>
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

        /// <summary>
        /// stops the listener from accepting incoming clients, this frees the bound port
        /// </summary>
        public void Stop()
        {
            if (Listener != null)
            {
                Listener.Stop();
                Listener = null;

                ListenThread.DisposeOfThread();
            }
        }

        /// <summary>
        /// stops the listener, cleans up threading and clears event subscribers
        /// </summary>
        public void Dispose()
        {
            Stop();
            OnClientConnected.RemoveAllListeners();
            OnError.RemoveAllListeners();
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
                        OnClientConnected.SafeExecuteAsync(this, connection);
                    }
                    else //no handler, close it
                    {
                        client.Close();
                    }
                }
                catch(Exception e)
                {
                    OnError.SafeExecuteAsync(this, e);
                }
            }
        }
    }
}
