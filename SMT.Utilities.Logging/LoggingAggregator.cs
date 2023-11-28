using System;

namespace SMT.Utilities.Logging
{
    public interface ILoggingAggregator
    {
        void RecordEntry(TraceFragment fragment);
        Trace GetFullTrace(string traceId);
    }
    public class TraceFragment
    {

    }
    public class Trace
    {
        public string Id { get; }
        public Span[] Spans { get; }
    }
    public class Span
    {
        public string Id { get; }
        public string TraceId { get; }
        public Span[] Spans { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
    }
}