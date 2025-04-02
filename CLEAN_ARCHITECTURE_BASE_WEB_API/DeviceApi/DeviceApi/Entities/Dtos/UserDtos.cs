using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using DeviceApi.Core.Enums;

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

        /// <summary>
        /// Kullanıcı profil bilgileri
        /// </summary>
        public UserProfileInfo ProfileInfo { get; set; }
    }

    /// <summary>
    /// Kullanıcı profil bilgileri
    /// </summary>
    public class UserProfileInfo
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string? NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Language { get; set; }

        // Şirket bilgileri
        public string? CompanyName { get; set; }
        public string? CompanyPosition { get; set; }
        public string? CompanyDepartment { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyAddress { get; set; }
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

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "İkinci ad en fazla 50 karakter olabilir")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(30, ErrorMessage = "Kimlik numarası en fazla 30 karakter olabilir")]
        public string? NationalId { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Dil seçimi zorunludur")]
        [StringLength(10, ErrorMessage = "Dil kodu en fazla 10 karakter olabilir")]
        public string Language { get; set; }

        // Şirket bilgileri - Opsiyonel
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string? CompanyName { get; set; }

        [StringLength(50, ErrorMessage = "Pozisyon en fazla 50 karakter olabilir")]
        public string? CompanyPosition { get; set; }

        [StringLength(50, ErrorMessage = "Departman en fazla 50 karakter olabilir")]
        public string? CompanyDepartment { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string? CompanyEmail { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        public string? CompanyPhone { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        [StringLength(100, ErrorMessage = "Web sitesi en fazla 100 karakter olabilir")]
        public string? CompanyWebsite { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? CompanyAddress { get; set; }
    }

    /// <summary>
    /// Random şifre ile kullanıcı oluşturma DTO'su
    /// </summary>
    public class RandomPasswordUserRequest
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Rol ID zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir rol ID giriniz")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "İkinci ad en fazla 50 karakter olabilir")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(30, ErrorMessage = "Kimlik numarası en fazla 30 karakter olabilir")]
        public string? NationalId { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Dil seçimi zorunludur")]
        [StringLength(10, ErrorMessage = "Dil kodu en fazla 10 karakter olabilir")]
        public string Language { get; set; }

        // Şirket bilgileri - Opsiyonel
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string? CompanyName { get; set; }

        [StringLength(50, ErrorMessage = "Pozisyon en fazla 50 karakter olabilir")]
        public string? CompanyPosition { get; set; }

        [StringLength(50, ErrorMessage = "Departman en fazla 50 karakter olabilir")]
        public string? CompanyDepartment { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string? CompanyEmail { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        public string? CompanyPhone { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        [StringLength(100, ErrorMessage = "Web sitesi en fazla 100 karakter olabilir")]
        public string? CompanyWebsite { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? CompanyAddress { get; set; }
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

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "İkinci ad en fazla 50 karakter olabilir")]
        public string? MiddleName { get; set; }

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(30, ErrorMessage = "Kimlik numarası en fazla 30 karakter olabilir")]
        public string? NationalId { get; set; }

        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? Address { get; set; }

        [StringLength(10, ErrorMessage = "Dil kodu en fazla 10 karakter olabilir")]
        public string? Language { get; set; }

        // Şirket bilgileri - Opsiyonel
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string? CompanyName { get; set; }

        [StringLength(50, ErrorMessage = "Pozisyon en fazla 50 karakter olabilir")]
        public string? CompanyPosition { get; set; }

        [StringLength(50, ErrorMessage = "Departman en fazla 50 karakter olabilir")]
        public string? CompanyDepartment { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string? CompanyEmail { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        public string? CompanyPhone { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        [StringLength(100, ErrorMessage = "Web sitesi en fazla 100 karakter olabilir")]
        public string? CompanyWebsite { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string? CompanyAddress { get; set; }
    }

    /// <summary>
    /// Kullanıcı rolü güncelleme DTO'su
    /// </summary>
    public class UpdateUserRoleRequest
    {
        [Required(ErrorMessage = "Rol ID zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir rol ID giriniz")]
        public int RoleId { get; set; }
    }

    /// <summary>
    /// Kullanıcı e-posta güncelleme DTO'su
    /// </summary>
    public class UpdateUserEmailRequest
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }
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