using System;

namespace DeviceApi.TCPListener.Models.Dto
{
    /// <summary>
    /// Onaylı cihaz bilgilerini taşıyan DTO sınıfı
    /// </summary>
    public class ApprovedDeviceDto
    {
        /// <summary>
        /// Cihaz ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cihaz adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IMEI numarası
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// Cihaz tipi
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Cihazın bağlı olduğu platform
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Cihazın bağlı olduğu istasyon
        /// </summary>
        public string Station { get; set; }

        /// <summary>
        /// Veritabanına eklenme tarihi
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Son bağlantı IP adresi
        /// </summary>
        public string LastConnectionIp { get; set; }

        /// <summary>
        /// Son bağlantı tarihi
        /// </summary>
        public DateTime? LastConnectionDate { get; set; }

        /// <summary>
        /// Durum
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Aktif mi
        /// </summary>
        public bool IsActive { get; set; }
    }
} 