using System.ComponentModel.DataAnnotations;
using AuthApi.Core.Enums;

namespace Entities.Dtos
{
    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
    /// <summary>
    /// Token bilgileri
    /// </summary>
    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
    /// <summary>
    /// Kimlik doğrulama yanıtında kullanıcı bilgileri
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleInfo Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Language { get; set; }
        
        // Şirket bilgileri
        public string? CompanyName { get; set; }
        public string? CompanyPosition { get; set; }
    }

    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
    /// <summary>
    /// Kullanıcı rol bilgisi
    /// </summary>
    public class RoleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
    /// <summary>
    /// Token yenileme isteği DTO'su
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
    /// </summary>
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