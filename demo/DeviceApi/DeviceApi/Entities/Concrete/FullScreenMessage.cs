using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihazlar için tam ekran mesaj bilgisini tutan entity
    /// </summary>
    public class FullScreenMessage
    {
        [Key]
        public int Id { get; set; }
        
        public string TurkishLine1 { get; set; }
        public string TurkishLine2 { get; set; }
        public string TurkishLine3 { get; set; }
        public string TurkishLine4 { get; set; }
        
        public string EnglishLine1 { get; set; }
        public string EnglishLine2 { get; set; }
        public string EnglishLine3 { get; set; }
        public string EnglishLine4 { get; set; }
        
        /// <summary>
        /// Mesaj hizalama türü - Foreign Key
        /// </summary>
        public int AlignmentTypeId { get; set; }
        
        /// <summary>
        /// Mesaj hizalama türü - Navigation property
        /// </summary>
        [ForeignKey("AlignmentTypeId")]
        public AlignmentType AlignmentType { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        
        // Device ile one-to-one ilişki
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
    }
} 