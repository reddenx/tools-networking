using System;

namespace SMT.Utilities.Tracing.Core
{
    public static class Tracer
    {
        private static ITraceContext TraceContext;

        public static void RegisterContext(ITraceContext traceContext)
        {
            TraceContext = traceContext;
        }

        public static Span GetCurrentTrace()
        {
            return TraceContext.GetCurrentTraceFromContext();
        }

        public static void StartNewTrace()
        {
            var newTrace = new Span(Guid.NewGuid().ToString("N"));
            TraceContext.SetCurrentTraceToContext(newTrace);
        }

        public static void ContinueTrace(Span trace)
        {
            TraceContext.SetCurrentTraceToContext(trace);
        }
    }
}
