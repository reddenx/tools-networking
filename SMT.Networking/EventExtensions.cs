using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking
{
    internal static class EventExtensions
    {
        public static void SafeExecute(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void SafeExecute<T>(this EventHandler<T> handler, object sender, T args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public static void RemoveAllListeners<T>(this EventHandler<T> handlerList)
        {
            if (handlerList != null)
            {
                handlerList.GetInvocationList().ToList().ForEach(item => handlerList -= (EventHandler<T>)item);
            }
        }

        public static void RemoveAllListeners(this EventHandler handlerList)
        {
            if (handlerList != null)
            {
                handlerList.GetInvocationList().ToList().ForEach(item => handlerList -= (EventHandler)item);
            }
        }
    }
}
