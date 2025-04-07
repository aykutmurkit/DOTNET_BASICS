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
        public string ServerIP { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public bool FtpStatus { get; set; }
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
        public string ServerIP { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int TcpPort { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int UdpPort { get; set; }
        
        [Required]
        public bool FtpStatus { get; set; }
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
        public string ServerIP { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int TcpPort { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int UdpPort { get; set; }
        
        [Required]
        public bool FtpStatus { get; set; }
    }
} 