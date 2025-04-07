using LogLibrary.Configuration.Settings;
using LogLibrary.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary.Services.Concrete
{
    /// <summary>
    /// Graylog service to send logs in GELF format over TCP
    /// </summary>
    public class GraylogService
    {
        private readonly LogSettings _logSettings;
        private readonly ILogger<GraylogService> _logger;

        /// <summary>
        /// GraylogService constructor
        /// </summary>
        public GraylogService(
            IOptions<LogSettings> logSettingsOptions,
            ILogger<GraylogService> logger)
        {
            _logSettings = logSettingsOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Send a log entry to Graylog in GELF format
        /// </summary>
        public async Task SendLogAsync(LogEntry logEntry)
        {
            if (!_logSettings.EnableGraylog)
                return;

            try
            {
                var gelfMessage = ConvertToGelf(logEntry);
                string gelfJson = JsonConvert.SerializeObject(gelfMessage);
                await SendGelfMessageAsync(gelfJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending log to Graylog: {ex.Message}");
            }
        }

        /// <summary>
        /// Convert LogEntry to GELF format
        /// </summary>
        private GelfMessage ConvertToGelf(LogEntry logEntry)
        {
            var gelfMessage = new GelfMessage
            {
                Version = "1.1",
                Host = Environment.MachineName,
                ShortMessage = logEntry.Message,
                Timestamp = ToUnixTimestamp(logEntry.Timestamp),
                Level = ConvertLogLevelToSyslog(logEntry.Level),
            };

            // Add additional fields with proper GELF naming (prefixed with _)
            gelfMessage.AdditionalFields.Add("_source", logEntry.Source);
            gelfMessage.AdditionalFields.Add("_application", logEntry.ApplicationName);
            gelfMessage.AdditionalFields.Add("_environment", logEntry.Environment);
            
            // Add user context if available
            if (!string.IsNullOrEmpty(logEntry.UserId))
                gelfMessage.AdditionalFields.Add("_user_id", logEntry.UserId);
            
            if (!string.IsNullOrEmpty(logEntry.UserName))
                gelfMessage.AdditionalFields.Add("_user_name", logEntry.UserName);
            
            if (!string.IsNullOrEmpty(logEntry.UserEmail))
                gelfMessage.AdditionalFields.Add("_user_email", logEntry.UserEmail);
            
            if (!string.IsNullOrEmpty(logEntry.IpAddress))
                gelfMessage.AdditionalFields.Add("_ip_address", logEntry.IpAddress);
            
            if (!string.IsNullOrEmpty(logEntry.TraceId))
                gelfMessage.AdditionalFields.Add("_trace_id", logEntry.TraceId);
            
            if (logEntry.HttpPath != null)
                gelfMessage.AdditionalFields.Add("_http_path", logEntry.HttpPath);
            
            if (logEntry.HttpMethod != null)
                gelfMessage.AdditionalFields.Add("_http_method", logEntry.HttpMethod);

            // Serialize the Data object if present
            if (logEntry.Data != null)
            {
                string dataJson = JsonConvert.SerializeObject(logEntry.Data);
                gelfMessage.AdditionalFields.Add("_data", dataJson);
            }

            return gelfMessage;
        }

        /// <summary>
        /// Send GELF message to Graylog TCP input
        /// </summary>
        private async Task SendGelfMessageAsync(string gelfJson)
        {
            _logger.LogInformation($"Graylog'a bağlanmaya çalışılıyor: {_logSettings.GraylogHost}:{_logSettings.GraylogPort}");
            
            using (var client = new TcpClient())
            {
                try
                {
                    // Bağlantı zaman aşımı ayarlama
                    client.SendTimeout = 5000;
                    client.ReceiveTimeout = 5000;
                    
                    await client.ConnectAsync(_logSettings.GraylogHost, _logSettings.GraylogPort);
                    _logger.LogInformation("Graylog sunucusuna başarıyla bağlanıldı");
                    
                    // Mesajı byte dizisine dönüştür ve null byte ekle
                    byte[] messageBytes = Encoding.UTF8.GetBytes(gelfJson + '\0');
                    
                    // NetworkStream kullanarak doğrudan gönder
                    using (NetworkStream stream = client.GetStream())
                    {
                        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                        _logger.LogInformation($"Log başarıyla Graylog'a gönderildi. JSON: {gelfJson}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Graylog'a log gönderimi başarısız oldu ({_logSettings.GraylogHost}:{_logSettings.GraylogPort}): {ex.Message}");
                    // İstisnayı fırlatmayı kaldırıyoruz, böylece uygulama devam edebilecek
                    // ve hata konsola yazılacak
                }
            }
        }

        /// <summary>
        /// Convert DateTime to Unix timestamp (seconds since epoch)
        /// </summary>
        private double ToUnixTimestamp(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }

        /// <summary>
        /// Convert log level to syslog severity level as used by GELF
        /// 0 = Emergency, 1 = Alert, 2 = Critical, 3 = Error, 4 = Warning, 5 = Notice, 6 = Informational, 7 = Debug
        /// </summary>
        private int ConvertLogLevelToSyslog(string level)
        {
            return level?.ToLower() switch
            {
                "critical" => 2,
                "error" => 3,
                "warning" => 4,
                "info" => 6,
                "debug" => 7,
                "trace" => 7,
                _ => 6 // Default to informational
            };
        }

        /// <summary>
        /// GELF message structure according to the specification
        /// </summary>
        private class GelfMessage
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("host")]
            public string Host { get; set; }

            [JsonProperty("short_message")]
            public string ShortMessage { get; set; }

            [JsonProperty("timestamp")]
            public double Timestamp { get; set; }

            [JsonProperty("level")]
            public int Level { get; set; }

            [JsonExtensionData]
            public Dictionary<string, object> AdditionalFields { get; set; } = new Dictionary<string, object>();
        }
    }
} 