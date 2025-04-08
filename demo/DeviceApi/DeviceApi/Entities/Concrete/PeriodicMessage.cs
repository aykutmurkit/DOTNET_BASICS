using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Periyodik Mesaj varlık sınıfı
    /// </summary>
    public class PeriodicMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int TemperatureLevel { get; set; }
        
        [Required]
        public int HumidityLevel { get; set; }
        
        [Required]
        public int GasLevel { get; set; }
        
        [Required]
        public int FrontLightLevel { get; set; }
        
        [Required]
        public int BackLightLevel { get; set; }
        
        [Required]
        public int LedFailureCount { get; set; }
        
        [Required]
        public bool CabinStatus { get; set; }
        
        [Required]
        public bool FanStatus { get; set; }
        
        [Required]
        public bool ShowStatus { get; set; }
        
        [Required]
        public bool Rs232Status { get; set; }
        
        [Required]
        public bool PowerSupplyStatus { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ForecastedAt { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 