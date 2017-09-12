using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Networking.Interfaces.SimpleMessaging;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SMT.Networking.Udp
{
    internal class SimpleUdpMessenger<TMessage> : ISimpleMessenger<TMessage>
        where TMessage : class
    {
        private SimpleUdpMessageListener<TMessage> Listener;
        private SimpleUdpMessageSender<TMessage> Messenger;

        private string Host;
        private int Port;

        private bool Connected;

        public SimpleUdpMessenger()
        {
            Connected = false;
            Listener = new SimpleUdpMessageListener<TMessage>();
            Messenger = new SimpleUdpMessageSender<TMessage>();

            Listener.OnMessageReceived += Listener_OnMessageReceived;
        }

        void Listener_OnMessageReceived(object sender, TMessage e)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(this, e);
            }
        }

        //public event EventHandler<TMessage> OnMessageReceived;

        //private Thread ReceiveThread;
        //private UdpClient Client;
        //private bool IsConnected;
        //private BinaryFormatter Formatter;

        //private IPEndPoint RemoteEndPoint;

        //public SimpleUdpMessenger()
        //{
        //    Client = null;
        //    ReceiveThread = null;
        //    Formatter = new BinaryFormatter();
        //}

        //public void Connect(string host, int port)
        //{
        //    if (!IsConnected)
        //    {
        //        try
        //        {
        //            IsConnected = true;

        //            Client = new UdpClient(port);

        //            if (!string.IsNullOrEmpty(host))
        //            {
        //                var resolvedHost = ResolveHostname(host);
        //                RemoteEndPoint = new IPEndPoint(resolvedHost, port);
        //                Client.Connect(RemoteEndPoint);//really only serves as a filter
        //            }
        //            else
        //            {
        //                RemoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        //            }

        //            ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
        //            ReceiveThread.IsBackground = true;
        //            ReceiveThread.Start();
        //        }
        //        catch (SocketException e)
        //        {
        //            CleanupEverything();

        //            throw new ApplicationException("Couldn't connect", e);
        //        }
        //    }
        //}

        //private IPAddress ResolveHostname(string host)
        //{
        //    var addresses = Dns.GetHostAddresses(host);
        //    if (!addresses.Any())
        //    {
        //        throw new SocketException();
        //    }

        //    return addresses[0];
        //}

        //private void ReceiveLoop()
        //{
        //    try
        //    {
        //        while (IsConnected)
        //        {
        //            var messageBytes = Client.Receive(ref RemoteEndPoint);

        //            TMessage message = null;
        //            if (typeof(TMessage) == typeof(string))
        //            {
        //                message = GetDeserializedMessageString(messageBytes) as TMessage;
        //            }
        //            else
        //            {
        //                message = GetDeserializedMessage(messageBytes);
        //            }

        //            if (OnMessageReceived != null)
        //            {
        //                OnMessageReceived(this, message);
        //            }
        //        }
        //    }
        //    catch (SocketException e)
        //    { }
        //    catch (ThreadAbortException e)
        //    { }
        //}

        //public void Send(TMessage message)
        //{
        //    if (IsConnected)
        //    {
        //        var serializedMessageBytes = GetSerializedMessage(message);
        //        Client.Send(serializedMessageBytes, serializedMessageBytes.Length);
        //    }
        //}

        //public void Disconnect()
        //{
        //    if (IsConnected)
        //    {
        //        CleanupEverything();
        //    }
        //}

        //private void CleanupEverything()
        //{
        //    IsConnected = false;

        //    try
        //    {
        //        Client.Close();
        //    }
        //    catch { }
        //    Client = null;

        //    if (!ReceiveThread.Join(1000))
        //    {
        //        ReceiveThread.Abort();
        //    }
        //    ReceiveThread = null;
        //}

        //private byte[] GetSerializedMessage(TMessage message)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        Formatter.Serialize(stream, message);
        //        return stream.ToArray();
        //    }
        //}

        //private string GetDeserializedMessageString(byte[] bytes)
        //{
        //    return ASCIIEncoding.ASCII.GetString(bytes);
        //}

        //private TMessage GetDeserializedMessage(byte[] bytes)
        //{
        //    using (var stream = new MemoryStream(bytes))
        //    {
        //        return (TMessage)Formatter.Deserialize(stream);
        //    }
        //}

        public event EventHandler<TMessage> OnMessageReceived;

        public void Connect(string host, int port)
        {
            if (Connected)
            {
                Disconnect();
            }

            Listener.StartListening(port);
            Host = host;
            Port = port;
        }

        public void Send(TMessage message)
        {
            if (Connected)
            {
                Messenger.SendMessage(Host, Port, message);
            }
        }

        public void Disconnect()
        {
            if (Connected)
            {
                Listener.StopListening();
                Host = null;
                Port = 0;
            }
        }
    }
}
