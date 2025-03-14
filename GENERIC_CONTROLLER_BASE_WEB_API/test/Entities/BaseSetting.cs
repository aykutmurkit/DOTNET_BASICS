using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public abstract class BaseSetting<TValue> : BaseEntity
    {
        [Required]
        public virtual TValue Value { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        protected BaseSetting()
        {
            Devices = new HashSet<Device>();
        }
    }
} 