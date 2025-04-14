namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihazların kullandığı iletişim tiplerini tanımlar
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// Bilinmeyen iletişim tipi
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Ethernet üzerinden iletişim
        /// </summary>
        Ethernet = 1,
        
        /// <summary>
        /// GSM/GPRS üzerinden iletişim
        /// </summary>
        GsmGprs = 2
    }
} 