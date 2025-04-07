using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
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
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int DeviceId { get; set; }
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
    }
} 