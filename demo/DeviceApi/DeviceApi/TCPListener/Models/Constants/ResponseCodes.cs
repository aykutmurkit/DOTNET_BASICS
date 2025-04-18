namespace DeviceApi.TCPListener.Models.Constants
{
    /// <summary>
    /// Yanıt kodlarını tanımlayan sabit değerler sınıfı
    /// </summary>
    public static class ResponseCodes
    {
        /// <summary>
        /// Kabul etme yanıt kodu (Accept)
        /// </summary>
        public const int Accept = 1;
        
        /// <summary>
        /// Reddetme yanıt kodu (Reject)
        /// </summary>
        public const int Reject = 2;
        
        /// <summary>
        /// Hata yanıt kodu (Error)
        /// </summary>
        public const int Error = 0;
    }
} 