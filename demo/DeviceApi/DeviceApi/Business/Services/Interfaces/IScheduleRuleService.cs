using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Zamanlanmış kurallar için servis arayüzü
    /// </summary>
    public interface IScheduleRuleService
    {
        /// <summary>
        /// Tüm zamanlanmış kuralları getirir
        /// </summary>
        Task<List<ScheduleRuleDto>> GetAllRulesAsync();
        
        /// <summary>
        /// ID'ye göre zamanlanmış kuralı getirir
        /// </summary>
        Task<ScheduleRuleDto> GetRuleByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre zamanlanmış kuralları getirir
        /// </summary>
        Task<List<ScheduleRuleDto>> GetRulesByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Yeni zamanlanmış kural oluşturur
        /// </summary>
        Task<ScheduleRuleDto> CreateRuleAsync(CreateScheduleRuleDto createScheduleRuleDto);
        
        /// <summary>
        /// Zamanlanmış kuralı günceller
        /// </summary>
        Task<ScheduleRuleDto> UpdateRuleAsync(int id, UpdateScheduleRuleDto updateScheduleRuleDto);
        
        /// <summary>
        /// Zamanlanmış kuralı siler
        /// </summary>
        Task DeleteRuleAsync(int id);
        
        /// <summary>
        /// Şu anki aktif kuralları uygular
        /// </summary>
        Task ApplyActiveRulesAsync();
        
        /// <summary>
        /// Belirli bir cihaz için şu anki aktif kuralları uygular
        /// </summary>
        Task ApplyActiveRulesForDeviceAsync(int deviceId);
    }
} 