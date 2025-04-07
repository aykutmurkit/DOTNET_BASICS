using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Platform listeleme DTO'su
    /// </summary>
    public class PlatformDto
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
        public List<DeviceDto> Devices { get; set; }
        public PredictionDto Prediction { get; set; }
    }

    /// <summary>
    /// Platform oluşturma isteği
    /// </summary>
    public class CreatePlatformRequest
    {
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public int StationId { get; set; }
    }

    /// <summary>
    /// Platform güncelleme isteği
    /// </summary>
    public class UpdatePlatformRequest
    {
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public int StationId { get; set; }
    }
} 