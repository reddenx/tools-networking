using SMT.Utilities.Tracing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.Core.HttpHeaderParsing
{
    public static class HttpHeaderFactory
    {
        public static Span ContinueTraceFromHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            //ensure the base minimum of trace values are present
            if (headers.Any(h => h.Key.Equals(HttpHeaderTracingConstants.SPAN_ID, StringComparison.InvariantCultureIgnoreCase)) && headers.Any(h => h.Key.Equals(HttpHeaderTracingConstants.TRACE_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                var successfulParsing = true;
                Guid traceId;
                successfulParsing &= Guid.TryParse(headers.First(h => h.Key.Equals(HttpHeaderTracingConstants.TRACE_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out traceId);

                Guid spanId;
                successfulParsing &= Guid.TryParse(headers.First(h => h.Key.Equals(HttpHeaderTracingConstants.SPAN_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out spanId);

                //short circuit if id parsing was not successful from header values
                if (!successfulParsing)
                    return null;

                Guid parentSpanId = Guid.Empty;
                bool includeParentSpanId = false;
                if (headers.Any(h => h.Key.Equals(HttpHeaderTracingConstants.PARENT_SPAN_ID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    includeParentSpanId = Guid.TryParse(headers.First(h => h.Key.Equals(HttpHeaderTracingConstants.PARENT_SPAN_ID, StringComparison.InvariantCultureIgnoreCase)).Value, out parentSpanId);
                }

                //finally attempt set the trace
                if (includeParentSpanId)
                    return Tracer.ContinueTrace(traceId, spanId, parentSpanId);
                else
                    return Tracer.ContinueTrace(traceId, spanId, null);
            }

            return null;
        }

        public static void AttachTracingHeaders(HttpWebRequest request)
        {
            //short circuit if there's no trace
            var trace = Tracer.SplitOffChildTrace();
            if (trace == null)
                return;

            //short circuit if there's an overlap
            var headers = BuildTracingHeaders(trace);
            var currentHeaderValues = request.Headers.AllKeys;
            if (headers.Any(traceHeader => currentHeaderValues.Contains(traceHeader.Key)))
                return;

            //attach headers
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key.ToLower(), header.Value.ToLower());
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> BuildTracingHeaders(Span trace)
        {
            if (trace != null)
            {
                if (!trace.ParentSpanId.HasValue)
                    return new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>(HttpHeaderTracingConstants.TRACE_ID, trace.TraceId.ToString("N")),
                        new KeyValuePair<string, string>(HttpHeaderTracingConstants.SPAN_ID, trace.SpanId.ToString("N")),
                    };
                else
                    return new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>(HttpHeaderTracingConstants.TRACE_ID, trace.TraceId.ToString("N")),
                        new KeyValuePair<string, string>(HttpHeaderTracingConstants.SPAN_ID, trace.SpanId.ToString("N")),
                        new KeyValuePair<string, string>(HttpHeaderTracingConstants.PARENT_SPAN_ID, trace.ParentSpanId.Value.ToString("N")),
                    };
            }

            return new KeyValuePair<string, string>[0];
        }
    }
}
