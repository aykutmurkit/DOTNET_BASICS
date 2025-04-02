using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihaz varlık sınıfı
    /// </summary>
    public class Device
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Ip { get; set; }
        
        [Required]
        public int Port { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        [Required]
        public int PlatformId { get; set; }
        
        [ForeignKey("PlatformId")]
        public Platform Platform { get; set; }
    }
} 