using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace SMT.Utilities.Logging
{
    public interface ITraceLogger : ILogger
    {
        Span GetCurrentTrace();
        Span StartNewTrace();
        Span SetCurrentSpan(Span trace);
        void LogTraceEvent(Span trace, TraceEvents traceEvent);

        void Error(string message, Exception e, params object[] serializableData);
        void Error(string message, params object[] serializableData);
        void Warn(string message, params object[] serializableData);
        void Warn(string message, Exception e, params object[] serializableData);
        void Info(string message, params object[] serializableData);
        void Debug(string message, params object[] serializableData);
    }
    public enum TraceEvents
    {
        Annotate, //currently left unimplemented
        ClientSend,
        ServerReceive,
        ServerSend,
        ClientReceive,
    }
    public class TraceLoggerConfiguration
    {
        public LogLevel CurrentLogLevel { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Secret { get; set; }
    }
    
    public class TraceLogger : ITraceLogger
    {
        private readonly ITraceContext _traceContext;
        private readonly ITraceLogWriter _logWriter;
        private readonly Func<TraceLoggerConfiguration> _getConfig;

        public TraceLogger(ITraceContext traceContext, ITraceLogWriter logWriter, Func<TraceLoggerConfiguration> getConfig)
        {
            _traceContext = traceContext;
            _logWriter = logWriter;
            _getConfig = getConfig;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disposer();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _getConfig()?.CurrentLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            //get trace
            var trace = GetCurrentTrace();

            var message = $"{eventId.Id}-{eventId.Name}: {formatter(state, exception)}";

            var config = _getConfig();
            if (config.Port != _logWriter.Port || config.Host != _logWriter.Host)
                _logWriter.Target(config.Host, config.Port);

            _logWriter.SendLog(trace, logLevel, message, exception?.StackTrace, exception?.Message);
        }

        private void LogWithData(LogLevel level, string message, Exception e, object[] data)
        {
            if (!IsEnabled(level))
                return;

            var trace = GetCurrentTrace();

            string dataString = "";
            try
            {
                dataString = JsonSerializer.Serialize(data);
            }
            catch
            {
                var types = string.Join(", ", data.Select(d => d.GetType()));
                dataString = $"serialization failed: {types}";
            }

            var config = _getConfig();
            if (config.Port != _logWriter.Port || config.Host != _logWriter.Host)
                _logWriter.Target(config.Host, config.Port);

            _logWriter.SendLog(trace, level, message, e?.StackTrace, dataString);
        }

        public void LogTraceEvent(Span trace, TraceEvents traceEvent)
        {
            if (!IsEnabled(LogLevel.Trace))
                return;

            var config = _getConfig();
            if (config.Port != _logWriter.Port || config.Host != _logWriter.Host)
                _logWriter.Target(config.Host, config.Port);

            _logWriter.SendTrace(trace, traceEvent);
        }

        public void Error(string message, params object[] serializableData) => LogWithData(LogLevel.Error, message, null, serializableData);
        public void Error(string message, Exception e, params object[] serializableData) => LogWithData(LogLevel.Error, message, e, serializableData);
        public void Warn(string message, params object[] serializableData) => LogWithData(LogLevel.Warning, message, null, serializableData);
        public void Warn(string message, Exception e, params object[] serializableData) => LogWithData(LogLevel.Warning, message, e, serializableData);
        public void Info(string message, params object[] serializableData) => LogWithData(LogLevel.Information, message, null, serializableData);
        public void Debug(string message, params object[] serializableData) => LogWithData(LogLevel.Debug, message, null, serializableData);

        public Span GetCurrentTrace()
        {
            var trace = _traceContext.GetCurrentTraceFromContext();
            if (trace != null)
                return trace;
            return StartNewTrace();
        }

        public Span SetCurrentSpan(Span trace)
        {
            _traceContext.SetCurrentTraceToContext(trace);
            return GetCurrentTrace();
        }

        public Span StartNewTrace()
        {
            _traceContext.SetCurrentTraceToContext(new Span(Guid.NewGuid(), Guid.NewGuid()));
            return _traceContext.GetCurrentTraceFromContext();
        }

        class Disposer : IDisposable
        {
            public void Dispose()
            {
                //TODO figure out what this is supposed to do
            }
        }
    }

    public class TraceLoggingProvider : ILoggerProvider
    {
        private IOptionsMonitor<TraceLoggerConfiguration> _config;
        private readonly ITraceContext _traceContext;
        private readonly ITraceLogWriter _loggingWriter;

        public TraceLoggingProvider(IOptionsMonitor<TraceLoggerConfiguration> config, ITraceContext traceContext, ITraceLogWriter loggingWriter)
        {
            _config = config;
            _traceContext = traceContext;
            _loggingWriter = loggingWriter;

            loggingWriter.Target(_config.CurrentValue.Host, _config.CurrentValue.Port);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new TraceLogger(_traceContext, _loggingWriter, () => _config.CurrentValue);
            return logger;
        }

        public void Dispose()
        {
        }
    }
}