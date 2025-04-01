using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogAPI.Entities.Models
{
    public class ApiLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string TraceId { get; set; }
        
        public string Level { get; set; }
        
        public string Message { get; set; }
        
        public string Exception { get; set; }
        
        public string UserId { get; set; }
        
        public string Username { get; set; }
        
        public string Path { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }
    }
} 