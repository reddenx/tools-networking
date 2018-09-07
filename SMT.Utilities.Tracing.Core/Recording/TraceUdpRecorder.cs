using SMT.Networking.NetworkConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.Core.Recording
{
    public static class TraceUdpRecorder
    {
        private static IUdpNetworkConnection<string> Client = new UdpNetworkConnection<string>(new AsciiSerializer());

        public static void TargetTraceLogger(string hostname, int port)
        {
            Client.Target(hostname, port);
        }

        public static void ClientSend()
        {
            SendTraceMessage("cs");
        }

        public static void ServerReceive()
        {
            SendTraceMessage("sr");
        }

        public static void ServerSend()
        {
            SendTraceMessage("ss");
        }

        public static void ClientReceive()
        {
            SendTraceMessage("cs");
        }

        private static void SendTraceMessage(string eventId)
        {
            var currentTrace = Tracer.GetCurrentTrace();
            if (currentTrace == null)
                return;

            var message = $"[{eventId}:{currentTrace.TraceId.ToString("N")}:{currentTrace.SpanId.ToString("N")}:{currentTrace.ParentSpanId?.ToString("N") ?? string.Empty}:]";

            Client.Send(message);
        }
    }
}
