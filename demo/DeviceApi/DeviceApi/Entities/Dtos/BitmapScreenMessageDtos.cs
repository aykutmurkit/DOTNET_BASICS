using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Bitmap Ekran Mesaj listeleme DTO'su
    /// </summary>
    public class BitmapScreenMessageDto
    {
        /// <summary>
        /// Mesaj ID'si
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Türkçe bitmap içeriği
        /// </summary>
        public string TurkishBitmap { get; set; }
        
        /// <summary>
        /// İngilizce bitmap içeriği
        /// </summary>
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
        /// Bu mesajı kullanan cihaz ID'leri
        /// </summary>
        public List<int> DeviceIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Bitmap Ekran Mesaj oluşturma isteği
    /// </summary>
    public class CreateBitmapScreenMessageRequest
    {
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
    }

    /// <summary>
    /// Bitmap Ekran Mesaj güncelleme isteği
    /// </summary>
    public class UpdateBitmapScreenMessageRequest
    {
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
    }
    
    /// <summary>
    /// Cihaza bitmap ekran mesajı atama isteği
    /// </summary>
    public class AssignBitmapScreenMessageRequest
    {
        /// <summary>
        /// Cihaz ID'si
        /// </summary>
        [Required(ErrorMessage = "Cihaz ID'si zorunludur")]
        public int DeviceId { get; set; }
        
        /// <summary>
        /// Bitmap ekran mesaj ID'si
        /// </summary>
        [Required(ErrorMessage = "Bitmap ekran mesaj ID'si zorunludur")]
        public int BitmapScreenMessageId { get; set; }
    }
} 