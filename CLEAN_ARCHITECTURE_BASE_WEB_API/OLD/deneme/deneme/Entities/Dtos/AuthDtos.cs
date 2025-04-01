using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Kullanıcı giriş isteği DTO'su
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Kullanıcı kayıt isteği DTO'su
    /// </summary>
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Kullanıcı JWT token yanıtı DTO'su
    /// </summary>
    public class AuthResponse
    {
        public TokenInfo AccessToken { get; set; }
        public TokenInfo RefreshToken { get; set; }
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Token bilgileri
    /// </summary>
    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Kimlik doğrulama yanıtında kullanıcı bilgileri
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleInfo Role { get; set; }
    }

    /// <summary>
    /// Kullanıcı rol bilgisi
    /// </summary>
    public class RoleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Token yenileme isteği DTO'su
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Şifre değiştirme isteği DTO'su
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mevcut şifre zorunludur")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmNewPassword { get; set; }
    }

    /// <summary>
    /// Şifre sıfırlama isteği gönderme DTO'su
    /// </summary>
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Şifre sıfırlama DTO'su
    /// </summary>
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmNewPassword { get; set; }
    }
} 