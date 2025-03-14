using System.ComponentModel.DataAnnotations;

namespace test.Entities
{
    public class ApnName : BaseSetting<string>
    {
        [MaxLength(100)]
        public override string Value { get; set; }
    }
} 