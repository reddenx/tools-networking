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
        event EventHandler<IPEndPoint> OnConnected;
        event EventHandler<T> OnMessageSent;
        event EventHandler<Exception> OnError;

        bool Connected { get; }
        string HostName { get; }
        int Port { get; }
        
        void Disconnect();
        void Connect(string hostname, int port);
        void Connect(string connectionString);
        void Send(T message);
    }
}
