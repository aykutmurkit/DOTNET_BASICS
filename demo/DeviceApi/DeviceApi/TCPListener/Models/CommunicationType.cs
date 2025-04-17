namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihazların kullandığı iletişim tiplerini tanımlar
    /// </summary>
    public static class CommunicationType
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