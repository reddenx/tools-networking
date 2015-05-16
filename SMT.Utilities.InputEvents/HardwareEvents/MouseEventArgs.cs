using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.InputEvents.HardwareEvents
{
    public class MouseEventArgs
    {
        public readonly uint DiffX;
        public readonly uint DiffY;
        public readonly uint Flags;
        public readonly uint Data;

        public MouseEventArgs(uint diffX, uint diffY, uint flags, uint data)
        {
            this.DiffX = diffX;
            this.DiffY = diffY;
            this.Flags = flags;
            this.Data = data;
        }

        public static MouseEventArgs LeftDown();
        public static MouseEventArgs LeftUp();
        public static MouseEventArgs RightDown();
        public static MouseEventArgs RightUp();
        public static MouseEventArgs ScrollUp();
        public static MouseEventArgs ScrollDown();        
        public static MouseEventArgs Move(
    }
}
