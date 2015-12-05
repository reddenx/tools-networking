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
    public interface INetworkConnectionFactory
    {
        INetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer);
        INetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer);
        INetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer);
    }

    //assembly component interface
    public class NetworkConnectionFactory : INetworkConnectionFactory
    {
        public INetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnection<T>(serializer);
        }

        public INetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnectionListener<T>(serializer);
        }

        //unfinished component
        public INetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new UdpNetworkConnection<T>(serializer);
        }
    }
}
