using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LogLib.Models;
using LogLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LogLib.Services
{
    /// <summary>
    /// Implementation of the log service that stores logs in MongoDB
    /// </summary>
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _applicationName;
        private readonly string _environment;
        private readonly string _hostName;

        public LogService(
            ILogRepository logRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _logRepository = logRepository;
            _httpContextAccessor = httpContextAccessor;
            _applicationName = configuration["LogSettings:ApplicationName"] ?? "LogLib";
            _environment = configuration["LogSettings:Environment"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            _hostName = Environment.MachineName;
        }

        public async Task LogDebugAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync("Debug", message, null, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogInfoAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync("Info", message, null, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogWarningAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync("Warning", message, null, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync("Error", message, exception, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogCriticalAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync("Critical", message, exception, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogSecurityAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            additionalData ??= new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(ipAddress))
            {
                additionalData["IpAddress"] = ipAddress;
            }
            
            await LogMessageAsync("Security", message, null, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogCustomAsync(string level, string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            await LogMessageAsync(level, message, null, userId, username, userEmail, correlationId, additionalData);
        }

        public async Task LogApiRequestAsync(string path, string method, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? requestBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            additionalData ??= new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(requestBody))
            {
                additionalData["RequestBody"] = requestBody;
            }
            
            if (!string.IsNullOrEmpty(ipAddress))
            {
                additionalData["IpAddress"] = ipAddress;
            }
            
            var logEntry = new LogEntry
            {
                Application = _applicationName,
                Environment = _environment,
                Level = "Info",
                Message = $"API Request: {method} {path}",
                Logger = "LogLib.ApiRequest",
                CorrelationId = correlationId ?? GetCorrelationId(),
                UserId = userId,
                Username = username,
                UserEmail = userEmail,
                HttpMethod = method,
                Path = path,
                IpAddress = ipAddress,
                Host = _hostName,
                AdditionalData = additionalData,
                Timestamp = DateTime.UtcNow
            };
            
            await _logRepository.SaveLogAsync(logEntry);
        }

        public async Task LogApiResponseAsync(string path, string method, int statusCode, long durationMs, string? userId = null, string? username = null, string? userEmail = null, string? responseBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null)
        {
            additionalData ??= new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(responseBody))
            {
                additionalData["ResponseBody"] = responseBody;
            }
            
            additionalData["DurationMs"] = durationMs;
            
            var level = statusCode < 400 ? "Info" : statusCode < 500 ? "Warning" : "Error";
            
            var logEntry = new LogEntry
            {
                Application = _applicationName,
                Environment = _environment,
                Level = level,
                Message = $"API Response: {method} {path} - {statusCode} - {durationMs}ms",
                Logger = "LogLib.ApiResponse",
                CorrelationId = correlationId ?? GetCorrelationId(),
                UserId = userId,
                Username = username,
                UserEmail = userEmail,
                HttpMethod = method,
                Path = path,
                StatusCode = statusCode,
                Duration = durationMs,
                Host = _hostName,
                AdditionalData = additionalData,
                Timestamp = DateTime.UtcNow
            };
            
            await _logRepository.SaveLogAsync(logEntry);
        }

        public async Task<IEnumerable<LogEntry>> GetLogsAsync(
            string? level = null,
            string? application = null,
            string? userId = null,
            string? username = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchTerm = null,
            int skip = 0,
            int take = 100)
        {
            return await _logRepository.GetLogsAsync(
                level,
                application,
                userId,
                username,
                startDate,
                endDate,
                searchTerm,
                skip,
                take);
        }

        private async Task LogMessageAsync(
            string level,
            string message,
            Exception? exception = null,
            string? userId = null,
            string? username = null,
            string? userEmail = null,
            string? correlationId = null,
            Dictionary<string, object>? additionalData = null)
        {
            var context = _httpContextAccessor?.HttpContext;
            
            var logEntry = new LogEntry
            {
                Application = _applicationName,
                Environment = _environment,
                Level = level,
                Message = message,
                Logger = "LogLib",
                Exception = exception?.Message,
                StackTrace = exception?.StackTrace,
                CorrelationId = correlationId ?? GetCorrelationId(),
                RequestId = context?.TraceIdentifier,
                UserId = userId ?? GetUserId(),
                Username = username ?? GetUsername(),
                UserEmail = userEmail ?? GetUserEmail(),
                Path = context?.Request?.Path,
                HttpMethod = context?.Request?.Method,
                IpAddress = context?.Connection?.RemoteIpAddress?.ToString(),
                UserAgent = context?.Request?.Headers["User-Agent"],
                Host = _hostName,
                AdditionalData = additionalData ?? new Dictionary<string, object>(),
                Timestamp = DateTime.UtcNow
            };
            
            await _logRepository.SaveLogAsync(logEntry);
        }

        private string? GetCorrelationId()
        {
            // Try to get from Activity or HttpContext
            return Activity.Current?.Id ?? 
                   _httpContextAccessor?.HttpContext?.TraceIdentifier ?? 
                   Guid.NewGuid().ToString();
        }

        private string? GetUserId()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst("nameid")?.Value;
        }

        private string? GetUsername()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst("unique_name")?.Value;
        }

        private string? GetUserEmail()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst("email")?.Value;
        }
    }
} 