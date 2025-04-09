using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihaz varlık sınıfı
    /// </summary>
    public class Device
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Ip { get; set; }
        
        [Required]
        public int Port { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        [Required]
        public int PlatformId { get; set; }
        
        [ForeignKey("PlatformId")]
        public Platform Platform { get; set; }
        
        // One-to-One ilişki - DeviceSettings
        public DeviceSettings Settings { get; set; }
        
        // One-to-One ilişki - DeviceStatus
        public DeviceStatus Status { get; set; }
        
        // Many-to-One ilişki - FullScreenMessage (Çoklu cihaz aynı mesajı kullanabilir)
        public int? FullScreenMessageId { get; set; }
        
        [ForeignKey("FullScreenMessageId")]
        public FullScreenMessage FullScreenMessage { get; set; }
        
        // Many-to-One ilişki - ScrollingScreenMessage (Çoklu cihaz aynı mesajı kullanabilir)
        public int? ScrollingScreenMessageId { get; set; }
        
        [ForeignKey("ScrollingScreenMessageId")]
        public ScrollingScreenMessage ScrollingScreenMessage { get; set; }
        
        // Many-to-One ilişki - BitmapScreenMessage (Çoklu cihaz aynı mesajı kullanabilir)
        public int? BitmapScreenMessageId { get; set; }
        
        [ForeignKey("BitmapScreenMessageId")]
        public BitmapScreenMessage BitmapScreenMessage { get; set; }
        
        // One-to-One ilişki - PeriodicMessage
        public PeriodicMessage PeriodicMessage { get; set; }
    }
} 