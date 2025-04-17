using System;
using System.Collections.Generic;

namespace DeviceApi.TCPListener.Connection.Models
{
    /// <summary>
    /// TCP sunucusu servisi istatistiklerini içeren sınıf
    /// </summary>
    public class ServerStatistics
    {
        /// <summary>
        /// Servisin çalışıp çalışmadığı
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// Dinlenen port numarası
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Dinlenen IP adresi
        /// </summary>
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Aktif bağlantı sayısı
        /// </summary>
        public int ActiveConnections { get; set; }
        
        /// <summary>
        /// Maksimum eşzamanlı bağlantı limiti
        /// </summary>
        public int MaximumConnections { get; set; }
        
        /// <summary>
        /// Toplam bağlantı sayısı (başlangıçtan beri)
        /// </summary>
        public long TotalConnectionsReceived { get; set; }
        
        /// <summary>
        /// Aktif bağlantı thread sayısı
        /// </summary>
        public int ActiveThreads { get; set; }
        
        /// <summary>
        /// Servis başlangıç zamanı
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// Toplam çalışma süresi (saat:dakika:saniye formatında)
        /// </summary>
        public string Uptime { get; set; }
        
        /// <summary>
        /// Son dakikada alınan bağlantı sayısı
        /// </summary>
        public int ConnectionsLastMinute { get; set; }
        
        /// <summary>
        /// Aktif bağlı cihaz listesi (IP:Port formatında)
        /// </summary>
        public List<string> ActiveClientAddresses { get; set; }
        
        /// <summary>
        /// Rate Limiting ve kara liste istatistikleri
        /// </summary>
        public RateLimitStatistics RateLimit { get; set; }
        
        /// <summary>
        /// Mesaj işleme istatistikleri
        /// </summary>
        public MessageStatistics MessageStats { get; set; }
    }
    
    /// <summary>
    /// Rate limiting ve kara liste istatistiklerini içeren sınıf
    /// </summary>
    public class RateLimitStatistics
    {
        /// <summary>
        /// Şu anda kara listede bulunan IMEI sayısı
        /// </summary>
        public int BlacklistedImeiCount { get; set; }
        
        /// <summary>
        /// Şu anda hız sınırlaması izlenen IMEI sayısı
        /// </summary>
        public int RateLimitedImeiCount { get; set; }
        
        /// <summary>
        /// Kara liste süresi (saniye)
        /// </summary>
        public int BlacklistDurationSeconds { get; set; }
        
        /// <summary>
        /// Hız sınırlaması yapılandırması
        /// </summary>
        public string RateLimitConfig { get; set; }
    }
    
    /// <summary>
    /// Mesaj işleme istatistiklerini içeren sınıf
    /// </summary>
    public class MessageStatistics
    {
        /// <summary>
        /// İşlenen toplam mesaj sayısı
        /// </summary>
        public long TotalProcessedMessages { get; set; }
        
        /// <summary>
        /// Log kısıtlama nedeniyle loglanmayan mesaj sayısı
        /// </summary>
        public long ThrottledLogCount { get; set; }
        
        /// <summary>
        /// Son başarılı handshake zamanı
        /// </summary>
        public DateTime? LastSuccessfulHandshake { get; set; }
        
        /// <summary>
        /// Son reddedilen handshake zamanı
        /// </summary>
        public DateTime? LastRejectedHandshake { get; set; }
    }
}