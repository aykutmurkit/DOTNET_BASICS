namespace TCPListenerLibrary.Models
{
    /// <summary>
    /// TCP Listener için gerekli yapılandırma ayarlarını içeren model sınıfı
    /// </summary>
    public class TcpListenerSettings
    {
        /// <summary>
        /// TCP Listener'ın dinleyeceği port numarası
        /// </summary>
        public int Port { get; set; } = 3456;

        /// <summary>
        /// TCP Listener'ın dinleyeceği IP adresi
        /// </summary>
        public string IpAddress { get; set; } = "0.0.0.0";

        /// <summary>
        /// Maksimum eşzamanlı bağlantı sayısı
        /// </summary>
        public int MaxConnections { get; set; } = 100;

        /// <summary>
        /// Bağlantı zaman aşımı süresi (milisaniye)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary>
        /// Buffer boyutu (byte)
        /// </summary>
        public int BufferSize { get; set; } = 1024;
    }
} 