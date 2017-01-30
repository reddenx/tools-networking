using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    public enum TcpMessengerStates
    {
        Connected,
        Connecting,
        Disconnected,
    }

    public interface ITcpMessenger
    {
        event EventHandler OnDisconnected;
        event EventHandler OnConnected;
        event EventHandler<byte[]> OnMessageReceived;
        event EventHandler<byte[]> OnMessageSent;
        event EventHandler<string> OnError;

        TcpMessengerStates State { get; }

        void Connect();
        void Send(byte[] message);
        void Disconnect();
    }

    public class TcpMessenger : ITcpMessenger, IDisposable
    {
        public event EventHandler OnDisconnected;
        public event EventHandler OnConnected;
        public event EventHandler<byte[]> OnMessageReceived;
        public event EventHandler<byte[]> OnMessageSent;
        public event EventHandler<string> OnError;

        private readonly string Host;
        private readonly int Port;
        public TcpMessengerStates State { get; private set; }

        private bool Disposing;
        private Thread ReceiveThread;
        private Thread SendThread;

        private Queue<byte[]> Outbox;

        private TcpClient Client;

        public TcpMessenger(TcpClient connectedClient)
        {
            connectedClient.NoDelay = true;
            this.State = TcpMessengerStates.Connected;
            this.Disposing = false;

            this.Client = connectedClient;

            this.Outbox = new Queue<byte[]>();

            this.ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            this.ReceiveThread.IsBackground = true;
            this.ReceiveThread.Start();

            this.SendThread = new Thread(new ThreadStart(SendLoop));
            this.SendThread.IsBackground = true;
            this.SendThread.Start();
        }

        public TcpMessenger(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            this.State = TcpMessengerStates.Disconnected;
            this.Disposing = false;

            this.Client = null;

            this.Outbox = new Queue<byte[]>();

            this.ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            this.ReceiveThread.IsBackground = true;
            this.ReceiveThread.Start();

            this.SendThread = new Thread(new ThreadStart(SendLoop));
            this.SendThread.IsBackground = true;
            this.SendThread.Start();
        }

        private void SendLoop()
        {
            while (!Disposing)
            {
                if (this.State != TcpMessengerStates.Connected)
                {
                    Thread.Sleep(100);
                    continue;
                }

                try
                {
                    //do sending here

                    bool hasMessages = false;
                    lock (Outbox)
                    {
                        hasMessages = Outbox.Any();
                    }

                    if (hasMessages)
                    {
                        byte[] messageBytes = null;
                        lock(Outbox)
                        {
                            messageBytes = Outbox.Dequeue();
                        }

                        Int32 messageLength = messageBytes.Length;
                        var lengthBytes = BitConverter.GetBytes(messageLength);
                        var fullMessageBytes = lengthBytes.Concat(messageBytes).ToArray();

                        var stream = this.Client.GetStream();
                        stream.Write(fullMessageBytes, 0, fullMessageBytes.Length);

                        this.OnMessageSent.SafeExecute(this, messageBytes);
                    }
                }
                catch (Exception e)
                {
                    //we don't want to catch this crap, gtfo
                    if (e is ThreadAbortException)
                        return;

                    this.OnError.SafeExecute(this, "TcpMessenger.SendLoop, exception whilesending message, error: " + e.Message);

                    //error handle and change state if necessary
                    throw new NotImplementedException(null, e);
                }

                Thread.Yield();
            }
        }

        private void ReceiveLoop()
        {
            while (!Disposing)
            {
                if (this.State != TcpMessengerStates.Connected)
                {
                    Thread.Sleep(100);
                    continue;
                }

                try
                {
                    //do message receive loop here
                    var stream = this.Client.GetStream();

                    //check first 4 bytes for size
                    var sizeBuffer = new byte[4];
                    int readByteCount = stream.Read(sizeBuffer, 0, 4);
                    if (readByteCount != 4)
                    {
                        SyncState();
                        Thread.Yield();
                        continue;
                    }

                    var expectedMessageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    //this is a close signal
                    if (expectedMessageSize == -1)
                    {
                        this.Disconnect();
                        continue;
                    }

                    readByteCount = 0;
                    var messageBuffer = new byte[expectedMessageSize];

                    //read until that is complete and move on
                    while (readByteCount < expectedMessageSize)
                    {
                        var readMessageByteCount = stream.Read(messageBuffer, readByteCount, expectedMessageSize);
                        readByteCount += readMessageByteCount;
                    }

                    this.OnMessageReceived.SafeExecute(this, messageBuffer);
                }
                catch (ThreadAbortException)
                {
                    //oh shit we don't want to catch this fireball, gtfo
                    return;
                }
                catch (IOException e)
                {
                    //this gets thrown if it interrupts the blocking call
                    if (e.Message.Contains("blocking"))
                    {
                        return;
                    }

                    this.OnError.SafeExecute(this, "TcpMessenger.ReceiveLoop, exception while receiving message, error: " + e.Message);
                    SyncState();
                }
                catch (Exception e)
                {
                    this.OnError.SafeExecute(this, "TcpMessenger.ReceiveLoop, exception while receiving message, error: " + e.Message);

                    //error handle and change state if necessary
                    SyncState();
                    //throw new NotImplementedException(null, e);
                }

                Thread.Yield();
            }
        }

        public void Connect()
        {
            //we're null gating out client just in case
            if (Client == null)
            {
                this.Client = new TcpClient();
                this.Client.NoDelay = true;
                this.State = TcpMessengerStates.Connecting;
                var result = this.Client.BeginConnect(this.Host, this.Port, HandleClientConnect, this);
            }
        }

        private void HandleClientConnect(IAsyncResult ar)
        {
            this.SyncState();
            this.Client.EndConnect(ar);
        }

        //handles all state changes and execution of events for state
        private void SyncState()
        {
            //change from disconnected to connected or reversed
            if (this.State == TcpMessengerStates.Connected && (this.Client == null || !this.Client.Connected))
            {
                this.State = TcpMessengerStates.Disconnected;

                if (this.Client != null)
                {
                    try
                    {
                        this.Client.Close();
                    }
                    catch (Exception e)
                    {
                        this.OnError.SafeExecute(this, "TcpMessenger.SyncState, exception while closing client, error: " + e.Message);
                    }

                    this.Client = null;
                }

                this.OnDisconnected.SafeExecute(this);
            }
            else if (this.State == TcpMessengerStates.Connecting && (this.Client != null && this.Client.Connected))
            {
                this.State = TcpMessengerStates.Connected;

                this.OnConnected.SafeExecute(this);
            }
        }

        public void Send(byte[] message)
        {
            lock (this.Outbox)
            {
                this.Outbox.Enqueue(message);
            }
        }

        public void Disconnect()
        {
            if (this.Client != null)
            {
                //lets try telling the other person to disconnect
                if (this.Client.Connected && this.State == TcpMessengerStates.Connected)
                {
                    var stream = this.Client.GetStream();
                    if (stream.CanWrite)
                    {
                        Int32 closeCall = -1;
                        stream.Write(BitConverter.GetBytes(closeCall), 0, 4);
                    }
                }


                try
                {
                    this.Client.Close();
                }
                catch (Exception e)
                {
                    this.OnError.SafeExecute(this, "TcpMessenger.Disconnect, exception while closing client, error: " + e.Message);
                }
                this.Client = null;
                this.SyncState();
            }
        }

        public void Dispose()
        {
            //set disposing true,
            this.Disposing = true;

            //wait for threads to finish for a moment, kill if they take too long
            this.SendThread.Join(1000);
            if (this.SendThread.IsAlive)
            {
                try
                {
                    this.SendThread.Abort();
                }
                catch (Exception e)
                {
                    this.OnError.SafeExecute(this, "TcpMessenger.Close, exception while aborting send thread, error: " + e.Message);
                }
            }

            this.ReceiveThread.Join(1000);
            if (this.ReceiveThread.IsAlive)
            {
                try
                {
                    this.ReceiveThread.Abort();
                }
                catch (Exception e)
                {
                    this.OnError.SafeExecute(this, "TcpMessenger.Close, exception while aborting receive thread, error: " + e.Message);
                }
            }

            //disconnect if connected
            //cleanup channel if necessary
            this.Disconnect();

            if (this.Client != null)
            {
                try
                {
                    this.Client.Close();
                }
                catch (Exception e)
                {
                    this.OnError.SafeExecute(this, "TcpMessenger.Close, exception while closing active TcpClient, error: " + e.Message);
                }
            }
        }
    }
}
