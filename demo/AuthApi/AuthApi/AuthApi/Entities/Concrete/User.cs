using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthApi.Core.Enums;

namespace Entities.Concrete
{
    /// <summary>
    /// Kullanıcı varlık sınıfı
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        public string PasswordSalt { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpireDate { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        [ForeignKey("RoleId")]
        public UserRole Role { get; set; }
        
        public byte[]? ProfilePicture { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// İki faktörlü kimlik doğrulama aktif mi
        /// </summary>
        public bool TwoFactorEnabled { get; set; } = false;

        /// <summary>
        /// Son oluşturulan 2FA kodu
        /// </summary>
        public string? TwoFactorCode { get; set; }

        /// <summary>
        /// 2FA kodunun oluşturulma zamanı
        /// </summary>
        public DateTime? TwoFactorCodeCreatedAt { get; set; }

        /// <summary>
        /// 2FA kodunun geçerlilik süresi (dakika)
        /// </summary>
        public int TwoFactorCodeExpirationMinutes { get; set; } = 10;

        #region Profil Bilgileri

        /// <summary>
        /// Kullanıcının adı (zorunlu)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Kullanıcının ikinci adı (opsiyonel)
        /// </summary>
        [MaxLength(50)]
        public string? MiddleName { get; set; }
        
        /// <summary>
        /// Kullanıcının soyadı (zorunlu)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        /// <summary>
        /// Kullanıcının tam adı (hesaplanır)
        /// </summary>
        [NotMapped]
        public string FullName => string.IsNullOrEmpty(MiddleName) 
            ? $"{FirstName} {LastName}" 
            : $"{FirstName} {MiddleName} {LastName}";
        
        /// <summary>
        /// Kullanıcının cinsiyeti (opsiyonel)
        /// </summary>
        public Gender? Gender { get; set; }
        
        /// <summary>
        /// Kullanıcının doğum tarihi (opsiyonel)
        /// </summary>
        public DateTime? Birthday { get; set; }
        
        /// <summary>
        /// Kullanıcının kimlik/TC numarası (opsiyonel)
        /// </summary>
        [MaxLength(30)]
        public string? NationalId { get; set; }
        
        /// <summary>
        /// Kullanıcının telefon numarası (E.164 formatında, zorunlu)
        /// Örn: +905551234567
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Kullanıcının adresi (opsiyonel)
        /// </summary>
        [MaxLength(200)]
        public string? Address { get; set; }
        
        /// <summary>
        /// Kullanıcının tercih ettiği dil (zorunlu)
        /// ISO 639-1 dil kodu (tr, en, vs)
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Language { get; set; }

        #endregion

        #region Şirket Bilgileri

        /// <summary>
        /// Kullanıcının çalıştığı şirket adı (opsiyonel)
        /// </summary>
        [MaxLength(100)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Kullanıcının şirketteki pozisyonu/unvanı (opsiyonel)
        /// </summary>
        [MaxLength(50)]
        public string? CompanyPosition { get; set; }

        /// <summary>
        /// Kullanıcının çalıştığı departman (opsiyonel)
        /// </summary>
        [MaxLength(50)]
        public string? CompanyDepartment { get; set; }

        /// <summary>
        /// Kullanıcının kurumsal e-posta adresi (opsiyonel)
        /// </summary>
        [MaxLength(100)]
        [EmailAddress]
        public string? CompanyEmail { get; set; }

        /// <summary>
        /// Kullanıcının iş telefonu (opsiyonel)
        /// </summary>
        [MaxLength(20)]
        [Phone]
        public string? CompanyPhone { get; set; }

        /// <summary>
        /// Kullanıcının çalıştığı şirketin web sitesi (opsiyonel)
        /// </summary>
        [MaxLength(100)]
        [Url]
        public string? CompanyWebsite { get; set; }

        /// <summary>
        /// Kullanıcının iş adresi (opsiyonel)
        /// </summary>
        [MaxLength(200)]
        public string? CompanyAddress { get; set; }

        #endregion
    }
} 