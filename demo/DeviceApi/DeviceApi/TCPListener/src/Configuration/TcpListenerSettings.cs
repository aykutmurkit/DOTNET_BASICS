namespace DeviceApi.TCPListener.Configuration
{
    /// <summary>
    /// TCP Listener için yapılandırma ayarları
    /// </summary>
    public class TcpListenerSettings
    {
        /// <summary>
        /// TCP sunucusunun dinleyeceği port numarası
        /// </summary>
        public int Port { get; set; } = 3456;
        
        /// <summary>
        /// TCP sunucusunun dinleyeceği IP adresi
        /// </summary>
        public string IpAddress { get; set; } = "0.0.0.0";
        
        /// <summary>
        /// TCP sunucusunun dinleyeceği IP adresi (alternatif adı)
        /// </summary>
        public string ListenAddress { 
            get => IpAddress;
            set => IpAddress = value;
        }
        
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
        /// Buffer boyutu (byte)
        /// </summary>
        public int BufferSize { get; set; } = 1024;
        
        /// <summary>
        /// Mesaj başlangıç karakteri
        /// </summary>
        public char StartChar { get; set; } = '^';
        
        /// <summary>
        /// Mesaj parametre ayırıcı karakteri
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
        /// Onaysız bir cihazın sıralı maksimum bağlantı denemesi sayısı
        /// </summary>
        public int MaxUnapprovedConnectionAttempts { get; set; } = 5;
        
        /// <summary>
        /// Kara listedeki cihazların listede kalma süresi (dakika)
        /// </summary>
        public int BlacklistDurationMinutes { get; set; } = 60;
    }
} 