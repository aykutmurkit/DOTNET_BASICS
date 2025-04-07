using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Platform varlık sınıfı
    /// </summary>
    public class Platform
    {
        [Key]
        public int Id { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        [Required]
        public int StationId { get; set; }
        
        [ForeignKey("StationId")]
        public Station Station { get; set; }
        
        // Device ilişkisi - One-to-Many
        public ICollection<Device> Devices { get; set; }
        
        // Prediction ilişkisi - One-to-One
        public Prediction Prediction { get; set; }
    }
} 