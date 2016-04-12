using SMT.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

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

        private UdpClient Client;

        private readonly INetworkConnectionSerializer<T> Serializer;
        private readonly Queue<T> OutBox;
        private readonly Queue<T> Inbox;

        public UdpNetworkConnection(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;
        }

        //stop listening to incoming messages, unbind port
        public void Disconnect()
        {
            //get client back to null by unbinding and throwing away any in process threads
            throw new NotImplementedException();
        }

        //bind port, start listening to messages
        public void Connect(string hostname, int port)
        {
            //cleanup current client if it's not null
            throw new NotImplementedException();
        }

        //bind port, start listening to messages
        public void Connect(string connectionString)
        {
            //parse connection string and call connect
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

        //run once connected, cleanup once disconnected
        private void SendLoop()
        {
            try
            {
                while (true)
                {
                    if (OutBox.Count > 0)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch (ThreadAbortException aborted) {} //expected abort procedure given blocking call
            finally
            {
                //cleanup
            }
        }

        private Thread GetBackgroundThread(Action asyncAction)
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
