using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihaz ayarları varlık sınıfı
    /// </summary>
    public class DeviceSettings
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ApnName { get; set; }
        
        [MaxLength(100)]
        public string ApnUsername { get; set; }
        
        [MaxLength(100)]
        public string ApnPassword { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ServerIp { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int TcpPort { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int UdpPort { get; set; }
        
        [Required]
        public bool FtpStatus { get; set; }
        
        [MaxLength(50)]
        public string FtpIp { get; set; }
        
        [Range(1, 65535)]
        public int? FtpPort { get; set; }
        
        [MaxLength(100)]
        public string FtpUsername { get; set; }
        
        [MaxLength(100)]
        public string FtpPassword { get; set; }
        
        public int? ConnectionTimeoutDuration { get; set; }
        
        [MaxLength(50)]
        public string CommunicationHardwareVersion { get; set; }
        
        [MaxLength(50)]
        public string CommunicationSoftwareVersion { get; set; }
        
        [MaxLength(50)]
        public string GraphicsCardHardwareVersion { get; set; }
        
        [MaxLength(50)]
        public string GraphicsCardSoftwareVersion { get; set; }
        
        public int? ScrollingTextSpeed { get; set; }
        
        [MaxLength(50)]
        public string TramDisplayType { get; set; }
        
        public int? BusScreenPageCount { get; set; }
        
        [MaxLength(50)]
        public string TimeDisplayFormat { get; set; }
        
        [MaxLength(50)]
        public string TramFont { get; set; }
        
        public int? ScreenVerticalPixelCount { get; set; }
        
        public int? ScreenHorizontalPixelCount { get; set; }
        
        public int? TemperatureAlarmThreshold { get; set; }
        
        public int? HumidityAlarmThreshold { get; set; }
        
        public int? GasAlarmThreshold { get; set; }
        
        public bool? LightSensorStatus { get; set; }
        
        public int? LightSensorOperationLevel { get; set; }
        
        public int? LightSensorLevel1 { get; set; }
        
        public int? LightSensorLevel2 { get; set; }
        
        public int? LightSensorLevel3 { get; set; }
        
        [MaxLength(50)]
        public string SocketType { get; set; }
        
        [MaxLength(100)]
        public string StopName { get; set; }
        
        [MaxLength(255)]
        public string StartupLogoFilename { get; set; }
        
        [MaxLength(50)]
        public string StartupLogoCrc16 { get; set; }
        
        [MaxLength(255)]
        public string VehicleLogoFilename { get; set; }
        
        [MaxLength(50)]
        public string VehicleLogoCrc16 { get; set; }
        
        [MaxLength(50)]
        public string CommunicationType { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 