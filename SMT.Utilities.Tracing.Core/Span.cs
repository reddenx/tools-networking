using System;

namespace SMT.Utilities.Tracing.Core
{
    public class Span
    {
        public readonly Guid TraceId;
        public readonly Guid SpanId;
        public readonly Guid? ParentSpanId;

        internal Span(Guid traceId, Guid spanId, Guid? parentSpanId = null)
        {
            this.TraceId = traceId;
            this.SpanId = spanId;
            this.ParentSpanId = parentSpanId;
        }
    }
}
