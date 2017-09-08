using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface ITcpNetworkConnectionListener<T> : IDisposable
    {
        event EventHandler<ITcpNetworkConnection<T>> OnClientConnected;

        void Start(int port);
        void Stop();
    }
}
