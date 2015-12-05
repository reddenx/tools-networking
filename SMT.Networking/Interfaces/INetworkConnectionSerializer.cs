using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Interfaces
{
    public interface INetworkConnectionSerializer<T>
    {
        byte[] Serialize(T message);
        T Deserialize(byte[] data);
    }
}
