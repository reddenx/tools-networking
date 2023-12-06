using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SMT.Utilities.Logging
{
    /// <summary>
    /// UDP client, turns synchronous calls into events
    /// </summary>
    /// <typeparam name="T">Message Type</typeparam>
    public class UdpNetworkSender
    {
        private const int DEFAULT_PORT = -1;

        /// <summary>
        /// remote hostname
        /// </summary>
        public string HostName { get; private set; }
        /// <summary>
        /// local bound port
        /// </summary>
        public int Port { get; private set; }


        private readonly UdpClient SendClient;
        private Thread SendThread;
        private readonly ConcurrentQueue<string> OutBox;

        public UdpNetworkSender()
        {
            OutBox = new ConcurrentQueue<string>();
            HostName = null;
            Port = DEFAULT_PORT;
            SendClient = new UdpClient();
            SendThread = null;
        }

        /// <summary>
        /// queue a message to be sent
        /// </summary>
        /// <param name="message"></param>
        public void Queue(string message) => OutBox.Enqueue(message);

        /// <summary>
        /// send all messages queued
        /// </summary>
        public void Send()
        {
            if (SendThread == null || !SendThread.IsAlive)
            {
                SendThread = new Thread(SendLoop) { IsBackground = true };
                SendThread.Start();
            }
        }

        /// <summary>
        /// queue and send all messages
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            Queue(message);
            Send();
        }

        //unbind port, stop threads, kill event lists
        public void Dispose()
        {
            DisposeOfThread(SendThread, 100);
        }

        //run once connected, cleanup once disconnected
        private void SendLoop()
        {
            try
            {
                while (OutBox.Count > 0)
                {
                    string message;
                    while (!OutBox.TryDequeue(out message))
                        Thread.Sleep(100);

                    byte[] buffer = null;

                    buffer = ASCIIEncoding.UTF8.GetBytes(message);

                    if (buffer != null && !string.IsNullOrWhiteSpace(HostName) && Port > 0)
                    {
                        SendClient.Send(buffer, buffer.Length, HostName, Port);
                    }
                }
            }
            catch (IOException) { }
            catch (ThreadAbortException) { } //expected abort procedure given blocking call
            finally
            {
                //cleanup?
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
        /// Clears the target endpoint
        /// </summary>
        public void Untarget()
        {
            HostName = null;
            Port = DEFAULT_PORT;
        }

        private static void DisposeOfThread(Thread thread, int timeoutMilli)
        {
            //give it every chance to cleanly terminate, then abort

            if (thread == null) return;
            if (!thread.IsAlive) return;
            if (thread.Join(timeoutMilli)) return;

            //thread.Abort(); //TODO there's gotta be a cross platform way to be sure of killing this, if not refactor for cancellation token
        }
    }
}