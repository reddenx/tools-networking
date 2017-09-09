using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Udp
{
    internal class SimpleUdpMessageSender<TMessage>
    {
        private readonly UdpClient Client;
        private readonly BinaryFormatter Formatter;

        public SimpleUdpMessageSender()
        {
            Client = new UdpClient();
            Formatter = new BinaryFormatter();
        }

        public void SendMessage(string host, int port, TMessage message)
        {
            var serializedMessageBytes = message is string ? GetSerializedMessageASCII(message as string) : GetSerializedMessage(message);
            Client.Send(serializedMessageBytes, serializedMessageBytes.Length, host, port);
        }

        private byte[] GetSerializedMessageASCII(string message)
        {
            return ASCIIEncoding.ASCII.GetBytes(message);
        }

        private byte[] GetSerializedMessage(TMessage message)
        {
            using (var stream = new MemoryStream())
            {
                Formatter.Serialize(stream, message);
                return stream.ToArray();
            }
        }
    }
}
