using Microsoft.Owin;
using Owin;
using SMT.Utilities.Tracing.Core;
using SMT.Utilities.Tracing.Core.HttpHeaderParsing;
using SMT.Utilities.Tracing.Core.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.OwinContext
{
    public static class TraceMiddleware
    {
        public static void Register(IAppBuilder app, string traceLoggingEndpointHostname, int traceLoggingEndpointPort)
        {
            TraceUdpRecorder.TargetTraceLogger(traceLoggingEndpointHostname, traceLoggingEndpointPort);

            Tracer.RegisterContext(new OwinTraceContext());
            app.Use<TraceMiddlewareInternal>();
        }
    }

    public class TraceMiddlewareInternal : OwinMiddleware
    {
        public TraceMiddlewareInternal(OwinMiddleware next) : base(next)
        { }

        public override async Task Invoke(IOwinContext context)
        {
            var trace = HttpHeaderFactory.ContinueTraceFromHeaders(context.Request.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value.FirstOrDefault())));

            if (trace == null)
                Tracer.ClearCurrentTraceAndStartNewOne();

            TraceUdpRecorder.ServerReceive();

            await Next.Invoke(context);

            TraceUdpRecorder.ServerSend();
        }
    }
}
