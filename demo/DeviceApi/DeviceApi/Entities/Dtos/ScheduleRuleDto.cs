using Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    /// <summary>
    /// Zamanlanmış kural bilgilerini taşıyan DTO
    /// </summary>
    public class ScheduleRuleDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string RuleName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringDays { get; set; }
        public bool ScreenStatus { get; set; }
        public int? FullScreenMessageId { get; set; }
        public int? ScrollingScreenMessageId { get; set; }
        public int? BitmapScreenMessageId { get; set; }
        public RulePriority Priority { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// Zamanlanmış kural oluşturma DTO
    /// </summary>
    public class CreateScheduleRuleDto
    {
        [Required(ErrorMessage = "Cihaz ID gereklidir.")]
        public int DeviceId { get; set; }
        
        [Required(ErrorMessage = "Kural adı gereklidir.")]
        [MaxLength(100, ErrorMessage = "Kural adı en fazla 100 karakter olabilir.")]
        public string RuleName { get; set; }
        
        [Required(ErrorMessage = "Başlangıç tarihi ve saati gereklidir.")]
        public DateTime StartDateTime { get; set; }
        
        [Required(ErrorMessage = "Bitiş tarihi ve saati gereklidir.")]
        public DateTime EndDateTime { get; set; }
        
        public bool IsRecurring { get; set; }
        
        [MaxLength(20, ErrorMessage = "Tekrarlayan günler en fazla 20 karakter olabilir.")]
        public string RecurringDays { get; set; }
        
        [Required(ErrorMessage = "Ekran durumu gereklidir.")]
        public bool ScreenStatus { get; set; }
        
        public int? FullScreenMessageId { get; set; }
        
        public int? ScrollingScreenMessageId { get; set; }
        
        public int? BitmapScreenMessageId { get; set; }
        
        [Required(ErrorMessage = "Öncelik seviyesi gereklidir.")]
        public RulePriority Priority { get; set; }
        
        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }
    }
    
    /// <summary>
    /// Zamanlanmış kural güncelleme DTO
    /// </summary>
    public class UpdateScheduleRuleDto
    {
        [Required(ErrorMessage = "Cihaz ID gereklidir.")]
        public int DeviceId { get; set; }
        
        [Required(ErrorMessage = "Kural adı gereklidir.")]
        [MaxLength(100, ErrorMessage = "Kural adı en fazla 100 karakter olabilir.")]
        public string RuleName { get; set; }
        
        [Required(ErrorMessage = "Başlangıç tarihi ve saati gereklidir.")]
        public DateTime StartDateTime { get; set; }
        
        [Required(ErrorMessage = "Bitiş tarihi ve saati gereklidir.")]
        public DateTime EndDateTime { get; set; }
        
        public bool IsRecurring { get; set; }
        
        [MaxLength(20, ErrorMessage = "Tekrarlayan günler en fazla 20 karakter olabilir.")]
        public string RecurringDays { get; set; }
        
        [Required(ErrorMessage = "Ekran durumu gereklidir.")]
        public bool ScreenStatus { get; set; }
        
        public int? FullScreenMessageId { get; set; }
        
        public int? ScrollingScreenMessageId { get; set; }
        
        public int? BitmapScreenMessageId { get; set; }
        
        [Required(ErrorMessage = "Öncelik seviyesi gereklidir.")]
        public RulePriority Priority { get; set; }
        
        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }
    }
} 