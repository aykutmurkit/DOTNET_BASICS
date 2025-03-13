using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class ApnName : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public ApnName()
        {
            Devices = new HashSet<Device>();
        }
    }
} 