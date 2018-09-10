using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Utility
{
    internal static class EventExtensions
    {
        public static void SafeExecuteAsync<T>(this EventHandler<T> handler, object sender, T args)
        {
            if (handler != null)
            {
                handler.BeginInvoke(sender, args, AsyncCallback<T>, null);
            }
        }

        private static void AsyncCallback<T>(IAsyncResult result)
        {
            try
            {
                ((result as AsyncResult)?.AsyncDelegate as EventHandler<T>)?.EndInvoke(result);
            }
            catch { } //for complete shit hit the fan scenario
        }

        public static void SafeExecuteAsync(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler.BeginInvoke(sender, EventArgs.Empty, AsyncCallback, null);
            }
        }

        private static void AsyncCallback(IAsyncResult result)
        {
            try
            {
                ((result as AsyncResult)?.AsyncDelegate as EventHandler)?.EndInvoke(result);
            }
            catch { }
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
