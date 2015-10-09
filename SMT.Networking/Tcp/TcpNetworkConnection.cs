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
    public class TcpNetworkConnection<T> : INetworkConnection<T>
    {
        public delegate byte[] Serialize(T message);
        public delegate T Deserialize(byte[] message);

        private const int MAX_MESSAGE_SIZE = 2048;

        public event EventHandler<T> OnMessageReceived;
        public event EventHandler<IPEndPoint> OnConnected;
        public event EventHandler<T> OnMessageSent;
        public event EventHandler<Exception> OnError;
        public event EventHandler OnDisconnected;

        public bool Connected
        {
            get { return Client == null ? false : Client.Connected; }
        }
        public string HostName
        {
            get { return Endpoint == null ? null : Endpoint.Address.ToString(); }
        }
        public int Port
        {
            get { return Endpoint == null ? -1 : Endpoint.Port; }
        }

        private readonly Deserialize Deserializer;
        private readonly Serialize Serializer;

        private IPEndPoint Endpoint;
        private TcpClient Client;
        private Thread SendThread;
        private Thread ReceiveThread;

        private Queue<T> Outbox;
        private Queue<T> Inbox; //maybe use later in some other scheme, for now this will just bloat

        public TcpNetworkConnection(Deserialize deserializer, Serialize serializer)
        {
            this.Serializer = serializer;
            this.Deserializer = deserializer;

            this.Outbox = new Queue<T>();
            this.Inbox = new Queue<T>();
        }

        public TcpNetworkConnection(TcpClient client, Deserialize deserializer, Serialize serializer)
            :this(deserializer, serializer)
        {
            this.Client = client;
            StartThreads();
        }

        public void Dispose()
        {
            Disconnect();

            DoCheapAsync(() =>
            {
                while (Connected) { Thread.Sleep(10); }

                OnMessageReceived.GetInvocationList().ToList().ForEach(item => OnMessageReceived -= (EventHandler<T>)item);
                OnConnected.GetInvocationList().ToList().ForEach(item => OnConnected -= (EventHandler<IPEndPoint>)item);
                OnMessageSent.GetInvocationList().ToList().ForEach(item => OnMessageSent -= (EventHandler<T>)item);
                OnError.GetInvocationList().ToList().ForEach(item => OnError -= (EventHandler<Exception>)item);
                OnDisconnected.GetInvocationList().ToList().ForEach(item => OnDisconnected -= (EventHandler)item);
            });
        }

        public void Disconnect()
        {
            DoCheapAsync(() =>
            {
                CleanupThreads();
                CleanupClient();
            });
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
            var pieces = connectionString.Split(':');
            if (pieces.Length != 2)
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            int port = -1;
            if (!int.TryParse(pieces[1], out port))
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            Connect(pieces[0], port);
        }

        private void Connect(IPEndPoint endpoint)
        {
            DoCheapAsync(() =>
            {
                CleanupThreads();
                CleanupClient();

                if (StartClient(endpoint))
                {
                    StartThreads();

                    if (OnConnected != null)
                        OnConnected(this, Endpoint);
                }
                else
                {
                    CleanupThreads();
                    CleanupClient();
                }
            });
        }

        public void Send(T message)
        {
            Outbox.Enqueue(message);
        }

        public T[] GetPendingMessages()
        {
            T[] messages;
            lock(Inbox)
            {
                messages = Inbox.ToArray();
                Inbox.Clear();
            }
            return messages;
        }

        private void ReceiveLoop()
        {
            try
            {
                var instream = Client.GetStream();
                byte[] buffer = new byte[MAX_MESSAGE_SIZE];

                while (Connected)
                {
                    int bytesRead = instream.Read(buffer, 0, 4);
                    if (bytesRead != 4)
                    {
                        CleanupClient();
                        return; //stream has been closed;
                    }

                    int messageSize = BitConverter.ToInt32(buffer, 0);
                    if (messageSize > 0 && messageSize < MAX_MESSAGE_SIZE)
                    {
                        int totalReadBytes = 0;
                        do //handling partial messages
                        {
                            int messageBytesRead = instream.Read(buffer, totalReadBytes, messageSize - totalReadBytes);
                            if (bytesRead <= 0)
                            {
                                CleanupClient();
                                return; //stream has been closed
                            }

                            totalReadBytes += messageBytesRead;
                        }
                        while (totalReadBytes < messageSize);

                        var message = Deserializer(buffer.Take(messageSize).ToArray());

                        lock (Inbox)
                        {
                            Inbox.Enqueue(message);
                        }
                        
                        OnMessageReceived(this, message);
                    }
                    else
                    {
                        throw new IOException("message was improperly formatted or too large");
                    }
                }
            }
            catch (IOException e)
            {
                CleanupClient();
                if (OnError != null)
                    OnError(this, e);
            }
            catch (ThreadAbortException) { }//expecting these
        }

        private void SendLoop()
        {
            var outStream = Client.GetStream();

            while (Connected)
            {
                if (Outbox.Count > 0)
                {
                    var message = Outbox.Dequeue();
                    var messageBytes = Serializer(message);
                    var sizeBytes = BitConverter.GetBytes(messageBytes.Length);//length of 4

                    outStream.Write(sizeBytes, 0, 4);
                    outStream.Write(messageBytes, 0, messageBytes.Length);

                    if (OnMessageSent != null)
                        OnMessageSent(this, message);
                }
                else
                {
                    Thread.Sleep(10);//yield while waiting
                }
            }
        }

        private void CleanupClient()
        {
            if (Client != null)
            {
                if (OnDisconnected != null)
                    OnDisconnected(this, null);

                if (Client.Connected)
                {
                    Client.Close();
                }
                Client = null;
            }
        }

        private void CleanupThreads()
        {
            CleanUpCheapThread(SendThread);
            CleanUpCheapThread(ReceiveThread);
        }

        private bool StartClient(IPEndPoint endpoint)
        {
            Client = new TcpClient();
            Client.NoDelay = true;

            try
            {
                Client.Connect(endpoint);
                this.Endpoint = endpoint;
                return true;
            }
            catch (SocketException e)
            {
                if (OnError != null)
                    OnError(this, e);
                return false;
            }
        }

        private void StartThreads()
        {
            if (SendThread != null || ReceiveThread != null)
                CleanupThreads();

            SendThread = DoCheapAsync(SendLoop);
            ReceiveThread = DoCheapAsync(ReceiveLoop);
        }

        //small lived operations flung into the background
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
