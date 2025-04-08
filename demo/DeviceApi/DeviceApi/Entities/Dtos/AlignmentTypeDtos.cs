using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Hizalama türü DTO
    /// </summary>
    public class AlignmentTypeDto
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Hizalama türü oluşturma isteği
    /// </summary>
    public class CreateAlignmentTypeRequest
    {
        [Required]
        public int Key { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Hizalama türü güncelleme isteği
    /// </summary>
    public class UpdateAlignmentTypeRequest
    {
        [Required]
        public int Key { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
} 