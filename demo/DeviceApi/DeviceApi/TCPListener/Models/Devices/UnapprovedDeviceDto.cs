using System;
using DeviceApi.TCPListener.Models.Protocol;

namespace DeviceApi.TCPListener.Models.Devices
{
    /// <summary>
    /// Onaylanmamış cihaz bilgilerini içeren DTO (Data Transfer Object) sınıfı
    /// </summary>
    public class UnapprovedDeviceDto
    {
        /// <summary>
        /// Cihaz IMEI
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// Bağlantı IP Adresi
        /// </summary>
        public string ConnectionIpAddress { get; set; }
        
        /// <summary>
        /// İlk bağlantı zamanı
        /// </summary>
        public DateTime FirstConnectionTime { get; set; }
        
        /// <summary>
        /// Son bağlantı zamanı
        /// </summary>
        public DateTime LastConnectionTime { get; set; }
        
        /// <summary>
        /// Toplam bağlantı sayısı
        /// </summary>
        public int ConnectionCount { get; set; }
        
        /// <summary>
        /// Bağlantı protokolü
        /// </summary>
        public int? CommunicationType { get; set; }
        
        /// <summary>
        /// Otomatik onay için önerilen mi
        /// </summary>
        public bool IsRecommendedForApproval { get; set; }
        
        /// <summary>
        /// Veriler veritabanına kaydedildi mi
        /// </summary>
        public bool IsSavedToDatabase { get; set; }
    }
} 