namespace TCPListenerLibrary.Services
{
    /// <summary>
    /// TCP Listener servisi için arayüz tanımı
    /// </summary>
    public interface ITcpListenerService
    {
        /// <summary>
        /// TCP sunucusunu başlatır
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// TCP sunucusunu durdurur
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// TCP sunucusunun çalışıp çalışmadığını döndürür
        /// </summary>
        bool IsRunning { get; }
    }
} 