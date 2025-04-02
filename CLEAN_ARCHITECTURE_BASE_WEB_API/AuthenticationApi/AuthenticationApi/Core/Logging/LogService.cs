using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog;

namespace AuthenticationApi.Core.Logging
{
    public class LogService : ILogService
    {
        private readonly Logger _logger;
        private readonly IConfiguration _configuration;
        
        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            var graylogSettings = _configuration.GetSection("GraylogSettings");
            var isEnabled = graylogSettings.GetValue<bool>("Enabled");
            var host = graylogSettings.GetValue<string>("Host");
            var port = graylogSettings.GetValue<int>("Port");
            var useUdp = graylogSettings.GetValue<bool>("UseUdp");
            var logSource = graylogSettings.GetValue<string>("LogSource");
            
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName();
            
            if (isEnabled && !string.IsNullOrEmpty(host))
            {
                var graylogConfig = new GraylogSinkOptions
                {
                    HostnameOrAddress = host,
                    Port = port,
                    TransportType = useUdp ? Serilog.Sinks.Graylog.Core.Transport.TransportType.Udp : Serilog.Sinks.Graylog.Core.Transport.TransportType.Tcp,
                    Facility = logSource,
                    MinimumLogEventLevel = LogEventLevel.Information,
                    IncludeMessageTemplate = true
                };
                
                loggerConfig.WriteTo.Graylog(graylogConfig);
            }
            
            // Geliştirme ortamında konsola da log yazalım
            if (_configuration.GetValue<bool>("Logging:WriteToConsole", true))
            {
                loggerConfig.WriteTo.Console();
            }
            
            _logger = (Logger)loggerConfig.CreateLogger();
        }
        
        public void LogInfo(string message, Dictionary<string, object> additionalData = null)
        {
            var properties = GetLogEventProperties(additionalData);
            _logger.Information(message, properties);
        }
        
        public void LogWarning(string message, Dictionary<string, object> additionalData = null)
        {
            var properties = GetLogEventProperties(additionalData);
            _logger.Warning(message, properties);
        }
        
        public void LogError(string message, Exception ex = null, Dictionary<string, object> additionalData = null)
        {
            var properties = GetLogEventProperties(additionalData);
            
            if (ex != null)
            {
                _logger.Error(ex, message, properties);
            }
            else
            {
                _logger.Error(message, properties);
            }
        }
        
        public void LogDebug(string message, Dictionary<string, object> additionalData = null)
        {
            var properties = GetLogEventProperties(additionalData);
            _logger.Debug(message, properties);
        }
        
        public void LogRequest(HttpContext context, string controllerName, string actionName, object requestData = null)
        {
            var endpoint = $"{controllerName}/{actionName}";
            var userId = GetUserId(context);
            var userName = GetUserName(context);
            var userIp = context.Connection.RemoteIpAddress?.ToString();
            var httpMethod = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString.ToString();
            
            var properties = new Dictionary<string, object>
            {
                ["EndpointName"] = endpoint,
                ["ControllerName"] = controllerName,
                ["ActionName"] = actionName,
                ["UserId"] = userId,
                ["UserName"] = userName,
                ["UserIp"] = userIp,
                ["HttpMethod"] = httpMethod,
                ["Path"] = path,
                ["QueryString"] = queryString,
                ["TraceId"] = context.TraceIdentifier,
                ["RequestData"] = SerializeObjectSafely(requestData)
            };
            
            LogInfo($"API Request: {httpMethod} {path}", properties);
        }
        
        public void LogResponse(HttpContext context, string controllerName, string actionName, object responseData = null, int statusCode = 200)
        {
            var endpoint = $"{controllerName}/{actionName}";
            var userId = GetUserId(context);
            var userName = GetUserName(context);
            var userIp = context.Connection.RemoteIpAddress?.ToString();
            var httpMethod = context.Request.Method;
            var path = context.Request.Path;
            
            var properties = new Dictionary<string, object>
            {
                ["EndpointName"] = endpoint,
                ["ControllerName"] = controllerName,
                ["ActionName"] = actionName,
                ["UserId"] = userId,
                ["UserName"] = userName,
                ["UserIp"] = userIp,
                ["HttpMethod"] = httpMethod,
                ["Path"] = path,
                ["TraceId"] = context.TraceIdentifier,
                ["StatusCode"] = statusCode,
                ["ResponseData"] = SerializeObjectSafely(responseData)
            };
            
            var logMessage = $"API Response: {httpMethod} {path} - {statusCode}";
            
            if (statusCode >= 200 && statusCode < 400)
            {
                LogInfo(logMessage, properties);
            }
            else if (statusCode >= 400 && statusCode < 500)
            {
                LogWarning(logMessage, properties);
            }
            else
            {
                LogError(logMessage, null, properties);
            }
        }
        
        public void LogException(HttpContext context, string controllerName, string actionName, Exception ex)
        {
            var endpoint = $"{controllerName}/{actionName}";
            var userId = GetUserId(context);
            var userName = GetUserName(context);
            var userIp = context.Connection.RemoteIpAddress?.ToString();
            var httpMethod = context.Request.Method;
            var path = context.Request.Path;
            
            var properties = new Dictionary<string, object>
            {
                ["EndpointName"] = endpoint,
                ["ControllerName"] = controllerName,
                ["ActionName"] = actionName,
                ["UserId"] = userId,
                ["UserName"] = userName,
                ["UserIp"] = userIp,
                ["HttpMethod"] = httpMethod,
                ["Path"] = path,
                ["TraceId"] = context.TraceIdentifier,
                ["ExceptionType"] = ex.GetType().Name,
                ["StackTrace"] = ex.StackTrace
            };
            
            LogError($"API Exception: {httpMethod} {path} - {ex.Message}", ex, properties);
        }
        
        private object[] GetLogEventProperties(Dictionary<string, object> additionalData)
        {
            if (additionalData == null || !additionalData.Any())
            {
                return Array.Empty<object>();
            }
            
            return additionalData.Select(kv => kv.Value).ToArray();
        }
        
        private string GetUserId(HttpContext context)
        {
            return context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        
        private string GetUserName(HttpContext context)
        {
            return context.User?.FindFirstValue(ClaimTypes.Name);
        }
        
        private string SerializeObjectSafely(object obj)
        {
            if (obj == null) return null;
            
            try
            {
                // Hassas verileri gizleyerek serialize etme işlemi eklenebilir
                return System.Text.Json.JsonSerializer.Serialize(obj);
            }
            catch (Exception)
            {
                return "Serialization failed";
            }
        }
    }
} 