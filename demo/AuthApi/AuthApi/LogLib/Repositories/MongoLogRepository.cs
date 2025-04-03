using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogLib.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;

namespace LogLib.Repositories
{
    /// <summary>
    /// MongoDB implementation of the log repository
    /// </summary>
    public class MongoLogRepository : ILogRepository
    {
        private readonly IMongoCollection<LogEntry> _logs;
        private readonly bool _resetDatabaseOnStartup;

        public MongoLogRepository(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration["LogSettings:DatabaseName"] ?? "LogLibLogs";
            var database = mongoClient.GetDatabase(databaseName);
            _logs = database.GetCollection<LogEntry>("Logs");
            
            // Reset database on startup if configured
            _resetDatabaseOnStartup = configuration.GetValue("LogSettings:ResetDatabaseOnStartup", false);
            if (_resetDatabaseOnStartup)
            {
                ClearAllLogsAsync().GetAwaiter().GetResult();
            }
            
            // Set up TTL index for automatic log expiration
            var expireAfterDays = configuration.GetValue("LogSettings:ExpireAfterDays", 30);
            var indexKeysDefinition = Builders<LogEntry>.IndexKeys.Ascending(x => x.Timestamp);
            var indexOptions = new CreateIndexOptions 
            { 
                ExpireAfter = TimeSpan.FromDays(expireAfterDays) 
            };
            
            var indexModel = new CreateIndexModel<LogEntry>(indexKeysDefinition, indexOptions);
            _logs.Indexes.CreateOne(indexModel);
        }

        public async Task SaveLogAsync(LogEntry logEntry)
        {
            await _logs.InsertOneAsync(logEntry);
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level, int skip = 0, int take = 100)
        {
            var filter = Builders<LogEntry>.Filter.Eq(x => x.Level, level);
            return await _logs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByApplicationAsync(string application, int skip = 0, int take = 100)
        {
            var filter = Builders<LogEntry>.Filter.Eq(x => x.Application, application);
            return await _logs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByUserIdAsync(string userId, int skip = 0, int take = 100)
        {
            var filter = Builders<LogEntry>.Filter.Eq(x => x.UserId, userId);
            return await _logs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByDateRangeAsync(DateTime start, DateTime end, int skip = 0, int take = 100)
        {
            var filter = Builders<LogEntry>.Filter.And(
                Builders<LogEntry>.Filter.Gte(x => x.Timestamp, start),
                Builders<LogEntry>.Filter.Lte(x => x.Timestamp, end)
            );
            
            return await _logs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
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
            var filters = new List<FilterDefinition<LogEntry>>();
            
            if (!string.IsNullOrEmpty(level))
                filters.Add(Builders<LogEntry>.Filter.Eq(x => x.Level, level));
                
            if (!string.IsNullOrEmpty(application))
                filters.Add(Builders<LogEntry>.Filter.Eq(x => x.Application, application));
                
            if (!string.IsNullOrEmpty(userId))
                filters.Add(Builders<LogEntry>.Filter.Eq(x => x.UserId, userId));
                
            if (!string.IsNullOrEmpty(username))
                filters.Add(Builders<LogEntry>.Filter.Eq(x => x.Username, username));
                
            if (startDate.HasValue)
                filters.Add(Builders<LogEntry>.Filter.Gte(x => x.Timestamp, startDate.Value));
                
            if (endDate.HasValue)
                filters.Add(Builders<LogEntry>.Filter.Lte(x => x.Timestamp, endDate.Value));
                
            if (!string.IsNullOrEmpty(searchTerm))
                filters.Add(Builders<LogEntry>.Filter.Text(searchTerm));
                
            var filter = filters.Count > 0
                ? Builders<LogEntry>.Filter.And(filters)
                : Builders<LogEntry>.Filter.Empty;
                
            return await _logs.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
        }

        public async Task ClearAllLogsAsync()
        {
            await _logs.DeleteManyAsync(Builders<LogEntry>.Filter.Empty);
        }
    }
} 