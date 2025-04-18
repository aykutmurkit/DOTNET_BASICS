using System.Threading;
using System.Threading.Tasks;
using DeviceApi.TCPListener.Models.Dto;

namespace DeviceApi.TCPListener.Core.Interfaces
{
    /// <summary>
    /// TCP Listener servisi için arayüz
    /// </summary>
    public interface ITcpListenerService
    {
        /// <summary>
        /// TCP Listener'ın dinlediği port numarası
        /// </summary>
        int Port { get; }

        /// <summary>
        /// TCP Listener'ın dinlediği IP adresi
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// TCP Listener servisini başlatır
        /// </summary>
        /// <param name="cancellationToken">İşlemi iptal etmek için token</param>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TCP Listener servisini durdurur
        /// </summary>
        /// <param name="cancellationToken">İşlemi iptal etmek için token</param>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TCP Listener servisinin çalışıp çalışmadığını kontrol eder
        /// </summary>
        /// <returns>Servis çalışıyorsa true, aksi halde false</returns>
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
} 