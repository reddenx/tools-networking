using SMT.Utilities.InputEvents.HardwareEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace App.TestingGrounds
{
    static class MouseInputTesting
    {
        public static void Run()
        {
            var mouse = new MouseEventRunner();

            //mouse.DoEvent(MouseEventArgs.LeftDown());

            mouse.DoEvent(MouseEventArgs.Set(.5f, .5f));

            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Set(0, 0));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Set(100, 100));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Set(800, 800));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Set(1920, 1080));

            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(100, 0));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(-100, 0));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(0, 100));
            //Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(0, -100));

            //Thread.Sleep(500);
            //mouse.DoEvent(MouseEventArgs.LeftUp());
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
    }
}
