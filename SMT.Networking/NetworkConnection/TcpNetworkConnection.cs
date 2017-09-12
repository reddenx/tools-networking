using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SMT.Networking.NetworkConnection
{
    /// <summary>
    /// Tcp connection, turns synchronous calls into events
    /// </summary>
    /// <typeparam name="T">Message Type</typeparam>
    public interface ITcpNetworkConnection<T> : INetworkConnection<T>
    {
        /// <summary>
        /// fired when this client has successfully connected to its endpoint
        /// </summary>
        event EventHandler<IPEndPoint> OnConnected;
        /// <summary>
        /// fired when this client has been disconnected
        /// </summary>
        event EventHandler OnDisconnected;
        /// <summary>
        /// the connection status for this client
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// disconnects and cleans up threads
        /// </summary>
        void Disconnect();
        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="hostname">name of the endpoint, either an IP or DNS resolvable name</param>
        /// <param name="port">port number to communicate on</param>
        void Connect(string hostname, int port);
        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="connectionString">connection string must follow the format "www.oodlesofboodlesnoodles.com:9000" or "192.168.10.100:9000"</param>
        void Connect(string connectionString);
        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="endpoint">endpoint to attempt a connection</param>
        void Connect(IPEndPoint remoteEndpoint);
    }

    /// <summary>
    /// Tcp connection, turns synchronous calls into events
    /// </summary>
    /// <typeparam name="T">Message Type</typeparam>
    public class TcpNetworkConnection<T> : ITcpNetworkConnection<T>
    {
        /// <summary>
        /// fired when a message is received, called on a non-network thread
        /// </summary>
        public event EventHandler<T> OnMessageReceived;
        /// <summary>
        /// fired when this client has successfully connected to its endpoint
        /// </summary>
        public event EventHandler<IPEndPoint> OnConnected;
        /// <summary>
        /// fired when a message is successfully sent to its endpoint
        /// </summary>
        public event EventHandler<T> OnMessageSent;
        /// <summary>
        /// fired when an error occurrs
        /// </summary>
        public event EventHandler<Exception> OnError;
        /// <summary>
        /// fired when this client has been disconnected
        /// </summary>
        public event EventHandler OnDisconnected;

        /// <summary>
        /// the connection status for this client
        /// </summary>
        public bool Connected
        {
            get { return Client != null && Client.Connected; }
        }
        /// <summary>
        /// the hostname of the endpoint this client is connected to
        /// </summary>
        public string HostName
        {
            get { return Endpoint == null ? null : Endpoint.Address.ToString(); }
        }
        /// <summary>
        /// the port this client is using to communicate
        /// </summary>
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

        /// <summary>
        /// constructs a tcp network connection client
        /// </summary>
        /// <param name="serializer">serializer to use for message sending</param>
        public TcpNetworkConnection(INetworkConnectionSerializer<T> serializer)
        {
            this.Serializer = serializer;

            this.Outbox = new Queue<T>();
        }

        internal TcpNetworkConnection(TcpClient client, INetworkConnectionSerializer<T> serializer)
            : this(serializer)
        {
            this.Client = client;
            StartThreads();
        }

        /// <summary>
        /// disconnects and cleans up client threading and event subscribers
        /// </summary>
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

        /// <summary>
        /// disconnects and cleans up threads
        /// </summary>
        public void Disconnect()
        {
            new Thread(() =>
            {
                CleanupThreads();
                CleanupClient();
            }).StartBackground();
        }

        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="hostname">name of the endpoint, either an IP or DNS resolvable name</param>
        /// <param name="port">port number to communicate on</param>
        public void Connect(string hostname, int port)
        {
            var ips = Dns.GetHostAddresses(hostname);
            if (ips.Length == 0)
                throw new ArgumentException("Unable to resolve hostname");

            Connect(new IPEndPoint(ips[0], port));
        }

        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="connectionString">connection string must follow the format "www.oodlesofboodlesnoodles.com:9000" or "192.168.10.100:9000"</param>
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

        /// <summary>
        /// connects to a remote endpoint if not already connected
        /// </summary>
        /// <param name="endpoint">endpoint to attempt a connection</param>
        public void Connect(IPEndPoint endpoint)
        {
            new Thread(() =>
            {
                CleanupThreads();
                CleanupClient();

                if (StartClient(endpoint))
                {
                    StartThreads();

                    OnConnected.SafeExecuteAsync(this, Endpoint);
                }
                else
                {
                    CleanupThreads();
                    CleanupClient();
                }
            }).StartBackground();
        }

        /// <summary>
        /// Queues a message to be sent
        /// </summary>
        /// <param name="message">the message to send</param>
        public void Queue(T message)
        {
            Outbox.Enqueue(message);
        }

        /// <summary>
        /// Queues the message then sends all pending messages
        /// </summary>
        /// <param name="message">the message to queue and send</param>
        public void Send(T message)
        {
            Outbox.Enqueue(message);
            Send();
        }

        /// <summary>
        /// Sends all queued messages
        /// </summary>
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

                        OnMessageReceived.SafeExecuteAsync(this, message);
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
                OnError.SafeExecuteAsync(this, e);
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
                        OnError.SafeExecuteAsync(this, e);
                    }

                    if (buffer != null && Connected)
                    {
                        var sizeBytes = BitConverter.GetBytes((Int32)buffer.Length);//length of 4
                        buffer = sizeBytes.Concat(buffer).ToArray();

                        outStream.Write(buffer, 0, buffer.Length);

                        OnMessageSent.SafeExecuteAsync(this, message);
                    }
                }
            }
            catch (IOException e)
            {
                CleanupClient();
                OnError.SafeExecuteAsync(this, e);
            }
            catch (ThreadAbortException) { }
        }

        private void CleanupClient()
        {
            if (Client != null)
            {
                OnDisconnected.SafeExecuteAsync(this);

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
                OnError.SafeExecuteAsync(this, e);
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
