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

        public static Span SplitOffChildTrace()
        {
            var trace = GetCurrentTrace();
            if (trace == null)
                return null;

            return new Span(trace.TraceId, Guid.NewGuid(), trace.SpanId);
        }

        public static Span GetCurrentTrace()
        {
            return TraceContext?.GetCurrentTraceFromContext();
        }

        public static Span ClearCurrentTraceAndStartNewOne()
        {
            var newTrace = new Span(Guid.NewGuid(), Guid.NewGuid(), null);
            TraceContext?.SetCurrentTraceToContext(newTrace);
            return GetCurrentTrace();
        }

        public static Span ContinueTrace(Guid traceId, Guid spanId, Guid? parentSpanId)
        {
            TraceContext?.SetCurrentTraceToContext(new Span(traceId, spanId, parentSpanId));
            return GetCurrentTrace();
        }
    }
}
