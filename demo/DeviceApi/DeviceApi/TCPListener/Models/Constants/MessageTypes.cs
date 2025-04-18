namespace DeviceApi.TCPListener.Models.Constants
{
    /// <summary>
    /// Mesaj tiplerini tanımlayan sabit değerler sınıfı
    /// </summary>
    public static class MessageTypes
    {
        /// <summary>
        /// El sıkışma (Handshake) mesaj tipi
        /// </summary>
        public const int Handshake = 1;
        
        /// <summary>
        /// Periyodik bilgi mesaj tipi
        /// </summary>
        public const int PeriodicInfo = 2;
        
        /// <summary>
        /// Hata/Bilinmeyen mesaj tipi
        /// </summary>
        public const int Error = 0;
    }
} 