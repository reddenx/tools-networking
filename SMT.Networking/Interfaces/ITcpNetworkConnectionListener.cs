using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface ITcpNetworkConnectionListener<T>
    {
        event EventHandler<ITcpNetworkConnection<T>> OnClientConnected;

        void Start(int port);
        void Stop();
    }
}
