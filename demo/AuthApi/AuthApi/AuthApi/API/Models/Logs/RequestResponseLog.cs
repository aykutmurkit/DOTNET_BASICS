using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthApi.Models.Logs
{
    public class RequestResponseLog
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string TraceId { get; set; }
        
        public string Path { get; set; }
        
        public string HttpMethod { get; set; }
        
        public string QueryString { get; set; }
        
        public string RequestBody { get; set; }
        
        public string ResponseBody { get; set; }
        
        public int StatusCode { get; set; }
        
        public string UserId { get; set; }
        
        public string Username { get; set; }
        
        public string UserIp { get; set; }
        
        public long RequestSize { get; set; }
        
        public long ResponseSize { get; set; }
        
        public long ExecutionTime { get; set; } // ms cinsinden
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }
    }
} 