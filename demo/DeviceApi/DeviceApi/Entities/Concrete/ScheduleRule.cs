using Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    /// <summary>
    /// Cihazlar için zamanlanmış kurallar
    /// </summary>
    public class ScheduleRule
    {
        public ScheduleRule()
        {
            // RecurringDays için varsayılan değer ata
            RecurringDays = "0";
        }
        
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
        
        /// <summary>
        /// Kuralın ismi (örn: Cumhuriyet Bayramı Mesajları)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RuleName { get; set; }
        
        /// <summary>
        /// Başlangıç tarih ve saati
        /// </summary>
        [Required]
        public DateTime StartDateTime { get; set; }
        
        /// <summary>
        /// Bitiş tarih ve saati
        /// </summary>
        [Required]
        public DateTime EndDateTime { get; set; }
        
        /// <summary>
        /// Haftalık tekrar eden kural mı?
        /// </summary>
        public bool IsRecurring { get; set; }
        
        /// <summary>
        /// Tekrarlayan günler (1=Pazartesi, 7=Pazar) - Örnek format: "1,2,5"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string RecurringDays { get; set; }
        
        /// <summary>
        /// Ekran açık/kapalı durumu
        /// </summary>
        public bool ScreenStatus { get; set; }
        
        /// <summary>
        /// Tam ekran mesaj ID
        /// </summary>
        public int? FullScreenMessageId { get; set; }
        
        /// <summary>
        /// Kayan mesaj ID
        /// </summary>
        public int? ScrollingScreenMessageId { get; set; }
        
        /// <summary>
        /// Bitmap mesaj ID
        /// </summary>
        public int? BitmapScreenMessageId { get; set; }
        
        /// <summary>
        /// Kuralın öncelik seviyesi
        /// </summary>
        [Required]
        public RulePriority Priority { get; set; }
        
        /// <summary>
        /// Kural açıklaması
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Güncellenme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
} 