using System;

namespace SMT.Networking.NetworkConnection
{
    /// <summary>
    /// base interface for a network connection client, agnostic to the underlying connection type
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
