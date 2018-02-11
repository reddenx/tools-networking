using System;

namespace SMT.Utilities.Tracing.Core
{
    public interface ITraceContext
    {
        Span GetCurrentTraceFromContext();
        void SetCurrentTraceToContext(Span currentContextSpan);
    }
}
