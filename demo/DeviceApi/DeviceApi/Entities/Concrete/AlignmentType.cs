using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Hizalama türleri için entity sınıfı
    /// </summary>
    [Table("AlignmentTypes")]
    public class AlignmentType
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Hizalama tipi için anahtar değer (enum değeri gibi kullanılacak)
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Hizalama tipinin adı
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Bu hizalama tipini kullanan FullScreenMessage'lar (navigation property)
        /// </summary>
        public ICollection<FullScreenMessage> FullScreenMessages { get; set; }
    }
} 