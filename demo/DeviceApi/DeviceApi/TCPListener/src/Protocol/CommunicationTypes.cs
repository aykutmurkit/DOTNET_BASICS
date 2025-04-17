namespace DeviceApi.TCPListener.Protocol
{
    /// <summary>
    /// Cihazların kullandığı iletişim tiplerini tanımlar
    /// </summary>
    public static class CommunicationTypes
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
        
        /// <summary>
        /// İletişim tipinin geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="communicationType">Kontrol edilecek iletişim tipi değeri</param>
        /// <returns>Geçerli ise true, değilse false</returns>
        public static bool IsValid(int communicationType)
        {
            return communicationType >= Unknown && communicationType <= Gsm;
        }
        
        /// <summary>
        /// İletişim tipinin adını döndürür
        /// </summary>
        /// <param name="communicationType">Adı alınacak iletişim tipi değeri</param>
        /// <returns>İletişim tipinin adı</returns>
        public static string GetName(int communicationType)
        {
            return communicationType switch
            {
                Unknown => "Bilinmeyen",
                TcpIp => "TCP/IP",
                Gsm => "GSM",
                _ => "Tanımlanmamış"
            };
        }
    }
} 