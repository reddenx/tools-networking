using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Utilities.WebDiagnostics
{
    internal static class SocketManager
    {
        private static readonly List<WebSocket> Sockets = new List<WebSocket>();

        internal static void Register(WebSocket socket)
        {
            CleanupSockets();
            lock (Sockets)
            {
                Sockets.Add(socket);
            }
        }

        internal static void UnRegister(WebSocket socket)
        {
            CleanupSockets();
            lock (Sockets)
            {
                Sockets.Remove(socket);
            }
        }

        internal static void Clear()
        {
            var closeTasks = new List<Task>();
            CleanupSockets();
            lock (Sockets)
            {
                foreach (var socket in Sockets)
                {
                    closeTasks.Add(socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "server is clearing all connections", CancellationToken.None));
                }
                Sockets.Clear();
            }
            
            foreach (var task in closeTasks)
            {
                task.Wait();
            }
        }

        private static void CleanupSockets()
        {
            lock (Sockets)
            {
                Sockets.RemoveAll(socket =>
                {
                    try
                    {
                        return socket.State != WebSocketState.Open;
                    }
                    catch (ObjectDisposedException)
                    {
                        return true;
                    }
                });
            }
        }
    }
}
