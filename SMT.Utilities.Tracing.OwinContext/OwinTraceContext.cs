using SMT.Utilities.Tracing.Core;
using System;
using System.Reflection;
using Owin;
using Microsoft.Owin;

namespace SMT.Utilities.Tracing.OwinContext
{
    public class OwinTraceContext : ITraceContext
    {
        private static readonly string HTTP_CONTEXT_KEY = $"smt-tracing-context-key-{Assembly.GetAssembly(typeof(OwinTraceContext))?.GetName()?.Version?.ToString() ?? "UNVERSIONED"}";

        public Span GetCurrentTraceFromContext()
        {
            if (System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsKey(HTTP_CONTEXT_KEY))
            {
                return System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values[HTTP_CONTEXT_KEY] as Span;
            }
            else
            {
                return null;
            }
        }

        public void SetCurrentTraceToContext(Span currentContextSpan)
        {
            System.Web.HttpContext.Current?.Request?.RequestContext?.RouteData.Values.Add(HTTP_CONTEXT_KEY, currentContextSpan);
        }
    }
}
