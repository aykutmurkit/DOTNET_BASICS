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
        // Tam ekran mesaj
        public FullScreenMessageDto FullScreenMessage { get; set; }
        // Kayan ekran mesaj
        public ScrollingScreenMessageDto ScrollingScreenMessage { get; set; }
        // Bitmap ekran mesaj
        public BitmapScreenMessageDto BitmapScreenMessage { get; set; }
        // Periyodik mesaj
        public PeriodicMessageDto PeriodicMessage { get; set; }
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
        
        // Tam ekran mesaj (opsiyonel)
        public CreateFullScreenMessageRequest FullScreenMessage { get; set; }
        
        // Kayan ekran mesaj (opsiyonel)
        public CreateScrollingScreenMessageRequest ScrollingScreenMessage { get; set; }
        
        // Bitmap ekran mesaj (opsiyonel)
        public CreateBitmapScreenMessageRequest BitmapScreenMessage { get; set; }
        
        // Periyodik mesaj (opsiyonel)
        public CreatePeriodicMessageRequest PeriodicMessage { get; set; }
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
        
        // Tam ekran mesaj (opsiyonel)
        public UpdateFullScreenMessageRequest FullScreenMessage { get; set; }
        
        // Kayan ekran mesaj (opsiyonel)
        public UpdateScrollingScreenMessageRequest ScrollingScreenMessage { get; set; }
        
        // Bitmap ekran mesaj (opsiyonel)
        public UpdateBitmapScreenMessageRequest BitmapScreenMessage { get; set; }
        
        // Periyodik mesaj (opsiyonel)
        public UpdatePeriodicMessageRequest PeriodicMessage { get; set; }
    }
} 