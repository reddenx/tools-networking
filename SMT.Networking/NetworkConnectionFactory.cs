using SMT.Networking.Interfaces;
using SMT.Networking.Tcp;
using SMT.Networking.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking
{
    public static class NetworkConnectionFactory
    {
        public static INetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnection<T>(serializer);
        }

        public static INetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnectionListener<T>(serializer);
        }

        //unfinished component
        public static INetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new UdpNetworkConnection<T>(serializer);
        }
    }
}
