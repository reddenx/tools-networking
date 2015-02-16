using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpFileMover.Models
{
    public class TransferState
    {
        public float Progress { get; set; }
        public float BytesPerSecond { get; set; }
    }
}
