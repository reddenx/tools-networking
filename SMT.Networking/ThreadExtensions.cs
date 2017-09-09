using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking
{
    internal static class ThreadExtensions
    {
        public static Thread StartBackground(this Thread thread)
        {
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }

        public static Thread StartBackground(this Thread thread, object state)
        {
            thread.IsBackground = true;
            thread.Start(state);
            return thread;
        }

        public static void DisposeOfThread(this Thread thread)
        {
            //give it every chance to cleanly terminate, then abort

            if (thread == null) return;
            if (!thread.IsAlive) return;
            if (thread.Join(100)) return;

            thread.Abort();
        }
    }
}
