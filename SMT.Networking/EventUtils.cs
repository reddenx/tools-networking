using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking
{
    static class EventUtils
    {
        public static void RemoveAllListeners<T>(EventHandler<T> handlerList)
        {
            if (handlerList != null)
            {
                handlerList.GetInvocationList().ToList().ForEach(item => handlerList -= (EventHandler<T>)item);
            }
        }

        public static void RemoveAllListeners(EventHandler handlerList)
        {
            handlerList.GetInvocationList().ToList().ForEach(item => handlerList -= (EventHandler)item);
        }

        public static void Call<T>(EventHandler<T> handlers, object sender, T eventArgs)
        {
            if (handlers != null)
            {
                handlers(sender, eventArgs);
            }
        }
    }
}
