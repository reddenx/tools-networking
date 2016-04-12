using SMT.Utilities.InputEvents.HardwareEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.TestingGrounds
{
    static class MouseInputTesting
    {
        public static void Run()
        {
            var mouse = new MouseEventRunner();

            mouse.DoEvent(MouseEventArgs.LeftDown());


            Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(100, 0));
            Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(-100, 0));
            Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(0, 100));
            Thread.Sleep(500); mouse.DoEvent(MouseEventArgs.Move(0, -100));

            Thread.Sleep(500);
            mouse.DoEvent(MouseEventArgs.LeftUp());
        }
    }
}
