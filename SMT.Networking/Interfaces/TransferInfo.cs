using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMT.Networking.Interfaces
{
    public class TransferInfo
    {
        public float Progress { get; private set; }
        public float BytesPerSecond { get; private set; }

        internal TransferInfo(float progress, float bytesPerSecond)
        {
            Progress = progress;
            BytesPerSecond = bytesPerSecond;
        }
    }
}
