using System;
using System.Collections.Generic;

namespace DeviceApi.TCPListener.Models.Dto
{
    /// <summary>
    /// TCP Listener servisi istatistiklerini içeren sınıf
    /// </summary>
    public class TcpListenerStatistics
    {
        /// <summary>
        /// Servisin çalışıp çalışmadığı
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Dinleme yapılan IP adresi
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Dinleme yapılan port numarası
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Aktif bağlantı sayısı
        /// </summary>
        public int ActiveConnections { get; set; }

        /// <summary>
        /// İzin verilen maksimum bağlantı sayısı
        /// </summary>
        public int MaximumConnections { get; set; }

        /// <summary>
        /// Toplam alınan bağlantı sayısı
        /// </summary>
        public long TotalConnectionsReceived { get; set; }

        /// <summary>
        /// Son dakikada gelen bağlantı sayısı
        /// </summary>
        public int ConnectionsLastMinute { get; set; }

        /// <summary>
        /// Aktif thread sayısı
        /// </summary>
        public int ActiveThreads { get; set; }

        /// <summary>
        /// Servisin başlama zamanı
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Çalışma süresi (saat:dakika:saniye formatında)
        /// </summary>
        public string Uptime { get; set; }

        /// <summary>
        /// Aktif istemci IP adresleri
        /// </summary>
        public List<string> ActiveClientAddresses { get; set; } = new List<string>();

        /// <summary>
        /// Hız sınırlama istatistikleri
        /// </summary>
        public RateLimitStats RateLimit { get; set; }

        /// <summary>
        /// Mesaj işleme istatistikleri
        /// </summary>
        public MessageProcessingStats MessageStats { get; set; }
    }

    /// <summary>
    /// Hız sınırlama istatistiklerini içeren iç sınıf
    /// </summary>
    public class RateLimitStats
    {
        /// <summary>
        /// Kara listeye alınmış IMEI sayısı
        /// </summary>
        public int BlacklistedImeiCount { get; set; }

        /// <summary>
        /// Hız sınırlamasına tabi tutulan IMEI sayısı
        /// </summary>
        public int RateLimitedImeiCount { get; set; }

        /// <summary>
        /// Kara liste süre sınırı (saniye)
        /// </summary>
        public int BlacklistDurationSeconds { get; set; }

        /// <summary>
        /// Hız sınırlama yapılandırması
        /// </summary>
        public string RateLimitConfig { get; set; }
    }

    /// <summary>
    /// Mesaj işleme istatistiklerini içeren iç sınıf
    /// </summary>
    public class MessageProcessingStats
    {
        /// <summary>
        /// Toplam işlenen mesaj sayısı
        /// </summary>
        public long TotalProcessedMessages { get; set; }

        /// <summary>
        /// Throttle edilen log sayısı
        /// </summary>
        public long ThrottledLogCount { get; set; }

        /// <summary>
        /// Son başarılı el sıkışma zamanı
        /// </summary>
        public DateTime? LastSuccessfulHandshake { get; set; }

        /// <summary>
        /// Son reddedilen el sıkışma zamanı
        /// </summary>
        public DateTime? LastRejectedHandshake { get; set; }
    }
} 