using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface INetworkConnectionListener<T>
    {
        event EventHandler<INetworkConnection<T>> OnClientConnected;

        void Start(int port);
        void Stop();
    }
}
