using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class Station : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }
} 