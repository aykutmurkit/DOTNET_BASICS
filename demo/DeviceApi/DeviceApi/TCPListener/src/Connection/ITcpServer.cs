using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeviceApi.TCPListener.Connection.Models;

namespace DeviceApi.TCPListener.Connection
{
    /// <summary>
    /// TCP sunucu servisi için arayüz
    /// </summary>
    public interface ITcpServer
    {
        /// <summary>
        /// TCP sunucusunun çalıştığı port numarası
        /// </summary>
        int Port { get; }
        
        /// <summary>
        /// TCP sunucusunun dinlediği IP adresi
        /// </summary>
        string IpAddress { get; }
        
        /// <summary>
        /// TCP sunucusu servisini başlatır
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// TCP sunucusu servisini durdurur
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// TCP sunucusu servisinin çalışıp çalışmadığını kontrol eder
        /// </summary>
        /// <returns>Servis çalışıyorsa true, değilse false</returns>
        bool IsRunning();
        
        /// <summary>
        /// Bağlı istemci sayısını döndürür
        /// </summary>
        /// <returns>Bağlı istemci sayısı</returns>
        int GetConnectedClientsCount();
        
        /// <summary>
        /// Aktif bağlantıların adreslerini döndürür
        /// </summary>
        /// <returns>Bağlı istemcilerin adresleri</returns>
        List<string> GetActiveClientAddresses();
        
        /// <summary>
        /// TCP sunucusu hakkında detaylı istatistik bilgilerini döndürür
        /// </summary>
        /// <returns>TCP sunucusu istatistikleri</returns>
        ServerStatistics GetStatistics();
    }
} 