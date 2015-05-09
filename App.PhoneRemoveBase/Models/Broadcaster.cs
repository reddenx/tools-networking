using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.PhoneRemoveBase.Models
{
    /// <summary>
    /// this is the udp broadcaster object
    /// it has two states, on and off
    /// while on, sends a message to the router broadcast looking for a phone
    /// while off, it does nothing
    /// </summary>
    class Broadcaster
    {
        public Broadcaster() { }
        public void Start() { }
        public void Stop() { }
    }
}
