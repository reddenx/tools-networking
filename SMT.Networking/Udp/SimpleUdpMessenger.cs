using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMT.Networking.Interfaces.SimpleMessaging;

namespace SMT.Networking.Udp
{
    class SimpleUdpMessenger<TMessage> : ISimpleMessenger<TMessage>
    {
        public event EventHandler<TMessage> OnMessageReceived;

        public void Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        public void Send(TMessage message)
        {
            throw new NotImplementedException();
        }


        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
