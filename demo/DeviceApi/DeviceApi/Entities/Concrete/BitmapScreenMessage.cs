using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Bitmap Ekran Mesaj varlık sınıfı
    /// </summary>
    public class BitmapScreenMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string TurkishBitmap { get; set; }
        
        [Required]
        public string EnglishBitmap { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 