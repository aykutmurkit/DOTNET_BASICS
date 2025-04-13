using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Zamanlanmış kurallar için repository arayüzü
    /// </summary>
    public interface IScheduleRuleRepository
    {
        /// <summary>
        /// Tüm zamanlanmış kuralları getirir
        /// </summary>
        Task<List<ScheduleRule>> GetAllAsync();
        
        /// <summary>
        /// ID'ye göre zamanlanmış kuralı getirir
        /// </summary>
        Task<ScheduleRule> GetByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre zamanlanmış kuralları getirir
        /// </summary>
        Task<List<ScheduleRule>> GetByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Zamanlanmış kural ekler
        /// </summary>
        Task<ScheduleRule> AddAsync(ScheduleRule scheduleRule);
        
        /// <summary>
        /// Zamanlanmış kuralı günceller
        /// </summary>
        Task<ScheduleRule> UpdateAsync(ScheduleRule scheduleRule);
        
        /// <summary>
        /// Zamanlanmış kuralı siler
        /// </summary>
        Task DeleteAsync(int id);
        
        /// <summary>
        /// Şu anki tarihe göre aktif olan kuralları getirir
        /// </summary>
        Task<List<ScheduleRule>> GetActiveRulesAsync(DateTime currentDateTime);
        
        /// <summary>
        /// Bir cihaz için şu anki tarihe göre aktif olan kuralları getirir
        /// </summary>
        Task<List<ScheduleRule>> GetActiveRulesForDeviceAsync(int deviceId, DateTime currentDateTime);
        
        /// <summary>
        /// Veritabanındaki tüm kurallar için RecurringDays alanının null olmamasını sağlar
        /// </summary>
        Task FixNullRecurringDaysAsync();
    }
} 