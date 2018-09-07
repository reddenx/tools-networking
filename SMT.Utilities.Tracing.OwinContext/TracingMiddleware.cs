using Microsoft.Owin;
using Owin;
using SMT.Utilities.Tracing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.OwinContext
{
    public static class TraceMiddleware
    {
        public static void Register(IAppBuilder app)
        {
            Tracer.RegisterContext(new OwinTraceContext());
            app.Use<TraceMiddlewareInternal>();
        }
    }

    internal class TraceMiddlewareInternal : OwinMiddleware
    {
        public TraceMiddlewareInternal(OwinMiddleware next) : base(next)
        { }

        public override async Task Invoke(IOwinContext context)
        {
            //if there's a tracing header, continue the trace
            Span span= null;
            if (context.Request.Headers.ContainsKey(HttpHeaderTracingConstants.TRACE_ID))
            {
                var traceId = context.Request.Headers.Get(HttpHeaderTracingConstants.TRACE_ID);
                if (!string.IsNullOrWhiteSpace(traceId))
                {
                    span = new Span(traceId);
                    Tracer.ContinueTrace(span);
                }
            }

            //fallthrough, if there's no trace, start one
            if (span == null)
            {
                Tracer.StartNewTrace();
                span = Tracer.GetCurrentTrace();
            }

            await Next.Invoke(context);
        }
    }
}
