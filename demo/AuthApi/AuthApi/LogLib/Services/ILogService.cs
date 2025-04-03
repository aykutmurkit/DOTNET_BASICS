using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogLib.Models;

namespace LogLib.Services
{
    /// <summary>
    /// Interface for logging services
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Logs a debug message
        /// </summary>
        Task LogDebugAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs an informational message
        /// </summary>
        Task LogInfoAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs a warning message
        /// </summary>
        Task LogWarningAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs an error message
        /// </summary>
        Task LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs a critical error message
        /// </summary>
        Task LogCriticalAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs a security event
        /// </summary>
        Task LogSecurityAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs a custom event with a specified level
        /// </summary>
        Task LogCustomAsync(string level, string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs an API request
        /// </summary>
        Task LogApiRequestAsync(string path, string method, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? requestBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Logs an API response
        /// </summary>
        Task LogApiResponseAsync(string path, string method, int statusCode, long durationMs, string? userId = null, string? username = null, string? userEmail = null, string? responseBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);
        
        /// <summary>
        /// Retrieves logs with filtering options
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsAsync(
            string? level = null,
            string? application = null,
            string? userId = null,
            string? username = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchTerm = null,
            int skip = 0,
            int take = 100);
    }
} 