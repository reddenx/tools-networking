using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.InputEvents.HardwareEvents
{
    public class MouseEventArgs
    {
        internal readonly int DiffX;
        internal readonly int DiffY;
        internal readonly uint Flags;
        internal readonly uint Data;

        private MouseEventArgs(int diffX, int diffY, uint flags, uint data)
        {
            this.DiffX = diffX;
            this.DiffY = diffY;
            this.Flags = flags;
            this.Data = data;
        }

        public static MouseEventArgs LeftDown()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_LEFTDOWN, 0);
        }

        public static MouseEventArgs LeftUp()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_LEFTUP, 0);
        }

        public static MouseEventArgs RightDown()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_RIGHTDOWN, 0);
        }

        public static MouseEventArgs RightUp()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_RIGHTUP, 0);
        }

        public static MouseEventArgs ScrollUp()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_WHEEL, 120);
        }

        public static MouseEventArgs ScrollDown()
        {
            return new MouseEventArgs(0, 0, MouseEventDefinitions.MOUSEEVENTF_HWHEEL, 120);
        }

        public static MouseEventArgs Move(int diffX, int diffY)
        {
            return new MouseEventArgs(diffX, diffY, MouseEventDefinitions.MOUSEEVENTF_MOVE, 0);
        }
    }
}
