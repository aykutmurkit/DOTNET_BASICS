using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Entities.Concrete
{
    /// <summary>
    /// Font türü varlık sınıfı
    /// </summary>
    public class FontType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Key { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required]
        public int Width { get; set; }
        
        [Required]
        public int Height { get; set; }
        
        // One-to-Many ilişki - Birden fazla FullScreenMessage bu font türünü kullanabilir
        public ICollection<FullScreenMessage> FullScreenMessages { get; set; }
    }
} 