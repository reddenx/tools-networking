using System;

namespace SMT.Utilities.Tracing.Core
{
    public class Span
    {
        public readonly string TraceId;

        public Span(string traceId)
        {
            TraceId = traceId;
        }

        //TODO span from a span
        //TODO annotate a span or trace
    }
}
