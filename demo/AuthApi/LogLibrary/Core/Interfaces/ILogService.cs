using LogLibrary.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogLibrary.Core.Interfaces
{
    /// <summary>
    /// Interface for logging operations.
    /// </summary>
    /// <remarks>
    /// Provides methods for logging at different severity levels.
    /// Version: 1.0.0
    /// Author: R&amp;D Engineer Aykut Mürkit, İsbak
    /// </remarks>
    public interface ILogService
    {
        /// <summary>
        /// Creates a log entry at Information level.
        /// </summary>
        Task LogInfoAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null);

        /// <summary>
        /// Creates a log entry at Warning level.
        /// </summary>
        Task LogWarningAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null);

        /// <summary>
        /// Creates a log entry at Error level.
        /// </summary>
        Task LogErrorAsync(string message, string source, Exception exception = null, string userId = null, string userName = null, string userEmail = null);

        /// <summary>
        /// Creates a log entry at Debug level.
        /// </summary>
        Task LogDebugAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null);

        /// <summary>
        /// Creates a log entry at Critical level.
        /// </summary>
        Task LogCriticalAsync(string message, string source, Exception exception = null, string userId = null, string userName = null, string userEmail = null);

        /// <summary>
        /// Deletes log entries older than the specified date.
        /// </summary>
        Task<long> CleanupLogsAsync(DateTime olderThan);

        /// <summary>
        /// Retrieves logs with pagination support.
        /// </summary>
        Task<(IEnumerable<LogEntry> Items, long TotalCount)> GetLogsAsync(int pageNumber, int pageSize, string level = null, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Logs HTTP request/response information.
        /// </summary>
        Task LogHttpAsync(string path, string method, int? statusCode, long? durationMs, string traceId, string userId = null, string userName = null, string ipAddress = null, object requestData = null, object responseData = null);
    }
} 