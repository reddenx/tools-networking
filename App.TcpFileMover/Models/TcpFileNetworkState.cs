using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpFileMover.Models
{
    public enum TcpFileNetworkState
    {
        Ready,
        Sending,
        Receiving,
    }
}
