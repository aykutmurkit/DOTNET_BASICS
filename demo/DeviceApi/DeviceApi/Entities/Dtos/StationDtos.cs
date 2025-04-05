using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// İstasyon listeleme DTO'su
    /// </summary>
    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<PlatformDto> Platforms { get; set; }
    }

    /// <summary>
    /// İstasyon oluşturma isteği
    /// </summary>
    public class CreateStationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
    }

    /// <summary>
    /// İstasyon güncelleme isteği
    /// </summary>
    public class UpdateStationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
    }
} 