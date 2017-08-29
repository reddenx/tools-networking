using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface INetworkConnection<T> : IDisposable
    {
        event EventHandler<T> OnMessageReceived;
        event EventHandler<T> OnMessageSent;
        event EventHandler<Exception> OnError;

        string HostName { get; }
        int Port { get; }
        
        void Send(T message);
    }
}
