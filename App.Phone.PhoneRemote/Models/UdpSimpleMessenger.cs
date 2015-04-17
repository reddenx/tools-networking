using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace App.Phone.PhoneRemote.Models
{
    /// <summary>
    /// Simple UDP string messenger friendly with the windows desktop/app/phone environment constriants
    /// </summary>
    internal class UdpSimpleMessenger
    {
        private const int UDP_PORT = 37019;

        public event EventHandler<string> MessageReceived;

        private DatagramSocket ListenClient;
        private DatagramSocket SendClient; //REFACTOR-SM: send and writer share lifecycle, could make custom udpsender object, could probably do the same with the listener
        private DataWriter Writer;

        public bool IsConnected { get { return SendClient != null; } }
        public bool IsListening { get { return ListenClient != null; } }

        public UdpSimpleMessenger()
        {
            SendClient = null;
            ListenClient = null;
            Writer = null;
        }

        /// <summary>
        /// start listening for incoming messages
        /// </summary>
        public async void StartListening()
        {
            if (!IsListening)
            {
                //instantiate and event hookup
                ListenClient = new DatagramSocket();
                ListenClient.MessageReceived += HandleMessageReceived;

                //get the proper network bind it to, prioritize wifi (iana 71)
                var localNetworks = NetworkInformation.GetHostNames();
                HostName bestNetwork = localNetworks.FirstOrDefault(network => network.IPInformation != null && network.IPInformation.NetworkAdapter.IanaInterfaceType == 71);
                if (bestNetwork == null)
                {
                    bestNetwork = localNetworks.FirstOrDefault(network => network.IPInformation != null);
                }

                //if there's no applicable network, back the hell out
                if (bestNetwork == null)
                {
                    CleanupListener();
                    return;
                }

                //bind it to the network and then we're "connected"
                await ListenClient.BindServiceNameAsync(UDP_PORT.ToString(), bestNetwork.IPInformation.NetworkAdapter);
            }
        }

        /// <summary>
        /// Stop the incoming message listener
        /// </summary>
        public void StopListening()
        {
            if (IsListening)
            {
                CleanupListener();
            }
        }

        /// <summary>
        /// Connect to a client
        /// </summary>
        /// <param name="host">hostname ip or url (portless)</param>
        public async void Connect(string host)
        {
            if (!IsConnected)
            {
                //create it
                SendClient = new DatagramSocket();
            
                //connect it
                await SendClient.ConnectAsync(new HostName(host), UDP_PORT.ToString());

                //setup writer
                Writer = new DataWriter(SendClient.OutputStream);
            }
        }

        /// <summary>
        /// Disconnect from the client
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                //cleaup writer
                Writer.DetachStream();
                Writer.Dispose();

                //cleanup sender
                SendClient.Dispose();
                SendClient = null;
            }
        }

        /// <summary>
        /// Send a message to the connected client
        /// </summary>
        /// <param name="message">message to be sent</param>
        public async void SendMessage(string message)
        {
            if (IsConnected)
            {
                //write and commit
                Writer.WriteString(message);
                await Writer.StoreAsync();
            }
        }

        /// <summary>
        /// disposes, detaches, and nullifies the listener
        /// </summary>
        private void CleanupListener()
        {
            ListenClient.MessageReceived -= HandleMessageReceived;
            ListenClient.Dispose();
            ListenClient = null;
        }

        /// <summary>
        /// Native message handler for the DatagramSockets
        /// </summary>
        /// <param name="sender">source socket</param>
        /// <param name="args">message arguments</param>
        private void HandleMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            //get the reader and then the message from it
            var reader = args.GetDataReader();
            var message = reader.ReadString(reader.UnconsumedBufferLength);

            //handle that message with the message handler
            MessageReceived(this, message);
            //MessageReceived.BeginInvoke(this, message, (result) => { }, null);//REFACTOR-SM: I really want this async thing to work but I doubt it will
        }
    }
}
