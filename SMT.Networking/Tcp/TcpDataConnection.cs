using SMT.Networking.Interfaces.SimpleMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    public class TcpDataConnection<TMessageType>
        where TMessageType : class
    {
        public event EventHandler<TMessageType> MessageReceived;
        private Func<byte[], TMessageType> FromBytes;
        private Func<TMessageType, byte[]> ToBytes;

        private TcpClient Client;

        private Thread ReceiveThread;

        internal TcpDataConnection(TcpClient connectedClient, Func<byte[], TMessageType> fromBytes, Func<TMessageType, byte[]> toBytes)
            : this(fromBytes, toBytes)
        {
            Client = connectedClient;
            StartReceiveLoop();
        }

        public TcpDataConnection(Func<byte[], TMessageType> fromBytes, Func<TMessageType, byte[]> toBytes)
        {
            ToBytes = toBytes;
            FromBytes = fromBytes;
        }

        public bool Connect(string host, int port)
        {
            if (Client != null)
            {
                Disconnect();
            }

            try
            {
                Client = new TcpClient(host, port);
                StartReceiveLoop();
                return Client.Connected;
            }
            catch (SocketException e)
            {
                return false;
            }
        }

        public void Disconnect()
        {
            if (Client != null)
            {
                if (Client.Connected)
                {
                    ReceiveThread.Abort();
                    Client.Close();
                }
                else
                {
                    Client = null;
                }
            }
        }

        public bool SendData(TMessageType data)
        {
            byte[] messageBytes = ToBytes(data);
            int messageLength = messageBytes.Length;

            byte[] preMessageBytes = BitConverter.GetBytes(messageLength);

            NetworkStream netStream = Client.GetStream();

            netStream.Write(preMessageBytes, 0, 4);
            netStream.Write(messageBytes, 0, messageLength);

            return true;
        }

        private void OnMessageReceived(TMessageType message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, message);
            }
        }

        private void StartReceiveLoop()
        {
            ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();
        }

        private void ReceiveLoop()
        {
            try
            {
                NetworkStream netStream = Client.GetStream();
                byte[] messageLengthBytes = new byte[4];
                Int32 messageLengthInt32 = 0;

                while (true)
                {
                    netStream.Read(messageLengthBytes, 0, 4);
                    messageLengthInt32 = BitConverter.ToInt32(messageLengthBytes, 0);

                    byte[] messageBytes = new byte[messageLengthInt32];

                    int readBytes = netStream.Read(messageBytes, 0, messageLengthInt32);

                    if (readBytes == 0)
                    {
                        Disconnect();//connection died
                    }
                    else if (readBytes != messageLengthInt32)
                    {
                        throw new Exception("Partial message received");
                    }

                    OnMessageReceived(FromBytes(messageBytes));
                }
            }
            catch (ThreadAbortException)
            {
                //expected to break this from disconnect on blocking read
            }
            catch (SocketException)
            {
                //unexpected client disconnect
            }
        }
    }
}
