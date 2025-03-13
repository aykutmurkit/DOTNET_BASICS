using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class ApnAddress : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public ApnAddress()
        {
            Devices = new HashSet<Device>();
        }
    }
} 