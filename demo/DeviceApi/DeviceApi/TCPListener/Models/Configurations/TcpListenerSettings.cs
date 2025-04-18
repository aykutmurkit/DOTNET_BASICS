namespace DeviceApi.TCPListener.Models.Configurations
{
    /// <summary>
    /// TCP Listener servisi için yapılandırma ayarları
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
        /// İstemci zaman aşımı süresi (milisaniye)
        /// </summary>
        public int ClientTimeoutMilliseconds { get; set; } = 60000;

        /// <summary>
        /// Okuma/yazma buffer boyutu
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// Mesaj başlangıç karakteri
        /// </summary>
        public char StartChar { get; set; } = '^';

        /// <summary>
        /// Parametre ayırıcı karakteri
        /// </summary>
        public char DelimiterChar { get; set; } = '+';

        /// <summary>
        /// Mesaj bitiş karakteri
        /// </summary>
        public char EndChar { get; set; } = '~';

        /// <summary>
        /// Dakika başına izin verilen maksimum istek sayısı
        /// </summary>
        public int RateLimitPerMinute { get; set; } = 60;

        /// <summary>
        /// Onaysız bir cihazın kara listeye alınmadan önceki maksimum bağlantı denemesi sayısı
        /// </summary>
        public int MaxUnapprovedConnectionAttempts { get; set; } = 5;

        /// <summary>
        /// Kara listedeki cihazların listede kalma süresi (dakika)
        /// </summary>
        public int BlacklistDurationMinutes { get; set; } = 60;
    }
} 