namespace DeviceApi.TCPListener.Models.Protocol
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
        /// TCP/IP tabanlı iletişim
        /// </summary>
        TcpIp = 1,
        
        /// <summary>
        /// GSM tabanlı iletişim
        /// </summary>
        Gsm = 2
    }
} 