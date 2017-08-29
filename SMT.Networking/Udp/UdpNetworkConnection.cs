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
    public class UdpNetworkConnection<T> : IUdpNetworkConnection<T>
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

        //send if targetted
        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        //unbind port, stop threads, kill event lists
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

        //bind to local port
        public bool StartListening(int port)
        {
            throw new NotImplementedException();
        }

        //unbind local port
        public void StopListening()
        {
            throw new NotImplementedException();
        }

        //direct output towards endpoint
        public void Target(string hostname, int port)
        {
            throw new NotImplementedException();
        }

        //direct output towards endpoint
        public void Target(string connectionString)
        {
            throw new NotImplementedException();
        }

        //direct output towards endpoint
        public void Target(IPEndPoint remoteEndpoint)
        {
            throw new NotImplementedException();
        }

        //stop listening to incoming messages, unbind port
        public void Untarget()
        {
            throw new NotImplementedException();
        }
    }
}
