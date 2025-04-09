using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Bitmap Ekran Mesaj varlık sınıfı
    /// </summary>
    public class BitmapScreenMessage
    {
        /// <summary>
        /// Mesaj ID'si
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Türkçe bitmap içeriği
        /// </summary>
        [Required(ErrorMessage = "Türkçe bitmap içeriği zorunludur")]
        public string TurkishBitmap { get; set; }
        
        /// <summary>
        /// İngilizce bitmap içeriği
        /// </summary>
        [Required(ErrorMessage = "İngilizce bitmap içeriği zorunludur")]
        public string EnglishBitmap { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Güncellenme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Bu mesajı kullanan cihazlar (Many-to-One ilişki)
        /// </summary>
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
} 