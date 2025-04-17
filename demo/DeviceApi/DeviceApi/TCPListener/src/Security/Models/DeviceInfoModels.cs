using System;

namespace DeviceApi.TCPListener.Security.Models
{
    /// <summary>
    /// Onaylı cihazların bilgilerini taşıyan sınıf
    /// </summary>
    public class ApprovedDeviceInfo
    {
        /// <summary>
        /// Cihaz ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Cihaz IMEI
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// Cihaz IP adresi
        /// </summary>
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Cihaz port numarası
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Cihaz adı
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Platform ID
        /// </summary>
        public int PlatformId { get; set; }
        
        /// <summary>
        /// Platform adı
        /// </summary>
        public string PlatformName { get; set; }
        
        /// <summary>
        /// İstasyon adı
        /// </summary>
        public string StationName { get; set; }
        
        /// <summary>
        /// Son bağlantı tarihi
        /// </summary>
        public DateTime LastConnectionDate { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Güncelleme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Aktif mesaj sayısı
        /// </summary>
        public int ActiveMessageCount { get; set; }
    }
    
    /// <summary>
    /// Onaysız cihazların bilgilerini taşıyan sınıf
    /// </summary>
    public class UnapprovedDeviceInfo
    {
        /// <summary>
        /// Cihaz IMEI
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// Bağlantı IP adresi
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
        /// İletişim tipi
        /// </summary>
        public int? CommunicationType { get; set; }
        
        /// <summary>
        /// Otomatik onay için önerilen mi
        /// </summary>
        public bool IsRecommendedForApproval { get; set; }
        
        /// <summary>
        /// Veritabanına kaydedildi mi
        /// </summary>
        public bool IsSavedToDatabase { get; set; }
    }
} 