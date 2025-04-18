namespace DeviceApi.TCPListener.Models.Constants
{
    /// <summary>
    /// Cihaz durumunu tanımlayan sabit değerler sınıfı
    /// </summary>
    public static class DeviceStatus
    {
        /// <summary>
        /// Onaylı cihaz
        /// </summary>
        public const int Approved = 1;
        
        /// <summary>
        /// Onaysız cihaz
        /// </summary>
        public const int Unapproved = 2;
        
        /// <summary>
        /// Kara listede olan cihaz
        /// </summary>
        public const int Blacklisted = 3;
        
        /// <summary>
        /// Hız sınırlamasına tabi cihaz
        /// </summary>
        public const int RateLimited = 4;
    }
} 