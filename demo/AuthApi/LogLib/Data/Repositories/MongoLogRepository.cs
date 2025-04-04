using LogLib.Core.Interfaces;
using LogLib.Core.Models;
using LogLib.Data.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogLib.Data.Repositories
{
    /// <summary>
    /// MongoDB için log repository implementasyonu
    /// </summary>
    public class MongoLogRepository : ILogRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<LogEntry> _logCollection;
        private readonly ILogger<MongoLogRepository> _logger;

        /// <summary>
        /// MongoDB log repository constructor
        /// </summary>
        public MongoLogRepository(MongoDbContext context, ILogger<MongoLogRepository> logger = null)
        {
            _context = context;
            _logger = logger;
            _logCollection = _context.LogEntries;
        }

        /// <summary>
        /// Log kaydı oluşturur
        /// </summary>
        public async Task SaveLogAsync(LogEntry logEntry)
        {
            if (!IsMongoAvailable())
            {
                LogWarning("MongoDB bağlantısı olmadığı için log kaydı oluşturulamadı.");
                LogDebug(logEntry.ToString());
                return;
            }

            try
            {
                await _logCollection.InsertOneAsync(logEntry);
            }
            catch (Exception ex)
            {
                LogError($"Log kaydı oluşturulurken hata: {ex.Message}");
                // Hata durumunda konsola yazalım
                Console.WriteLine($"Log kaydı: {Newtonsoft.Json.JsonConvert.SerializeObject(logEntry)}");
            }
        }

        /// <summary>
        /// Belirtilen tarihten daha eski log kayıtlarını siler
        /// </summary>
        public async Task<long> DeleteOldLogsAsync(DateTime olderThan)
        {
            if (!IsMongoAvailable())
            {
                LogWarning("MongoDB bağlantısı olmadığı için eski log kayıtları silinemedi.");
                return 0;
            }

            try
            {
                var filter = Builders<LogEntry>.Filter.Lt(l => l.Timestamp, olderThan);
                var result = await _logCollection.DeleteManyAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                LogError($"Eski log kayıtları silinirken hata: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Belirtilen filtrelere göre log kayıtlarını getirir
        /// </summary>
        public async Task<(IEnumerable<LogEntry> Items, long TotalCount)> GetLogsAsync(
            int pageNumber, 
            int pageSize, 
            string level = null, 
            string searchTerm = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            if (!IsMongoAvailable())
            {
                LogWarning("MongoDB bağlantısı olmadığı için log kayıtları getirilemedi.");
                return (Enumerable.Empty<LogEntry>(), 0);
            }

            try
            {
                var builder = Builders<LogEntry>.Filter;
                var filter = builder.Empty;

                // Level filtresi
                if (!string.IsNullOrWhiteSpace(level))
                {
                    filter = builder.And(filter, builder.Eq(l => l.Level, level));
                }

                // Tarih aralığı filtresi
                if (startDate.HasValue)
                {
                    filter = builder.And(filter, builder.Gte(l => l.Timestamp, startDate.Value));
                }

                if (endDate.HasValue)
                {
                    filter = builder.And(filter, builder.Lte(l => l.Timestamp, endDate.Value));
                }

                // Metin araması filtresi
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchFilter = builder.Or(
                        builder.Regex(l => l.Message, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                        builder.Regex(l => l.Source, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                        builder.Regex(l => l.UserName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                        builder.Regex(l => l.UserEmail, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                        builder.Regex(l => l.HttpPath, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                    );
                    filter = builder.And(filter, searchFilter);
                }

                var totalCount = await _logCollection.CountDocumentsAsync(filter);

                var logs = await _logCollection
                    .Find(filter)
                    .Sort(Builders<LogEntry>.Sort.Descending(l => l.Timestamp))
                    .Skip((pageNumber - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                return (logs, totalCount);
            }
            catch (Exception ex)
            {
                LogError($"Log kayıtları getirilirken hata: {ex.Message}");
                return (Enumerable.Empty<LogEntry>(), 0);
            }
        }

        /// <summary>
        /// Tüm log kayıtlarını siler
        /// </summary>
        public async Task<long> DeleteAllLogsAsync()
        {
            if (!IsMongoAvailable())
            {
                LogWarning("MongoDB bağlantısı olmadığı için log kayıtları silinemedi.");
                return 0;
            }

            try
            {
                var result = await _logCollection.DeleteManyAsync(Builders<LogEntry>.Filter.Empty);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                LogError($"Tüm log kayıtları silinirken hata: {ex.Message}");
                return 0;
            }
        }

        #region Private Methods

        private bool IsMongoAvailable()
        {
            return _context.IsConnected && _logCollection != null;
        }

        private void LogInfo(string message)
        {
            _logger?.LogInformation(message);
            Debug.WriteLine($"INFO: {message}");
        }
        
        private void LogWarning(string message)
        {
            _logger?.LogWarning(message);
            Debug.WriteLine($"WARNING: {message}");
        }
        
        private void LogError(string message)
        {
            _logger?.LogError(message);
            Debug.WriteLine($"ERROR: {message}");
            Console.WriteLine($"MongoDB Repository Error: {message}");
        }
        
        private void LogDebug(string message)
        {
            _logger?.LogDebug(message);
            Debug.WriteLine($"DEBUG: {message}");
        }

        #endregion
    }
} 