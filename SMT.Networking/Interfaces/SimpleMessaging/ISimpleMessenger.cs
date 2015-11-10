﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces.SimpleMessaging
{
    [Obsolete("Use TcpNetworkConnection")]
    public interface ISimpleMessenger<TMessage>
        where TMessage : class
    {
        event EventHandler<TMessage> OnMessageReceived;

        void Connect(string host, int port);
        void Send(TMessage message);
        void Disconnect();
    }
}
