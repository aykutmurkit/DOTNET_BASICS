using System;

namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Onaylı cihaz bilgilerini içeren DTO (Data Transfer Object) sınıfı
    /// </summary>
    public class ApprovedDeviceDto
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
        /// Cihaz IP Adresi
        /// </summary>
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Cihaz Port
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Cihaz Adı
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Cihazın bağlı olduğu platform ID
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
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Aktif mesaj sayısı
        /// </summary>
        public int ActiveMessageCount { get; set; }
    }
} 