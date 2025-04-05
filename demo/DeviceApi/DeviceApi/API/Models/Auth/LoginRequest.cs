using System.ComponentModel.DataAnnotations;

namespace DeviceApi.API.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }
    }
} 