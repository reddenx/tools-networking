using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Udp
{
    internal class SimpleUdpMessageListener<TMessage>
        where TMessage : class
    {
        public event EventHandler<TMessage> OnMessageReceived;
        public event EventHandler<string> OnNetworkError;
        private UdpClient Client;
        private readonly BinaryFormatter Formatter;
        private int ListenPort;
        private Thread ListenThread;

        public SimpleUdpMessageListener()
        {
            Client = null;
            Formatter = new BinaryFormatter();
        }

        public void StartListening(int port)
        {
            if (Client != null)
            {
                CleanupClient();
            }

            try
            {
                Client = new UdpClient(port);
                ListenPort = port;
                StartListenLoop();
            }
            catch (SocketException e)
            {
                RaiseError(e.Message);
            }
        }

        private void StartListenLoop()
        {
            if (ListenThread != null)
            {
                CleanupThread();
            }

            ListenThread = new Thread(new ThreadStart(ListenLoop));
            ListenThread.IsBackground = true;
            ListenThread.Start();
        }

        private void ListenLoop()
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Any, ListenPort);
                while (true)
                {
                    var messageBytes = Client.Receive(ref endpoint);
                    var message = GetMessageFromBytes(messageBytes);
                    if (OnMessageReceived != null)
                    {
                        OnMessageReceived(this, message);
                    }
                }
            }
            catch (SocketException e)
            {
                RaiseError(e.Message);
            }
        }

        private TMessage GetMessageFromBytes(byte[] messageBytes)
        {
            if (typeof(TMessage) == typeof(string))
            {
                return ASCIIEncoding.ASCII.GetString(messageBytes) as TMessage;
            }
            else
            {
                using (var stream = new MemoryStream(messageBytes))
                {
                    return (TMessage)Formatter.Deserialize(stream);
                }
            }
        }

        private void CleanupClient()
        {
            try
            {
                Client.Close();
            }
            catch (SocketException e)
            {
                RaiseError(e.Message);
            }
            finally
            {
                Client = null;
            }
        }

        private void CleanupThread()
        {
            try
            {
                if (!ListenThread.Join(800))
                {
                    ListenThread.Abort();
                }
            }
            catch (Exception e)
            {
                RaiseError(e.Message);
            }
            finally
            {
                ListenThread = null;
            }
        }

        public void StopListening()
        {
            if (Client != null)
            {
                CleanupClient();
            }
            else
            {
                RaiseError("Attempted to shutdown an inactive client");
            }
        }

        private void RaiseError(string error)
        {
            if (OnNetworkError != null)
            {
                OnNetworkError(this, error);
            }
        }
    }
}
