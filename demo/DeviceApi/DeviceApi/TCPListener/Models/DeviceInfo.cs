using System;
using DeviceApi.TCPListener.Models.Constants;

namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihaz bilgilerini içeren sınıf
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Cihaz IMEI numarası
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// Cihaz modeli
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Son bağlantı zamanı
        /// </summary>
        public DateTime LastConnectionTime { get; set; }
        
        /// <summary>
        /// Toplam bağlantı sayısı
        /// </summary>
        public int ConnectionCount { get; set; }
        
        /// <summary>
        /// Onay tarihi (yalnızca onaylı cihazlar için)
        /// </summary>
        public DateTime? ApprovalDate { get; set; }
        
        /// <summary>
        /// Cihaz durumu
        /// </summary>
        public int Status { get; set; }
    }
} 