using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// İki faktörlü doğrulama kodu giriş isteği DTO'su
    /// </summary>
    public class TwoFactorVerifyRequest
    {
        [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Doğrulama kodu zorunludur")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Doğrulama kodu 4-10 karakter olmalıdır")]
        public string Code { get; set; }
    }

    /// <summary>
    /// İki faktörlü doğrulama ilk aşama giriş yanıtı
    /// </summary>
    public class TwoFactorRequiredResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool RequiresTwoFactor { get; set; } = true;
        public string Message { get; set; } = "İki faktörlü kimlik doğrulama gerekli. E-posta adresinize gönderilen kodu giriniz.";
    }

    /// <summary>
    /// İki faktörlü doğrulamanın aktifleştirilmesi/devre dışı bırakılması için istek
    /// </summary>
    public class TwoFactorSetupRequest
    {
        [Required(ErrorMessage = "İki faktörlü doğrulama durumu zorunludur")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Mevcut şifre zorunludur")]
        public string CurrentPassword { get; set; }
    }

    /// <summary>
    /// İki faktörlü kimlik doğrulama (2FA) durumu yanıtı.
    /// Kullanıcının 2FA durumunu, sistem ayarlarını ve açıklayıcı mesajları içerir.
    /// </summary>
    public class TwoFactorStatusResponse
    {
        /// <summary>
        /// Kullanıcının 2FA'yı kendi hesabı için etkinleştirip etkinleştirmediği
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Sistem tarafından 2FA'nın tüm kullanıcılar için zorunlu tutulup tutulmadığı
        /// </summary>
        public bool IsGloballyRequired { get; set; }

        /// <summary>
        /// 2FA durumu ile ilgili kullanıcı dostu açıklama
        /// </summary>
        public string Message { get; set; }
    }
} 