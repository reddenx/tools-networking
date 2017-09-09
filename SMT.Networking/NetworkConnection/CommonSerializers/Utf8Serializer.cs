using System.Text;

namespace SMT.Networking.NetworkConnection
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
