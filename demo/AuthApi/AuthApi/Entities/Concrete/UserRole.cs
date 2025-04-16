using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
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
    /// Kullanıcı rolü varlık sınıfı
    /// </summary>
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        // Navigation property
        public ICollection<User> Users { get; set; }
    }
} 