namespace DeviceApi.TCPListener.Models.Constants
{
    /// <summary>
    /// İletişim tiplerini tanımlayan sabit değerler sınıfı
    /// </summary>
    public static class CommunicationTypes
    {
        /// <summary>
        /// Bilinmeyen iletişim tipi
        /// </summary>
        public const int Unknown = 0;
        
        /// <summary>
        /// Ethernet üzerinden iletişim
        /// </summary>
        public const int Ethernet = 1;
        
        /// <summary>
        /// GSM/GPRS üzerinden iletişim
        /// </summary>
        public const int GsmGprs = 2;
    }
} 