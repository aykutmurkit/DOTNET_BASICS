using LogLib.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogLib.Core.Interfaces
{
    /// <summary>
    /// Log veritabanı erişim arayüzü
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Yeni bir log kaydı oluşturur
        /// </summary>
        Task CreateLogAsync(LogEntry logEntry);
        
        /// <summary>
        /// Belirtilen tarihten daha eski log kayıtlarını siler
        /// </summary>
        Task<long> DeleteOldLogsAsync(DateTime olderThan);
        
        /// <summary>
        /// Belirtilen filtrelere göre log kayıtlarını getirir
        /// </summary>
        Task<(IEnumerable<LogEntry> Items, long TotalCount)> GetLogsAsync(int pageNumber, int pageSize, string level = null, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Tüm log kayıtlarını siler
        /// </summary>
        Task<long> DeleteAllLogsAsync();
    }
} 