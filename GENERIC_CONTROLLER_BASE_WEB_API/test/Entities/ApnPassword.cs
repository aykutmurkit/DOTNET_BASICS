using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class ApnPassword : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public ApnPassword()
        {
            Devices = new HashSet<Device>();
        }
    }
} 