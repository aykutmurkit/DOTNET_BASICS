namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// TCP Listener ayarlarını tanımlayan sınıf
    /// </summary>
    public class TcpListenerSettings
    {
        /// <summary>
        /// Dinlenecek port numarası
        /// </summary>
        public int Port { get; set; } = 3456;
        
        /// <summary>
        /// Dinlenecek IP adresi
        /// </summary>
        public string IpAddress { get; set; } = "0.0.0.0";
        
        /// <summary>
        /// Maksimum aynı anda bağlı istemci sayısı
        /// </summary>
        public int MaxConnections { get; set; } = 100;
        
        /// <summary>
        /// Bağlantı zaman aşımı süresi (milisaniye cinsinden)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30000;
        
        /// <summary>
        /// Okuma/yazma buffer boyutu
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// Mesajın başlangıç karakteri
        /// </summary>
        public char StartChar { get; set; } = '^';
        
        /// <summary>
        /// Mesaj içindeki alan ayırıcı karakter
        /// </summary>
        public char DelimiterChar { get; set; } = '+';
        
        /// <summary>
        /// Mesajın bitiş karakteri
        /// </summary>
        public char EndChar { get; set; } = '~';
    }
} 