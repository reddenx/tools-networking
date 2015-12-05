using SMT.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Udp
{
    public class UdpNetworkConnection<T> : INetworkConnection<T>
    {
        public event EventHandler<T> OnMessageReceived;
        public event EventHandler<IPEndPoint> OnConnected;
        public event EventHandler<T> OnMessageSent;
        public event EventHandler<Exception> OnError;
        public event EventHandler OnDisconnected;

        public bool Connected { get; private set; }
        public string HostName { get; private set; }
        public int Port { get; private set; }

        private readonly INetworkConnectionSerializer<T> Serializer;

        public UdpNetworkConnection(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;
        }

        //stop listening to incoming messages, unbind port
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        //bind port, start listening to messages
        public void Connect(string hostname, int port)
        {
            throw new NotImplementedException();
        }

        //bind port, start listening to messages
        public void Connect(string connectionString)
        {
            throw new NotImplementedException();
        }

        //queue message
        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        //disconnect, stop threads, kill event lists
        public void Dispose()
        {
            throw new NotImplementedException();
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
