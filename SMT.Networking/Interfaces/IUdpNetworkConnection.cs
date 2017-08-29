using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface IUdpNetworkConnection<T> : INetworkConnection<T>
    {
        bool StartListening(int port);
        void StopListening();

        void Target(string hostname, int port);
        void Target(string connectionString);
        void Target(IPEndPoint remoteEndpoint);
        void Untarget();
    }
}
