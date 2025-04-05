using System.ComponentModel.DataAnnotations;

namespace DeviceApi.API.Models.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }
        
        [Required]
        public string RefreshToken { get; set; }
    }
} 