using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Platform için tren tahmin bilgileri
    /// </summary>
    public class Prediction
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string StationName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Direction { get; set; }
        
        // İlk tren tahmini (null olabilir)
        [MaxLength(50)]
        public string Train1 { get; set; }
        
        [MaxLength(20)]
        public string Line1 { get; set; }
        
        [MaxLength(100)]
        public string Destination1 { get; set; }
        
        public DateTime? Time1 { get; set; }
        
        // İkinci tren tahmini (null olabilir)
        [MaxLength(50)]
        public string Train2 { get; set; }
        
        [MaxLength(20)]
        public string Line2 { get; set; }
        
        [MaxLength(100)]
        public string Destination2 { get; set; }
        
        public DateTime? Time2 { get; set; }
        
        // Üçüncü tren tahmini (null olabilir) 
        [MaxLength(50)]
        public string Train3 { get; set; }
        
        [MaxLength(20)]
        public string Line3 { get; set; }
        
        [MaxLength(100)]
        public string Destination3 { get; set; }
        
        public DateTime? Time3 { get; set; }
        
        // Tahmin oluşturma ve kayıt tarihi
        [Required]
        public DateTime ForecastGenerationAt { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        // Platform ile one-to-one ilişki
        [Required]
        public int PlatformId { get; set; }
        
        [ForeignKey("PlatformId")]
        public Platform Platform { get; set; }
    }
} 