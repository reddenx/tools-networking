using SMT.Networking.Interfaces.SimpleMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Udp
{
    public class SimpleUdpMessageHost<TMessage> : ISimpleMessageHost<TMessage>
        where TMessage : class
    {
        public SimpleUdpMessageHost()
        {

        }

        public event EventHandler<ISimpleMessenger<TMessage>> OnClientConnect;

        public bool StartHosting(int port)
        {
            throw new NotImplementedException();
        }

        public void StopHosting()
        {
            throw new NotImplementedException();
        }
    }
}
