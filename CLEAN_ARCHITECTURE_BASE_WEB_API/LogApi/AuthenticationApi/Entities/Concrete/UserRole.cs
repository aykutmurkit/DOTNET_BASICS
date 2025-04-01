using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
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