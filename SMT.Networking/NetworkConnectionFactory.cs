using SMT.Networking.CommonSerializers;
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
        ITcpNetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer);
        ITcpNetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer);

        IUdpNetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer);
    }

    /// <summary>
    /// factory for building network connections
    /// </summary>
    public class NetworkConnectionFactory : INetworkConnectionFactory
    {
        public ITcpNetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnection<T>(serializer);
        }

        public ITcpNetworkConnection<T> GetTcpNetworkConnection<T>(Func<T, byte[]> serialize, Func<byte[], T> deserialize)
        {
            return new TcpNetworkConnection<T>(new AnonymousSerializer<T>(deserialize, serialize));
        }

        public ITcpNetworkConnection<string> GetTcpNetworkConnection()
        {
            return new TcpNetworkConnection<string>(new AsciiSerializer());
        }

        public ITcpNetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpTcpNetworkConnectionListener<T>(serializer);
        }

        public ITcpNetworkConnectionListener<string> GetTcpNetworkConnectionListener()
        {
            return new TcpTcpNetworkConnectionListener<string>(new AsciiSerializer());
        }

        //unfinished component
        public IUdpNetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            throw new NotImplementedException("unfinished component");
            return new UdpNetworkConnection<T>(serializer);
        }

        public IUdpNetworkConnection<string> GetUdpNetworkConnection()
        {
            throw new NotImplementedException("unfinished component");
            return new UdpNetworkConnection<string>(new AsciiSerializer());
        }
    }
}
