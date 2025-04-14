using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Font türü listeleme DTO'su
    /// </summary>
    public class FontTypeDto
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    
    /// <summary>
    /// Font türü değer DTO'su (başka DTO'lar içinde kullanılacak)
    /// </summary>
    public class FontTypeValueDto
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Font türü oluşturma isteği
    /// </summary>
    public class CreateFontTypeRequest
    {
        [Required]
        public int Key { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Width { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Height { get; set; }
    }

    /// <summary>
    /// Font türü güncelleme isteği
    /// </summary>
    public class UpdateFontTypeRequest
    {
        [Required]
        public int Key { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Width { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Height { get; set; }
    }
} 