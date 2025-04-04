using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace LogLib.Core.Models
{
    /// <summary>
    /// Log entry model stored in MongoDB.
    /// </summary>
    /// <remarks>
    /// Version: 1.0.0
    /// Author: R&amp;D Engineer Aykut Mürkit, İsbak
    /// </remarks>
    public class LogEntry
    {
        /// <summary>
        /// MongoDB belge kimliği
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Loglamanın gerçekleştiği zaman (UTC)
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Log seviyesi (Info, Error, Warning, Debug vb.)
        /// </summary>
        public string Level { get; set; } = "Info";

        /// <summary>
        /// Log mesajı
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Loglamayı oluşturan kaynak (sınıf/metod adı)
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının ID'si (varsa)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının adı (varsa)
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının email adresi (varsa)
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// İsteğin trace ID'si (correlation için)
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// İlgili ek veri (parametreler, exception detayı vb.)
        /// </summary>
        [BsonElement("Data")]
        [BsonIgnoreIfNull]
        [JsonProperty("Data")]
        [BsonSerializer(typeof(MongoDB.Bson.Serialization.Serializers.ObjectSerializer))]
        public object? Data { get; set; }

        /// <summary>
        /// İsteğin geldiği IP adresi
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Uygulama adı
        /// </summary>
        public string ApplicationName { get; set; } = "AuthApi";

        /// <summary>
        /// Ortam (Development, Production, Staging vb.)
        /// </summary>
        public string Environment { get; set; } = "Development";

        /// <summary>
        /// Varsa HTTP isteği metodu (GET, POST, PUT, DELETE vb.)
        /// </summary>
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Varsa HTTP isteği yolu
        /// </summary>
        public string? HttpPath { get; set; }

        /// <summary>
        /// HTTP durum kodu (varsa)
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// İşlem süresi (milisaniye)
        /// </summary>
        public long? Duration { get; set; }
        
        /// <summary>
        /// HTTP durum kodu (varsa) - StatusCode ile aynı, uyumluluk için
        /// </summary>
        public int? HttpStatusCode { get; set; }
        
        /// <summary>
        /// İşlem süresi (milisaniye) - Duration ile aynı, uyumluluk için
        /// </summary>
        public long? HttpDurationMs { get; set; }
        
        /// <summary>
        /// HTTP isteği verisi
        /// </summary>
        public object? Request { get; set; }
        
        /// <summary>
        /// HTTP yanıt verisi
        /// </summary>
        public object? Response { get; set; }
    }
} 