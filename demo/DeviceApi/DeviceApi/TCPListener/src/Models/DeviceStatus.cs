namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihaz durumunu tanımlayan enum
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// Onaylı cihaz
        /// </summary>
        Approved = 1,
        
        /// <summary>
        /// Onaysız cihaz
        /// </summary>
        Unapproved = 2,
        
        /// <summary>
        /// Kara listede olan cihaz
        /// </summary>
        Blacklisted = 3,
        
        /// <summary>
        /// Hız sınırlamasına tabi cihaz
        /// </summary>
        RateLimited = 4
    }
} 