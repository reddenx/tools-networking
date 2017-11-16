using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace SMT.Utilities.WebDiagnostics
{
    public class DiagnosticsHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            if(context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(HandleSocket);
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.Write(DIAGNOSTICS_COMPILED_UI);
                context.Response.ContentType = "text/html";
            }
        }

        private async Task HandleSocket(AspNetWebSocketContext context)
        {
            while (context.WebSocket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var result = await context.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
            }
        }

        private const string DIAGNOSTICS_COMPILED_UI = @"";
    }
}
