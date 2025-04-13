using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihaz durum varlık sınıfı
    /// </summary>
    public class DeviceStatus
    {
        [Key]
        public int Id { get; set; }
        
        public bool FullScreenMessageStatus { get; set; }
        
        public bool ScrollingScreenMessageStatus { get; set; }
        
        public bool BitmapScreenMessageStatus { get; set; }
        
        public bool ScreenStatus { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 