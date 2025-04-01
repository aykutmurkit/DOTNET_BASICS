using System;
using System.Threading.Tasks;
using AuthenticationApi.Models.Logs;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;

namespace AuthenticationApi.Services.Logging
{
    public class MongoLogRepository : ILogRepository
    {
        private readonly IMongoCollection<RequestResponseLog> _requestResponseLogs;
        private readonly IMongoCollection<ApiLog> _apiLogs;
        
        public MongoLogRepository(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration["LogSettings:DatabaseName"] ?? "AuthenticationApiApiLogs";
            var database = mongoClient.GetDatabase(databaseName);
            
            _requestResponseLogs = database.GetCollection<RequestResponseLog>("RequestResponseLogs");
            _apiLogs = database.GetCollection<ApiLog>("ApiLogs");
            
            // TTL index ayarla
            var expireAfterDays = configuration.GetValue<int>("LogSettings:ExpireAfterDays", 30);
            
            var indexKeysRR = Builders<RequestResponseLog>.IndexKeys.Ascending(x => x.Timestamp);
            var indexOptionsRR = new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(expireAfterDays) };
            _requestResponseLogs.Indexes.CreateOne(new CreateIndexModel<RequestResponseLog>(indexKeysRR, indexOptionsRR));
            
            var indexKeysApi = Builders<ApiLog>.IndexKeys.Ascending(x => x.Timestamp);
            var indexOptionsApi = new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(expireAfterDays) };
            _apiLogs.Indexes.CreateOne(new CreateIndexModel<ApiLog>(indexKeysApi, indexOptionsApi));
        }
        
        public async Task SaveRequestResponseLogAsync(RequestResponseLog log)
        {
            await _requestResponseLogs.InsertOneAsync(log);
        }
        
        public async Task SaveApiLogAsync(ApiLog log)
        {
            await _apiLogs.InsertOneAsync(log);
        }
        
        public async Task<PagedResult<RequestResponseLog>> GetRequestResponseLogsAsync(int pageNumber, int pageSize, string search = null)
        {
            var filter = BuildRequestResponseFilter(search);
            
            var totalCount = await _requestResponseLogs.CountDocumentsAsync(filter);
            
            var logs = await _requestResponseLogs
                .Find(filter)
                .SortByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            
            return new PagedResult<RequestResponseLog> 
            {
                Items = logs,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        
        public async Task<PagedResult<ApiLog>> GetApiLogsAsync(int pageNumber, int pageSize, string level = null, string search = null)
        {
            var filter = BuildApiLogFilter(level, search);
            
            var totalCount = await _apiLogs.CountDocumentsAsync(filter);
            
            var logs = await _apiLogs
                .Find(filter)
                .SortByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            
            return new PagedResult<ApiLog> 
            {
                Items = logs,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        
        private FilterDefinition<RequestResponseLog> BuildRequestResponseFilter(string search)
        {
            var builder = Builders<RequestResponseLog>.Filter;
            var filter = builder.Empty;
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                filter = builder.Or(
                    builder.Regex(l => l.Path, new BsonRegularExpression(search, "i")),
                    builder.Regex(l => l.Username, new BsonRegularExpression(search, "i")),
                    builder.Regex(l => l.RequestBody, new BsonRegularExpression(search, "i")),
                    builder.Regex(l => l.ResponseBody, new BsonRegularExpression(search, "i"))
                );
            }
            
            return filter;
        }
        
        private FilterDefinition<ApiLog> BuildApiLogFilter(string level, string search)
        {
            var builder = Builders<ApiLog>.Filter;
            var filter = builder.Empty;
            
            if (!string.IsNullOrWhiteSpace(level))
            {
                filter = builder.Eq(l => l.Level, level);
            }
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchFilter = builder.Or(
                    builder.Regex(l => l.Message, new BsonRegularExpression(search, "i")),
                    builder.Regex(l => l.Username, new BsonRegularExpression(search, "i")),
                    builder.Regex(l => l.Path, new BsonRegularExpression(search, "i"))
                );
                
                filter = filter == builder.Empty 
                    ? searchFilter 
                    : builder.And(filter, searchFilter);
            }
            
            return filter;
        }

        public async Task<long> DeleteRequestResponseLogsAsync(DateTime? olderThan = null, string path = null)
        {
            var builder = Builders<RequestResponseLog>.Filter;
            var filter = builder.Empty;
            
            if (olderThan.HasValue)
            {
                filter = builder.And(filter, builder.Lt(l => l.Timestamp, olderThan.Value));
            }
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                filter = builder.And(filter, builder.Regex(l => l.Path, new BsonRegularExpression(path, "i")));
            }
            
            var result = await _requestResponseLogs.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteApiLogsAsync(DateTime? olderThan = null, string level = null)
        {
            var builder = Builders<ApiLog>.Filter;
            var filter = builder.Empty;
            
            if (olderThan.HasValue)
            {
                filter = builder.And(filter, builder.Lt(l => l.Timestamp, olderThan.Value));
            }
            
            if (!string.IsNullOrWhiteSpace(level))
            {
                filter = builder.And(filter, builder.Eq(l => l.Level, level));
            }
            
            var result = await _apiLogs.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteAllLogsAsync()
        {
            var requestLogsResult = await _requestResponseLogs.DeleteManyAsync(Builders<RequestResponseLog>.Filter.Empty);
            var apiLogsResult = await _apiLogs.DeleteManyAsync(Builders<ApiLog>.Filter.Empty);
            
            return requestLogsResult.DeletedCount + apiLogsResult.DeletedCount;
        }
    }
} 