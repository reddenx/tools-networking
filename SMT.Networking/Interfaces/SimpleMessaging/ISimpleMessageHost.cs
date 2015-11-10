using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces.SimpleMessaging
{
    [Obsolete("Use TcpNetworkConnection")]
    public interface ISimpleMessageHost<TMessage>
        where TMessage : class
    {
        event EventHandler<ISimpleMessenger<TMessage>> OnClientConnect; //host shouldn't care about client's message type? maybe I should rethink the syntax... on second thought, it'd be weird if different clients had different message types...

        bool StartHosting(int port);
        void StopHosting();
    }
}
