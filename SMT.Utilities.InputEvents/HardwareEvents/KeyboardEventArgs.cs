using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMT.Utilities.InputEvents.HardwareEvents
{
    public class KeyboardEventArgs
    {
        public Keys Key { get { return (Keys)VirtualKey; } }
        public int VirtualKey { get; private set; }
        public int ScanKey { get; private set; }

        public KeyboardEventArgs(Keys key, int scanKey)
            : this((int)key, scanKey)
        { }

        public KeyboardEventArgs(int virtualKey, int scanKey)
        {
            this.VirtualKey = virtualKey;
            this.ScanKey = scanKey;
        }
    }
}
