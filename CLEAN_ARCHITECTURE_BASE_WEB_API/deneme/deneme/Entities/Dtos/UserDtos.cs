using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Kullanıcı listeleme DTO'su
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleInfo Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        /// <summary>
        /// İki faktörlü kimlik doğrulama bilgileri
        /// </summary>
        public TwoFactorInfo TwoFactor { get; set; }
        
        /// <summary>
        /// Profil fotoğrafı bilgileri
        /// </summary>
        public ProfilePictureInfo ProfilePicture { get; set; }
    }
    
    /// <summary>
    /// İki faktörlü kimlik doğrulama bilgileri
    /// </summary>
    public class TwoFactorInfo
    {
        /// <summary>
        /// Kullanıcının 2FA'yı kendi hesabı için etkinleştirip etkinleştirmediği
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Sistemde 2FA'nın zorunlu olup olmadığı
        /// </summary>
        public bool Required { get; set; }
    }
    
    /// <summary>
    /// Profil fotoğrafı bilgileri
    /// </summary>
    public class ProfilePictureInfo
    {
        /// <summary>
        /// Kullanıcının profil fotoğrafının olup olmadığı
        /// </summary>
        public bool HasProfilePicture { get; set; }
        
        /// <summary>
        /// Profil fotoğrafı URL'si
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Profil fotoğrafı verileri (base64 formatında)
        /// </summary>
        public string Picture { get; set; }
    }

    /// <summary>
    /// Kullanıcı oluşturma DTO'su
    /// </summary>
    public class CreateUserRequest
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

        [Required(ErrorMessage = "Rol ID zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir rol ID giriniz")]
        public int RoleId { get; set; }
    }

    /// <summary>
    /// Kullanıcı güncelleme DTO'su
    /// </summary>
    public class UpdateUserRequest
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir rol ID giriniz")]
        public int? RoleId { get; set; }
    }

    /// <summary>
    /// Profil fotoğrafı yükleme DTO'su
    /// </summary>
    public class UploadProfilePictureRequest
    {
        [Required(ErrorMessage = "Profil fotoğrafı zorunludur")]
        public IFormFile ProfilePicture { get; set; }
    }
} 