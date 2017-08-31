using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Networking.Interfaces;

namespace SMT.Networking.CommonSerializers
{
    public class Utf8Serializer : INetworkConnectionSerializer<string>
    {
        public byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public string Deserialize(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
