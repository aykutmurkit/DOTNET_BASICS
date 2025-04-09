using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Kayan Ekran Mesaj varlık sınıfı
    /// </summary>
    public class ScrollingScreenMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string TurkishLine { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string EnglishLine { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Bu mesajı kullanan cihazlar (Many-to-One ilişki)
        /// </summary>
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
} 