using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class Device : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string SerialNumber { get; set; }

        public int ApnNameId { get; set; }
        public int ApnPasswordId { get; set; }
        public int ApnAddressId { get; set; }

        public virtual ApnName ApnName { get; set; }
        public virtual ApnPassword ApnPassword { get; set; }
        public virtual ApnAddress ApnAddress { get; set; }
    }
} 