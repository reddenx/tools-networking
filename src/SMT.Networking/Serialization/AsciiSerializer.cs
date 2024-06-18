﻿using System.Text;

namespace SMT.Networking.Serialization
{
    public class AsciiSerializer : INetworkConnectionSerializer<string>
    {
        public byte[] Serialize(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }

        public string Deserialize(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}
