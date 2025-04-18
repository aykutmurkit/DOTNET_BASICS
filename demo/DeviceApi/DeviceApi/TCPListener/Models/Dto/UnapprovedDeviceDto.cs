using System;

namespace DeviceApi.TCPListener.Models.Dto
{
    /// <summary>
    /// Onaysız cihaz bilgilerini taşıyan DTO sınıfı
    /// </summary>
    public class UnapprovedDeviceDto
    {
        /// <summary>
        /// IMEI numarası
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// İlk bağlantı IP adresi
        /// </summary>
        public string FirstConnectionIp { get; set; }

        /// <summary>
        /// İlk bağlantı tarihi
        /// </summary>
        public DateTime FirstConnectionTime { get; set; }

        /// <summary>
        /// Son bağlantı tarihi
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
        /// Onay için öneriliyor mu
        /// </summary>
        public bool IsRecommendedForApproval { get; set; }

        /// <summary>
        /// Veritabanına kaydedildi mi
        /// </summary>
        public bool IsSavedToDatabase { get; set; }
    }
} 