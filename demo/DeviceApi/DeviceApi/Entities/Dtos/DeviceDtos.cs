using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Cihaz listeleme DTO'su
    /// </summary>
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int PlatformId { get; set; }
        public string PlatformStationName { get; set; }
        // Cihaz ayarları
        public DeviceSettingsDto Settings { get; set; }
    }

    /// <summary>
    /// Cihaz oluşturma isteği
    /// </summary>
    public class CreateDeviceRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Ip { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int Port { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public int PlatformId { get; set; }
        
        // Cihaz ayarları
        [Required]
        public CreateDeviceSettingsRequest Settings { get; set; }
    }

    /// <summary>
    /// Cihaz güncelleme isteği
    /// </summary>
    public class UpdateDeviceRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Ip { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int Port { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public int PlatformId { get; set; }
        
        // Cihaz ayarları
        [Required]
        public UpdateDeviceSettingsRequest Settings { get; set; }
    }
} 