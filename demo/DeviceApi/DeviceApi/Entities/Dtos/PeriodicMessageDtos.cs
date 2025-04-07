using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Periyodik Mesaj listeleme DTO'su
    /// </summary>
    public class PeriodicMessageDto
    {
        public int Id { get; set; }
        public int TemperatureLevel { get; set; }
        public int HumidityLevel { get; set; }
        public int GasLevel { get; set; }
        public int FrontLightLevel { get; set; }
        public int BackLightLevel { get; set; }
        public int LedFailureCount { get; set; }
        public bool CabinStatus { get; set; }
        public bool FanStatus { get; set; }
        public bool ShowStatus { get; set; }
        public bool Rs232Status { get; set; }
        public bool PowerSupplyStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Periyodik Mesaj oluşturma isteği
    /// </summary>
    public class CreatePeriodicMessageRequest
    {
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
        
        [Required]
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Periyodik Mesaj güncelleme isteği
    /// </summary>
    public class UpdatePeriodicMessageRequest
    {
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
    }
} 