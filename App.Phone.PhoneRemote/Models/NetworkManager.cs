using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Phone.PhoneRemote.Models
{
    /// <summary>
    /// Manages the internals of the network, providing a simple interface for the viewmodel consumer.
    /// Responsible for handling most incoming events.
    /// </summary>
    internal class NetworkManager : IDisposable
    {
        private readonly UdpSimpleMessenger SimpleMessenger;

        public NetworkManager()
        {
            SimpleMessenger = new UdpSimpleMessenger();
            SimpleMessenger.MessageReceived += HandleMessage;
        }

        /// <summary>
        /// starts up the incoming messenger
        /// </summary>
        public void Startup()
        {
            SimpleMessenger.StartListening();
        }

        /// <summary>
        /// shuts the lower levels completely off
        /// </summary>
        public void Shutdown()
        {
            SimpleMessenger.Disconnect();
            SimpleMessenger.StopListening();
        }

        /// <summary>
        /// Sets host that will receive the outbound messages
        /// </summary>
        /// <param name="hostname">destination host</param>
        public void SetNewMessageHost(string hostname)
        {
            SimpleMessenger.Connect(hostname);
        }

        /// <summary>
        /// Sends a message to the connected host
        /// </summary>
        /// <param name="message">message to send</param>
        public void SendMessage(string message)
        {
            SimpleMessenger.SendMessage(message);
        }

        public void Dispose()
        {
            SimpleMessenger.MessageReceived -= HandleMessage;
            Shutdown();
        }

        /// <summary>
        /// Incoming Udp message handler
        /// </summary>
        private void HandleMessage(object sender, string message)
        {
            //if it's a broadcast, point that direction
            if(IsMessageBroadcast(message))
            {
                var host = GetHostFromBroadcastMessage(message);
            }
        }

        /// <summary>
        /// Determines if the message is a broadcast message
        /// </summary>
        /// <param name="message">message to check</param>
        /// <returns>whether or not it's a broadcast message</returns>
        private bool IsMessageBroadcast(string message)
        {
            //TODO-SM haven't decided on a protocol
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the hostname from the message
        /// </summary>
        /// <param name="message">broadcast message</param>
        /// <returns>hostname the broadcast would like you to connect to</returns>
        private string GetHostFromBroadcastMessage(string broadcastMessage)
        {
            //TODO-SM haven't decided on a protocol yet
            throw new NotImplementedException();
        }
    }
}
