namespace DeviceApi.TCPListener.Models.Protocol
{
    /// <summary>
    /// Cihazların kullandığı iletişim tiplerini tanımlar
    /// </summary>
    public class CommunicationType
    {
        /// <summary>
        /// Bilinmeyen iletişim tipi
        /// </summary>
        public const int Unknown = 0;
        
        /// <summary>
        /// TCP/IP tabanlı iletişim
        /// </summary>
        public const int TcpIp = 1;
        
        /// <summary>
        /// GSM tabanlı iletişim
        /// </summary>
        public const int Gsm = 2;
    }
} 