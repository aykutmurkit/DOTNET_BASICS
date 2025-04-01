using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace deneme.Models.Logs
{
    public class ApiLog
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string TraceId { get; set; }
        
        public string Level { get; set; } // Info, Warning, Error
        
        public string Message { get; set; }
        
        public string Exception { get; set; }
        
        public string UserId { get; set; }
        
        public string Username { get; set; }
        
        public string Path { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }
    }
} 