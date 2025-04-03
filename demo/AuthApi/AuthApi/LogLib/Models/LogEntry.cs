using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace LogLib.Models
{
    /// <summary>
    /// Standard log entry following industry best practices similar to ELK Stack/Graylog
    /// </summary>
    public class LogEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Application/service name that generated the log
        /// </summary>
        public string? Application { get; set; }

        /// <summary>
        /// Environment (Production, Development, Testing, etc.)
        /// </summary>
        public string? Environment { get; set; }

        /// <summary>
        /// Log level (Debug, Info, Warning, Error, Fatal)
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Log message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Logger name/category
        /// </summary>
        public string? Logger { get; set; }

        /// <summary>
        /// Exception details if any
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Exception stack trace if any
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// Correlation ID for distributed tracing
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Request ID for HTTP requests
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// User ID if available
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Username if available
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// User email if available
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// IP address of the client
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// HTTP method for API requests
        /// </summary>
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Request path for API requests
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// HTTP status code for API responses
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Request duration in milliseconds
        /// </summary>
        public long? Duration { get; set; }

        /// <summary>
        /// User agent string
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Host name of the server
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// Additional custom fields
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Timestamp when the log was created
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [JsonPropertyName("@timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 