using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Kayan Ekran Mesaj listeleme DTO'su
    /// </summary>
    public class ScrollingScreenMessageDto
    {
        public int Id { get; set; }
        public string TurkishLine { get; set; }
        public string EnglishLine { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Bu mesajı kullanan cihaz ID'leri
        /// </summary>
        public List<int> DeviceIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Kayan Ekran Mesaj oluşturma isteği
    /// </summary>
    public class CreateScrollingScreenMessageRequest
    {
        [Required]
        [MaxLength(200)]
        public string TurkishLine { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string EnglishLine { get; set; }
    }

    /// <summary>
    /// Kayan Ekran Mesaj güncelleme isteği
    /// </summary>
    public class UpdateScrollingScreenMessageRequest
    {
        [Required]
        [MaxLength(200)]
        public string TurkishLine { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string EnglishLine { get; set; }
    }
    
    /// <summary>
    /// Cihaza kayan ekran mesajı atama isteği
    /// </summary>
    public class AssignScrollingScreenMessageRequest
    {
        [Required]
        public int DeviceId { get; set; }
        
        [Required]
        public int ScrollingScreenMessageId { get; set; }
    }
} 