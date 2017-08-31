using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface INetworkConnection<T> : IDisposable
    {
        /// <summary>
        /// fired when a message is received from the remote host
        /// </summary>
        event EventHandler<T> OnMessageReceived;
        /// <summary>
        /// fired when a message is successfully sent to the remote host
        /// </summary>
        event EventHandler<T> OnMessageSent;
        /// <summary>
        /// fired when an error occurs, often resulting in a network cleanup
        /// </summary>
        event EventHandler<Exception> OnError;

        /// <summary>
        /// remote hostname
        /// </summary>
        string HostName { get; }
        /// <summary>
        /// local bound port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// queue a message to be sent
        /// </summary>
        /// <param name="message"></param>
        void Queue(T message);
        /// <summary>
        /// queue and send all messages
        /// </summary>
        /// <param name="message"></param>
        void Send(T message);
        /// <summary>
        /// send all messages queued
        /// </summary>
        void Send();
    }
}
