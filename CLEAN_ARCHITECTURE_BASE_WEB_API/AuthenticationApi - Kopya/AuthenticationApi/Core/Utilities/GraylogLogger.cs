using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Utilities
{
    /// <summary>
    /// Graylog GELF protokolü üzerinden loglama yapan sınıf
    /// </summary>
    public class GraylogLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _graylogHost;
        private readonly int _graylogPort;
        private readonly string _source;

        public GraylogLogger(string categoryName, IConfiguration configuration)
        {
            _categoryName = categoryName;
            _graylogHost = configuration["GraylogSettings:Host"] ?? "localhost";
            _graylogPort = int.Parse(configuration["GraylogSettings:Port"] ?? "12201");
            _source = configuration["GraylogSettings:Source"] ?? "authentication-api";
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            try
            {
                var message = formatter(state, exception);
                var gelfMessage = CreateGelfMessage(logLevel, message, exception);
                SendMessageToGraylog(gelfMessage);
            }
            catch
            {
                // Logging should never throw exceptions
            }
        }

        private Dictionary<string, object> CreateGelfMessage(LogLevel logLevel, string message, Exception? exception)
        {
            // GELF formatı (Graylog Extended Log Format)
            // https://docs.graylog.org/docs/gelf
            var gelfMessage = new Dictionary<string, object>
            {
                { "version", "1.1" },
                { "host", _source },
                { "short_message", message },
                { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "level", ConvertLogLevelToSyslogLevel(logLevel) },
                { "_category", _categoryName },
                { "_facility", "AuthenticationApi" }
            };

            if (exception != null)
            {
                gelfMessage.Add("_exception_message", exception.Message);
                gelfMessage.Add("_exception_type", exception.GetType().FullName ?? "Unknown");
                gelfMessage.Add("_exception_stacktrace", exception.StackTrace ?? "");
                gelfMessage.Add("full_message", $"{message}\n{exception}");
            }
            else
            {
                gelfMessage.Add("full_message", message);
            }

            return gelfMessage;
        }

        private void SendMessageToGraylog(Dictionary<string, object> gelfMessage)
        {
            var payload = JsonSerializer.Serialize(gelfMessage);
            var bytes = Encoding.UTF8.GetBytes(payload);

            using var client = new UdpClient();
            client.Connect(_graylogHost, _graylogPort);
            client.Send(bytes, bytes.Length);
        }

        private int ConvertLogLevelToSyslogLevel(LogLevel logLevel)
        {
            // Convert .NET Core log level to Syslog level (used by GELF)
            return logLevel switch
            {
                LogLevel.Critical => 2,    // Critical -> Critical
                LogLevel.Error => 3,       // Error -> Error
                LogLevel.Warning => 4,     // Warning -> Warning
                LogLevel.Information => 6, // Information -> Informational
                LogLevel.Debug => 7,       // Debug -> Debug
                LogLevel.Trace => 7,       // Trace -> Debug
                _ => 7
            };
        }
    }

    public class GraylogLoggerProvider : ILoggerProvider
    {
        private readonly IConfiguration _configuration;

        public GraylogLoggerProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new GraylogLogger(categoryName, _configuration);
        }

        public void Dispose()
        {
            // No resources to dispose
        }
    }
} 