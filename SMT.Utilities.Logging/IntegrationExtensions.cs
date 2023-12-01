using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SMT.Utilities.Logging
{
    public static class IntegrationExtensions
    {
        /// <summary>
        /// sets up dependencies for tracing and logging, optionally pairs with UseSmtTracingMiddleware
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultLogLevel"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static void AddSmtLogging(this IServiceCollection services, Action<TraceLoggerConfiguration> config)
        {
            services.Configure(config);

            services.AddHttpContextAccessor();
            services.AddSingleton<ILoggerProvider, TraceLoggingProvider>(); //integrates into normal log pipeline
            services.AddSingleton<TraceLoggingProvider>(); //needed for custom logging
            services.AddSingleton<ITraceLogger>(p => p.GetService<TraceLoggingProvider>().CreateLogger("Tracing") as TraceLogger);
            services.AddSingleton<ITraceLogWriter, UdpTraceLogWriter>();
            services.AddSingleton<ITraceContext, AspHttpRequestTraceContext>();
        }
        

        /// <summary>
        /// registers logging endpoint services, pairs with UseSmtLoggingEndpoints
        /// </summary>
        /// <param name="services"></param>
        /// <param name="loggingBaseUrl"></param>
        /// <returns></returns>
        public static void AddSmtLoggingEndpoints(this IServiceCollection services, Action<LoggingEndpointConfiguration> config)
        {
            services.Configure(config);
        }
        public class LoggingEndpointConfiguration
        {
            public string BaseUrl { get; set; }
        }
       

        public static void UseSmtTracingHeaderInterpreter(this IApplicationBuilder app)
        {
            app.Use(TracingMiddlewareHandler);
        }

        private static async Task TracingMiddlewareHandler(HttpContext context, RequestDelegate next)
        {
            var headers = context.Request.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value)).ToArray();
            var span = Span.FromHeaders(headers);

            var logger = context.RequestServices.GetService<ITraceLogger>();
            if (span == null)
            {
                logger.GetCurrentTrace(); //creates a trace if one doesn't exist
                await next(context);
                return;
            }

            if (logger != null)
                logger.SetCurrentSpan(span);

            logger.LogTraceEvent(span, TraceEvents.ServerReceive);
            await next(context);
            logger.LogTraceEvent(span, TraceEvents.ServerSend);
        }

        /// <summary>
        /// sets up logging endpoint routes and handlers
        /// </summary>
        /// <param name="app"></param>
        public static void UseSmtLoggingEndpoints(this WebApplication app)
        {
            var endpointConfig = app.Services.GetService<IOptions<LoggingEndpointConfiguration>>()?.Value;
            var loggingConfig = app.Services.GetService<IOptionsMonitor<TraceLoggerConfiguration>>();


            app.MapPut($"/{endpointConfig.BaseUrl}", async context =>
            {
                var authHeader = context.Request.Headers.Authorization;
                if (authHeader.FirstOrDefault() != loggingConfig.CurrentValue.Secret)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                var dto = await context.Request.ReadFromJsonAsync<SmtLoggingConfigurationDto>();

                loggingConfig.CurrentValue.CurrentLogLevel = dto.CurrentLogLevel;
                loggingConfig.CurrentValue.Port = dto.Port;
                loggingConfig.CurrentValue.Host = dto.Hostname;

                context.Response.StatusCode = 204;

                app.Logger.Log(LogLevel.Information, "Log settings updated via api");
            });

            app.MapGet($"/{endpointConfig.BaseUrl}", async context =>
            {
                var authHeader = context.Request.Headers.Authorization;
                if (authHeader.FirstOrDefault() != loggingConfig.CurrentValue.Secret)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new SmtLoggingConfigurationDto
                {
                    CurrentLogLevel = loggingConfig.CurrentValue.CurrentLogLevel,
                    Hostname = loggingConfig.CurrentValue.Host,
                    Port = loggingConfig.CurrentValue.Port
                });
                app.Logger.Log(LogLevel.Information, "Log settings retrieved via api");
            });
        }
        public class SmtLoggingConfigurationDto
        {
            public LogLevel CurrentLogLevel { get; set; }
            public string Hostname { get; set; }
            public int? Port { get; set; }
        }
    }
}