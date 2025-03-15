using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace test.DTOs
{
    public class StationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateStationDto
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [Required]
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }

    public class UpdateStationDto
    {
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [Required]
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
} 