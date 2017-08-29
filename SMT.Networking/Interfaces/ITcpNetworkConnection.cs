using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface ITcpNetworkConnection<T> : INetworkConnection<T>
    {
        event EventHandler<IPEndPoint> OnConnected;
        event EventHandler OnDisconnected;
        bool Connected { get; }
        void Disconnect();

        void Connect(string hostname, int port);
        void Connect(string connectionString);
        void Connect(IPEndPoint remoteEndpoint);
    }
}
