using System;

namespace SMT.Networking.NetworkConnection
{
    /// <summary>
    /// factory for building network connections
    /// </summary>
    public static class NetworkConnectionFactory
    {
        public static ITcpNetworkConnection<T> GetTcpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpNetworkConnection<T>(serializer);
        }

        public static ITcpNetworkConnection<T> GetTcpNetworkConnection<T>(Func<T, byte[]> serialize, Func<byte[], T> deserialize)
        {
            return new TcpNetworkConnection<T>(new AnonymousSerializer<T>(deserialize, serialize));
        }

        public static ITcpNetworkConnection<string> GetTcpNetworkConnection()
        {
            return new TcpNetworkConnection<string>(new AsciiSerializer());
        }

        public static ITcpNetworkConnectionListener<T> GetTcpNetworkConnectionListener<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new TcpTcpNetworkConnectionListener<T>(serializer);
        }

        public static ITcpNetworkConnectionListener<string> GetTcpNetworkConnectionListener()
        {
            return new TcpTcpNetworkConnectionListener<string>(new AsciiSerializer());
        }

        //unfinished component
        public static IUdpNetworkConnection<T> GetUdpNetworkConnection<T>(INetworkConnectionSerializer<T> serializer)
        {
            return new UdpNetworkConnection<T>(serializer);
        }

        public static IUdpNetworkConnection<string> GetUdpNetworkConnection()
        {
            throw new NotImplementedException("unfinished component");
            return new UdpNetworkConnection<string>(new AsciiSerializer());
        }
    }
}
