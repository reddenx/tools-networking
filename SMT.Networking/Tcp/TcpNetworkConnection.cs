using SMT.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    internal class TcpNetworkConnection<T> : ITcpNetworkConnection<T>
    {
        public event EventHandler<T> OnMessageReceived;
        public event EventHandler<IPEndPoint> OnConnected;
        public event EventHandler<T> OnMessageSent;
        public event EventHandler<Exception> OnError;
        public event EventHandler OnDisconnected;

        public bool Connected
        {
            get { return Client != null && Client.Connected; }
        }
        public string HostName
        {
            get { return Endpoint == null ? null : Endpoint.Address.ToString(); }
        }
        public int Port
        {
            get { return Endpoint == null ? -1 : Endpoint.Port; }
        }

        private readonly INetworkConnectionSerializer<T> Serializer;

        private IPEndPoint Endpoint;
        private TcpClient Client;
        private Thread SendThread;
        private Thread ReceiveThread;

        private readonly Queue<T> Outbox;

        public TcpNetworkConnection(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;

            this.Outbox = new Queue<T>();
        }

        public TcpNetworkConnection(TcpClient client, INetworkConnectionSerializer<T> serializer)
            : this(serializer)
        {
            this.Client = client;
            StartThreads();
        }

        public void Dispose()
        {
            Disconnect();

            new Thread(() =>
            {
                while (Connected) { Thread.Sleep(10); }

                OnMessageReceived.RemoveAllListeners();
                OnConnected.RemoveAllListeners();
                OnMessageSent.RemoveAllListeners();
                OnError.RemoveAllListeners();
                OnDisconnected.RemoveAllListeners();
            }).StartBackground();
        }

        public void Disconnect()
        {
            new Thread(() =>
            {
                CleanupThreads();
                CleanupClient();
            }).StartBackground();
        }

        public void Connect(string hostname, int port)
        {
            var ips = Dns.GetHostAddresses(hostname);
            if (ips.Length == 0)
                throw new ArgumentException("Unable to resolve hostname");

            Connect(new IPEndPoint(ips[0], port));
        }

        public void Connect(string connectionString)
        {
            var pieces = connectionString?.Split(':');
            if (pieces?.Length != 2)
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            int port = -1;
            if (!int.TryParse(pieces[1], out port))
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            Connect(pieces[0], port);
        }

        public void Connect(IPEndPoint endpoint)
        {
            new Thread(() =>
            {
                CleanupThreads();
                CleanupClient();

                if (StartClient(endpoint))
                {
                    StartThreads();

                    OnConnected.SafeExecute(this, Endpoint);
                }
                else
                {
                    CleanupThreads();
                    CleanupClient();
                }
            }).StartBackground();
        }

        public void Queue(T message)
        {
            Outbox.Enqueue(message);
        }

        public void Send(T message)
        {
            Outbox.Enqueue(message);
            Send();
        }

        public void Send()
        {
            if (SendThread == null || !SendThread.IsAlive)
            {
                SendThread = new Thread(SendLoop).StartBackground();
            }
        }

        private void ReceiveLoop()
        {
            try
            {
                var instream = Client.GetStream();
                byte[] sizeBuffer = new byte[4];

                while (Connected)
                {
                    int bytesRead = instream.Read(sizeBuffer, 0, 4);
                    if (bytesRead != 4)
                    {
                        CleanupClient();
                        return; //stream has been closed;
                    }

                    int messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    if (messageSize > 0)
                    {
                        byte[] messageBuffer = new byte[messageSize];//this could thrash memory in a realtime environment :(
                        int totalReadBytes = 0;
                        do
                        {
                            int messageBytesRead = instream.Read(messageBuffer, totalReadBytes, messageSize - totalReadBytes);
                            if (messageBytesRead <= 0)
                            {
                                CleanupClient();
                                return; //stream has been closed
                            }

                            totalReadBytes += messageBytesRead;
                        }
                        while (totalReadBytes < messageSize);//handling partial messages

                        var message = Serializer.Deserialize(messageBuffer);

                        OnMessageReceived.SafeExecute(this, message);
                    }
                    else
                    {
                        throw new IOException("message was improperly formatted");
                    }
                }
            }
            catch (IOException e)
            {
                CleanupClient();
                OnError.SafeExecute(this, e);
            }
            catch (ThreadAbortException) { }//expecting these on abort
        }

        private void SendLoop()
        {
            try
            {
                var outStream = Client.GetStream();

                while (Outbox.Count > 0)
                {
                    var message = Outbox.Dequeue();

                    byte[] buffer = null;
                    try
                    {
                        buffer = Serializer.Serialize(message);
                    }
                    catch (Exception e)
                    {
                        OnError.SafeExecute(this, e);
                    }

                    if (buffer != null && Connected)
                    {
                        var sizeBytes = BitConverter.GetBytes((Int32)buffer.Length);//length of 4
                        buffer = sizeBytes.Concat(buffer).ToArray();

                        outStream.Write(buffer, 0, buffer.Length);

                        OnMessageSent.SafeExecute(this, message);
                    }
                }
            }
            catch (IOException e)
            {
                CleanupClient();
                OnError.SafeExecute(this, e);
            }
            catch (ThreadAbortException) { }
        }

        private void CleanupClient()
        {
            if (Client != null)
            {
                OnDisconnected.SafeExecute(this);

                if (Client.Connected)
                {
                    Client.Close();
                }
                Client = null;
            }
        }

        private void CleanupThreads()
        {
            SendThread.DisposeOfThread();
            ReceiveThread.DisposeOfThread();
        }

        private bool StartClient(IPEndPoint endpoint)
        {
            Client = new TcpClient
            {
                NoDelay = true,
            };

            try
            {
                Client.Connect(endpoint);
                this.Endpoint = endpoint;
                return true;
            }
            catch (SocketException e)
            {
                OnError.SafeExecute(this, e);
                return false;
            }
        }

        private void StartThreads()
        {
            if (SendThread != null || ReceiveThread != null)
                CleanupThreads();

            ReceiveThread = new Thread(ReceiveLoop).StartBackground();
        }
    }
}
