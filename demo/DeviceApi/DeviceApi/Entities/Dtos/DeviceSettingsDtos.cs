using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Cihaz ayarları listeleme DTO'su
    /// </summary>
    public class DeviceSettingsDto
    {
        public int Id { get; set; }
        public string ApnName { get; set; }
        public string ApnUsername { get; set; }
        public string ApnPassword { get; set; }
        public string ServerIp { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public bool FtpStatus { get; set; }
        public string FtpIp { get; set; }
        public int? FtpPort { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public int? ConnectionTimeoutDuration { get; set; }
        public string CommunicationHardwareVersion { get; set; }
        public string CommunicationSoftwareVersion { get; set; }
        public string GraphicsCardHardwareVersion { get; set; }
        public string GraphicsCardSoftwareVersion { get; set; }
        public int? ScrollingTextSpeed { get; set; }
        public string TramDisplayType { get; set; }
        public int? BusScreenPageCount { get; set; }
        public string TimeDisplayFormat { get; set; }
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
        public string SocketType { get; set; }
        public string StopName { get; set; }
        public string StartupLogoFilename { get; set; }
        public string StartupLogoCrc16 { get; set; }
        public string VehicleLogoFilename { get; set; }
        public string VehicleLogoCrc16 { get; set; }
        public string CommunicationType { get; set; }
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Cihaz ayarları oluşturma isteği
    /// </summary>
    public class CreateDeviceSettingsRequest
    {
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
    }

    /// <summary>
    /// Cihaz ayarları güncelleme isteği
    /// </summary>
    public class UpdateDeviceSettingsRequest
    {
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
    }
} 