using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// İstasyon varlık sınıfı
    /// </summary>
    public class Station
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        // Platform ilişkisi - One-to-Many
        public ICollection<Platform> Platforms { get; set; }
    }
} 