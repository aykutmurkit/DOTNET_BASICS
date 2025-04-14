using System.Threading;
using System.Threading.Tasks;

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
    }
} 