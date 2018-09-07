using SMT.Utilities.Tracing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.HttpClient
{
    public static class HttpHeaderFactory
    {
        public static IEnumerable<KeyValuePair<string, string>> GetTracingHeaders()
        {
            var trace = Tracer.GetCurrentTrace();
            if (trace != null)
            {
                return new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(HttpHeaderTracingConstants.TRACE_ID, trace.TraceId),
                };
            }

            return new KeyValuePair<string, string>[] { };
        }
    }
}
