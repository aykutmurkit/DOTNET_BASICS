using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class ApnAddress : BaseSetting<string>
    {
        [MaxLength(255)]
        public override string Value { get; set; }
    }
} 