using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
} 