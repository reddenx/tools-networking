using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using SMT.Networking.Utility;

namespace SMT.Networking
{
    /// <summary>
    /// UDP client, turns synchronous calls into events
    /// </summary>
    /// <typeparam name="T">Message Type</typeparam>
    public class UdpNetworkConnection<T>
    {
        private const int DEFAULT_PORT = -1;

        /// <summary>
        /// fired when a message is received from the remote host
        /// </summary>
        public event EventHandler<T> OnMessageReceived;
        /// <summary>
        /// fired when a message is successfully sent to the remote host
        /// </summary>
        public event EventHandler<T> OnMessageSent;
        /// <summary>
        /// fired when an error occurs, often resulting in a network cleanup
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// remote hostname
        /// </summary>
        public string HostName { get; private set; }
        /// <summary>
        /// local bound port
        /// </summary>
        public int Port { get; private set; }

        private bool IsListening;

        private readonly UdpClient SendClient;
        private UdpClient ReceiveClient;

        private Thread SendThread;
        private Thread ReceiveThread;

        private readonly INetworkConnectionSerializer<T> Serializer;
        private readonly Queue<T> OutBox;

        public UdpNetworkConnection(INetworkConnectionSerializer<T> serializer)
        {
            OutBox = new Queue<T>();
            HostName = null;
            Port = DEFAULT_PORT;
            SendClient = new UdpClient();
            ReceiveClient = null;

            ReceiveThread = null;
            SendThread = null;

            Serializer = serializer;
        }

        /// <summary>
        /// queue a message to be sent
        /// </summary>
        /// <param name="message"></param>
        public void Queue(T message)
        {
            OutBox.Enqueue(message);
        }

        /// <summary>
        /// send all messages queued
        /// </summary>
        public void Send()
        {
            if (SendThread == null || !SendThread.IsAlive)
                SendThread = new Thread(SendLoop).StartBackground();
        }

        /// <summary>
        /// queue and send all messages
        /// </summary>
        /// <param name="message"></param>
        public void Send(T message)
        {
            Queue(message);
            Send();
        }

        //unbind port, stop threads, kill event lists
        public void Dispose()
        {
            new Thread(() =>
            {
                ReceiveThread.DisposeOfThread(100);
                SendThread.DisposeOfThread(100);

                OnError.RemoveAllListeners();
                OnMessageReceived.RemoveAllListeners();
                OnMessageSent.RemoveAllListeners();

                try
                {
                    if (ReceiveClient != null && IsListening)
                        ReceiveClient.Close();
                }
                catch { }

            }).StartBackground();
        }

        //run once connected, cleanup once disconnected
        private void SendLoop()
        {
            try
            {
                while (OutBox.Count > 0)
                {
                    var message = OutBox.Dequeue();
                    byte[] buffer = null;
                    try
                    {
                        buffer = Serializer.Serialize(message);
                    }
                    catch (Exception e)
                    {
                        OnError?.Invoke(this, e);
                    }

                    if (buffer != null && !string.IsNullOrWhiteSpace(HostName) && Port > 0)
                    {
                        SendClient.Send(buffer, buffer.Length, HostName, Port);
                        OnMessageSent?.Invoke(this, message);
                    }
                }
            }
            catch (IOException e)
            {
                OnError?.Invoke(this, e);
            }
            catch (ThreadAbortException aborted) { } //expected abort procedure given blocking call
            finally
            {
                //cleanup
            }
        }

        private void ReceiveLoop()
        {
            try
            {
                while (IsListening)
                {
                    var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    var data = ReceiveClient.Receive(ref remoteEndpoint);

                    try
                    {
                        var message = Serializer.Deserialize(data);
                        OnMessageReceived?.Invoke(this, message);
                    }
                    catch (Exception e)
                    {
                        OnError?.Invoke(this, e);
                    }
                }
            }
            catch (IOException e)
            {
                StopListening();
                OnError?.Invoke(this, e);
            }
            catch (ThreadAbortException) { }//expecting these on abort
        }

        /// <summary>
        /// Binds and start listening to the specified port
        /// </summary>
        /// <param name="port">port to listen to</param>
        /// <returns>true if it successfully binds and starts listening</returns>
        public bool StartListening(int port)
        {
            if (IsListening)
            {
                OnError?.Invoke(this, new IOException("udp networkconnection is already listening"));
                return false;
            }

            IsListening = true;
            CleanupListener();
            ReceiveThread.DisposeOfThread(100);

            try
            {
                ReceiveClient = new UdpClient(port);
                ReceiveThread = new Thread(ReceiveLoop).StartBackground();
                Port = port;
                IsListening = true;
                return true;
            }
            catch (IOException e)
            {
                OnError?.Invoke(this, e);
                CleanupListener();
                StopListening();
                return false;
            }
        }

        /// <summary>
        /// Unbinds and stops listening
        /// </summary>
        public void StopListening()
        {
            if (!IsListening)
                return;

            IsListening = false;
            ReceiveThread.DisposeOfThread(100);
            CleanupListener();
        }

        private void CleanupListener()
        {
            if (ReceiveClient != null)
            {
                try
                {
                    ReceiveClient.Close();
                }
                catch (IOException e)
                {
                    OnError?.Invoke(this, e);
                }
                ReceiveClient = null;
            }
        }

        /// <summary>
        /// Directs messages to the specified enpoint
        /// </summary>
        /// <param name="hostname">name of the endpoint, either an IP or DNS resolvable name</param>
        /// <param name="port">port number to communicate on</param>
        public void Target(string hostname, int port)
        {
            HostName = hostname;
            Port = port;
        }

        /// <summary>
        /// Directs messages to the specified enpoint
        /// </summary>
        /// <param name="connectionString">connection string must follow the format "www.oodlesofboodlesnoodles.com:9000" or "192.168.10.100:9000"</param>
        public void Target(string connectionString)
        {
            var pieces = connectionString?.Split(':');
            if (pieces?.Length != 2)
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            int port = -1;
            if (!int.TryParse(pieces[1], out port))
                throw new ArgumentException("Improperly formatted string, expecting NAME:PORT e.g. www.oodlesofboodlesnoodles.com:9000");

            Port = port;
            HostName = pieces[0];
        }

        /// <summary>
        /// Clears the target endpoint
        /// </summary>
        public void Untarget()
        {
            HostName = null;
            Port = DEFAULT_PORT;
        }
    }
}
