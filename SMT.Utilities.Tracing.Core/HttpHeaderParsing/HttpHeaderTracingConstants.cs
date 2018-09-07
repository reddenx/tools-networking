using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.Tracing.Core.HttpHeaderParsing
{
    internal class HttpHeaderTracingConstants
    {
        //primary system trace id that strings all values together
        public const string TRACE_ID = "x-smt-01-traceid";

        //this current span's id
        public const string SPAN_ID = "x-smt-01-spanid";

        //this current span's parent span id
        public const string PARENT_SPAN_ID = "x-smt-01-parentspanid";
    }
}
