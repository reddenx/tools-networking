using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SMT.Utilities.Logging
{
    public interface ITraceLogWriter
    {
        int Port { get; }
        string Host { get; }

        void SendLog(Span trace, LogLevel logLevel, string message, string stack, string data);
        void SendTrace(Span trace, TraceEvents eventName);
        void Target(string host, int? port);
    }
    public class UdpTraceLogWriter : ITraceLogWriter
    {
        private readonly UdpNetworkSender _client;

        public string Host => _client.HostName;
        public int Port => _client.Port;

        public UdpTraceLogWriter()
        {
            _client = new UdpNetworkSender();
        }
        public void Target(string host, int? port)
        {
            if (string.IsNullOrEmpty(host) || !port.HasValue)
                _client.Untarget();
            else
                _client.Target(host, port.Value);
        }
        public void SendLog(Span trace, LogLevel logLevel, string message, string stack, string data)
        {
            var logEntry = new LogEntryDto()
            {
                TraceId = trace?.TraceId,
                SpanId = trace?.SpanId,
                ParentSpanId = trace?.ParentSpanId,
                LogLevel = logLevel.ToString(),
                Message = message,
                Stack = stack,
                Data = data,
                ServerTimestamp = DateTime.UtcNow.ToString("o"),
            };
            var json = JsonSerializer.Serialize(logEntry);
            _client.Send(json);
        }

        public void SendTrace(Span trace, TraceEvents eventName)
        {
            var logEntry = new LogEntryDto()
            {
                TraceId = trace?.TraceId,
                SpanId = trace?.SpanId,
                ParentSpanId = trace?.ParentSpanId,
                LogLevel = LogLevel.Trace.ToString(),
                Message = eventName.ToString(),
                Stack = null,
                Data = null,
                ServerTimestamp = DateTime.UtcNow.ToString("o"),
            };
            var json = JsonSerializer.Serialize(logEntry);
            _client.Send(json);
        }

        private class LogEntryDto
        {
            public string LogLevel { get; set; }
            public string Message { get; set; }
            public string Stack { get; set; }
            public string Data { get; set; }
            public Guid? TraceId { get; set; }
            public Guid? SpanId { get; set; }
            public Guid? ParentSpanId { get; set; }
            public string ServerTimestamp { get; set; }
        }
    }

}