using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogLib.Models;

namespace LogLib.Repositories
{
    /// <summary>
    /// Interface for log storage operations
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Saves a log entry to the database
        /// </summary>
        Task SaveLogAsync(LogEntry logEntry);
        
        /// <summary>
        /// Gets logs by level
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level, int skip = 0, int take = 100);
        
        /// <summary>
        /// Gets logs by application
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsByApplicationAsync(string application, int skip = 0, int take = 100);
        
        /// <summary>
        /// Gets logs for a specific user
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsByUserIdAsync(string userId, int skip = 0, int take = 100);
        
        /// <summary>
        /// Gets logs between specified dates
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime start, DateTime end, int skip = 0, int take = 100);
        
        /// <summary>
        /// Gets logs with advanced filtering
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
        
        /// <summary>
        /// Clears all logs (used for testing/reset)
        /// </summary>
        Task ClearAllLogsAsync();
    }
} 