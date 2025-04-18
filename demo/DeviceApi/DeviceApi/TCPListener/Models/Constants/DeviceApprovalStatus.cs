namespace DeviceApi.TCPListener.Models.Constants
{
    /// <summary>
    /// Cihaz onay durumunu tanımlayan sabit değerler sınıfı
    /// </summary>
    public static class DeviceApprovalStatus
    {
        /// <summary>
        /// Bilinmeyen onay durumu
        /// </summary>
        public const int Unknown = 0;
        
        /// <summary>
        /// Onaylı cihaz
        /// </summary>
        public const int Approved = 1;
        
        /// <summary>
        /// Onaysız cihaz
        /// </summary>
        public const int Unapproved = 2;
    }
} 