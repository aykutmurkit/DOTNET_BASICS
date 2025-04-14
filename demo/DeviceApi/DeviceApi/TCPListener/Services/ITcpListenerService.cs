using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace DeviceApi.TCPListener.Services
{
    /// <summary>
    /// TCP Listener servisi için arayüz
    /// </summary>
    public interface ITcpListenerService
    {
        /// <summary>
        /// TCP Listener'ın çalıştığı port numarası
        /// </summary>
        int Port { get; }
        
        /// <summary>
        /// TCP Listener'ın dinlediği IP adresi
        /// </summary>
        string IpAddress { get; }
        
        /// <summary>
        /// TCP Listener servisini başlatır
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// TCP Listener servisini durdurur
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// TCP Listener servisinin çalışıp çalışmadığını kontrol eder
        /// </summary>
        /// <returns>Servis çalışıyorsa true, değilse false</returns>
        bool IsRunning();
        
        /// <summary>
        /// Bağlı istemci sayısını döndürür
        /// </summary>
        /// <returns>Bağlı istemci sayısı</returns>
        int GetConnectedClientsCount();
        
        /// <summary>
        /// TCP Listener hakkında detaylı istatistik bilgilerini döndürür
        /// </summary>
        /// <returns>TCP Listener istatistikleri</returns>
        TcpListenerStatistics GetStatistics();
    }
    
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
        /// Aktif bağlantı thread sayısı (her bağlantı için oluşturulan threadler)
        /// </summary>
        public int ActiveThreads { get; set; }
        
        /// <summary>
        /// Servis başlangıç zamanı
        /// </summary>
        public System.DateTime? StartTime { get; set; }
        
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
        /// Hız sınırlaması yapılandırması (1 saniyede izin verilen maksimum istek sayısı)
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