using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Bitmap Ekran Mesaj listeleme DTO'su
    /// </summary>
    public class BitmapScreenMessageDto
    {
        public int Id { get; set; }
        public string TurkishBitmap { get; set; }
        public string EnglishBitmap { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Bitmap Ekran Mesaj oluşturma isteği
    /// </summary>
    public class CreateBitmapScreenMessageRequest
    {
        [Required]
        public string TurkishBitmap { get; set; }
        
        [Required]
        public string EnglishBitmap { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Bitmap Ekran Mesaj güncelleme isteği
    /// </summary>
    public class UpdateBitmapScreenMessageRequest
    {
        [Required]
        public string TurkishBitmap { get; set; }
        
        [Required]
        public string EnglishBitmap { get; set; }
    }
} 