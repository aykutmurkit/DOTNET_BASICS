using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Alignment bilgisi taşıyan yardımcı DTO
    /// </summary>
    public class AlignmentValueDto
    {
        /// <summary>
        /// Alignment'ın ID'si (veritabanı ID'si)
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Alignment'ın anahtar değeri (enum'daki sayısal değere benzer)
        /// </summary>
        public int Key { get; set; }
        
        /// <summary>
        /// Alignment'ın metin karşılığı
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Tam ekran mesaj DTO sınıfı
    /// </summary>
    public class FullScreenMessageDto
    {
        public int Id { get; set; }
        public string TurkishLine1 { get; set; }
        public string TurkishLine2 { get; set; }
        public string TurkishLine3 { get; set; }
        public string TurkishLine4 { get; set; }
        public string EnglishLine1 { get; set; }
        public string EnglishLine2 { get; set; }
        public string EnglishLine3 { get; set; }
        public string EnglishLine4 { get; set; }
        
        /// <summary>
        /// Hizalama bilgisi (hem sayısal değeri hem de metin karşılığı)
        /// </summary>
        public AlignmentValueDto Alignment { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        
        /// <summary>
        /// Bu mesajı kullanan cihaz ID'leri
        /// </summary>
        public List<int> DeviceIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Tam ekran mesaj oluşturma isteği
    /// </summary>
    public class CreateFullScreenMessageRequest
    {
        [MaxLength(255)]
        public string TurkishLine1 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine2 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine3 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine4 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine1 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine2 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine3 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine4 { get; set; }
        
        /// <summary>
        /// Hizalama türü ID'si
        /// </summary>
        [Required]
        public int AlignmentTypeId { get; set; } = 1; // Varsayılan olarak Center (1)
    }

    /// <summary>
    /// Tam ekran mesaj güncelleme isteği
    /// </summary>
    public class UpdateFullScreenMessageRequest
    {
        [MaxLength(255)]
        public string TurkishLine1 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine2 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine3 { get; set; }
        
        [MaxLength(255)]
        public string TurkishLine4 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine1 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine2 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine3 { get; set; }
        
        [MaxLength(255)]
        public string EnglishLine4 { get; set; }
        
        /// <summary>
        /// Hizalama türü ID'si
        /// </summary>
        [Required]
        public int AlignmentTypeId { get; set; } = 1; // Varsayılan olarak Center (1)
    }
    
    /// <summary>
    /// Cihaza mesaj atama isteği
    /// </summary>
    public class AssignFullScreenMessageRequest
    {
        [Required]
        public int DeviceId { get; set; }
        
        [Required]
        public int FullScreenMessageId { get; set; }
    }
} 