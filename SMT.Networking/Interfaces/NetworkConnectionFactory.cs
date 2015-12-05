using SMT.Networking.Tcp;
using SMT.Networking.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public static class NetworkConnectionFactory
    {
        public static INetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer, int maxMessageSize = 65536)
        {
            return new TcpNetworkConnection<T>(serializer, maxMessageSize);
        }

        public static INetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer, int maxMessageSize = 65536)
        {
            return new TcpNetworkConnectionListener<T>(serializer, maxMessageSize);
        }

        //unfinished component
        public static INetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer, int maxMessageSize = 65536)
        {
            return new UdpNetworkConnection<T>(serializer, maxMessageSize);
        }
    }
}
