using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.OwinContext
{
    internal class HttpHeaderTracingConstants
    {
        public const string TRACE_ID = "x-b3-traceid";
        public const string SPAN_ID = "x-b3-spanid";
        public const string PARENT_SPAN_ID = "x-b3-parentspanid";
    }
}
