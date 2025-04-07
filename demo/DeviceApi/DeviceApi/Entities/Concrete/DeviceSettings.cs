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
        public string ServerIP { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int TcpPort { get; set; }
        
        [Required]
        [Range(1, 65535)]
        public int UdpPort { get; set; }
        
        [Required]
        public bool FtpStatus { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 