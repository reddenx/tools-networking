using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace SMT.Utilities.Logging
{
    public class Span
    {
        public readonly Guid TraceId;
        public readonly Guid SpanId;
        public readonly Guid? ParentSpanId;

        //primary system trace id that strings all values together
        private const string TRACE_ID = "x-smt-01-traceid";
        //this current span's id
        private const string SPAN_ID = "x-smt-01-spanid";
        //this current span's parent span id
        private const string PARENT_SPAN_ID = "x-smt-01-parentspanid";

        public KeyValuePair<string, string>[] ToHeaders()
        {
            if (!ParentSpanId.HasValue)
                return new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(TRACE_ID, TraceId.ToString("N")),
                    new KeyValuePair<string, string>(SPAN_ID, SpanId.ToString("N")),
                };
            else
                return new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(TRACE_ID, TraceId.ToString("N")),
                    new KeyValuePair<string, string>(SPAN_ID, SpanId.ToString("N")),
                    new KeyValuePair<string, string>(PARENT_SPAN_ID, ParentSpanId.Value.ToString("N")),
                };
        }

        public static Span FromHeaders(KeyValuePair<string, string>[] headers)
        {
            if (headers.Any(h => h.Key.Equals(SPAN_ID, StringComparison.InvariantCultureIgnoreCase)) && headers.Any(h => h.Key.Equals(TRACE_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                var successfulParsing = true;
                Guid traceId;
                successfulParsing &= Guid.TryParse(headers.First(h => h.Key.Equals(TRACE_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out traceId);

                Guid spanId;
                successfulParsing &= Guid.TryParse(headers.First(h => h.Key.Equals(SPAN_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out spanId);

                //short circuit if id parsing was not successful from header values
                if (!successfulParsing)
                    return null;

                Guid parentSpanId = Guid.Empty;
                bool includeParentSpanId = false;
                if (headers.Any(h => h.Key.Equals(PARENT_SPAN_ID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    includeParentSpanId = Guid.TryParse(headers.First(h => h.Key.Equals(PARENT_SPAN_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out parentSpanId);
                }

                //finally attempt set the trace
                if (includeParentSpanId)
                    return new Span(traceId, spanId, parentSpanId);
                else
                    return new Span(traceId, spanId, null);
            }

            return null;
        }

        public Span SplitOffSpan()
        {
            return new Span(this.TraceId, Guid.NewGuid(), this.SpanId);
        }

        internal Span(Guid traceId, Guid spanId, Guid? parentSpanId = null)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentSpanId = parentSpanId;
        }
    }

    public interface ITraceContext
    {
        Span GetCurrentTraceFromContext();
        void SetCurrentTraceToContext(Span trace);
    }

    public class AspHttpRequestTraceContext : ITraceContext
    {
        private readonly IHttpContextAccessor _context;
        private const string SMT_TRACING_KEY = @"smt-tracing-key";

        public AspHttpRequestTraceContext(IHttpContextAccessor context)
        {
            _context = context;
        }

        public Span GetCurrentTraceFromContext()
        {
            var context = _context.HttpContext;
            if (context == null)
                return null;

            if (!context.Items.TryGetValue(SMT_TRACING_KEY, out var spanBlob))
            {
                return null;
            }
            return spanBlob as Span;
        }

        public void SetCurrentTraceToContext(Span trace)
        {
            var context = _context.HttpContext;
            if (context == null)
                return;

            lock (context.Items)
            {
                if (context.Items.ContainsKey(SMT_TRACING_KEY))
                    context.Items.Remove(SMT_TRACING_KEY);
                context.Items.Add(SMT_TRACING_KEY, trace);
            }
        }
    }

    public class StaticTraceContext : ITraceContext
    {
        private static Span _trace = null;
        
        public Span GetCurrentTraceFromContext()
        {
            return _trace;
        }

        public void SetCurrentTraceToContext(Span trace)
        {
            _trace = trace;
        }
    }

    public class ThreadContext : ITraceContext
    {
        private readonly ThreadLocal<Span> _span;

        public ThreadContext()
        {
            _span = new ThreadLocal<Span>();
        }

        public Span GetCurrentTraceFromContext() => _span.Value;
        public void SetCurrentTraceToContext(Span trace) => _span.Value = trace;
    }
}