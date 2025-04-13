using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    /// <summary>
    /// Zamanlanmış kurallar için repository implementasyonu
    /// </summary>
    public class ScheduleRuleRepository : IScheduleRuleRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ScheduleRuleRepository> _logger;

        public ScheduleRuleRepository(AppDbContext context, ILogger<ScheduleRuleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ScheduleRule>> GetAllAsync()
        {
            return await _context.ScheduleRules
                .Include(r => r.Device)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.StartDateTime)
                .ToListAsync();
        }

        public async Task<ScheduleRule> GetByIdAsync(int id)
        {
            var rule = await _context.ScheduleRules
                .Include(r => r.Device)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (rule == null)
            {
                throw new Exception($"Zamanlanmış kural bulunamadı. ID: {id}");
            }
            
            return rule;
        }

        public async Task<List<ScheduleRule>> GetByDeviceIdAsync(int deviceId)
        {
            return await _context.ScheduleRules
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.StartDateTime)
                .ToListAsync();
        }

        public async Task<ScheduleRule> AddAsync(ScheduleRule scheduleRule)
        {
            scheduleRule.CreatedAt = DateTime.Now;
            
            await _context.ScheduleRules.AddAsync(scheduleRule);
            await _context.SaveChangesAsync();
            
            return scheduleRule;
        }

        public async Task<ScheduleRule> UpdateAsync(ScheduleRule scheduleRule)
        {
            var existingRule = await _context.ScheduleRules.FindAsync(scheduleRule.Id);
            
            if (existingRule == null)
            {
                throw new Exception($"Zamanlanmış kural bulunamadı. ID: {scheduleRule.Id}");
            }
            
            // Varolan kuralın değerlerini güncelle
            existingRule.RuleName = scheduleRule.RuleName;
            existingRule.DeviceId = scheduleRule.DeviceId;
            existingRule.StartDateTime = scheduleRule.StartDateTime;
            existingRule.EndDateTime = scheduleRule.EndDateTime;
            existingRule.IsRecurring = scheduleRule.IsRecurring;
            existingRule.RecurringDays = scheduleRule.RecurringDays;
            existingRule.ScreenStatus = scheduleRule.ScreenStatus;
            existingRule.FullScreenMessageId = scheduleRule.FullScreenMessageId;
            existingRule.ScrollingScreenMessageId = scheduleRule.ScrollingScreenMessageId;
            existingRule.BitmapScreenMessageId = scheduleRule.BitmapScreenMessageId;
            existingRule.Priority = scheduleRule.Priority;
            existingRule.Description = scheduleRule.Description;
            existingRule.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            return existingRule;
        }

        public async Task DeleteAsync(int id)
        {
            var rule = await _context.ScheduleRules.FindAsync(id);
            
            if (rule == null)
            {
                throw new Exception($"Zamanlanmış kural bulunamadı. ID: {id}");
            }
            
            _context.ScheduleRules.Remove(rule);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduleRule>> GetActiveRulesAsync(DateTime currentDateTime)
        {
            var allRules = await _context.ScheduleRules.ToListAsync();
            var activeRules = new List<ScheduleRule>();
            
            foreach (var rule in allRules)
            {
                if (IsRuleActive(rule, currentDateTime))
                {
                    activeRules.Add(rule);
                }
            }
            
            return activeRules
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.StartDateTime)
                .ToList();
        }

        public async Task<List<ScheduleRule>> GetActiveRulesForDeviceAsync(int deviceId, DateTime currentDateTime)
        {
            var deviceRules = await _context.ScheduleRules
                .Where(r => r.DeviceId == deviceId)
                .ToListAsync();
                
            var activeRules = new List<ScheduleRule>();
            
            foreach (var rule in deviceRules)
            {
                if (IsRuleActive(rule, currentDateTime))
                {
                    activeRules.Add(rule);
                }
            }
            
            return activeRules
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.StartDateTime)
                .ToList();
        }

        /// <summary>
        /// Veritabanındaki tüm kurallar için RecurringDays alanının null olmamasını sağlar
        /// </summary>
        public async Task FixNullRecurringDaysAsync()
        {
            var rules = await _context.ScheduleRules.ToListAsync();
            var hasChanges = false;
            
            foreach (var rule in rules)
            {
                if (string.IsNullOrEmpty(rule.RecurringDays))
                {
                    _logger.LogWarning("Kural ID {RuleId} için null RecurringDays değeri düzeltiliyor", rule.Id);
                    
                    // IsRecurring true ise her gün (1-7), false ise tek seferlik (0)
                    rule.RecurringDays = rule.IsRecurring ? "1,2,3,4,5,6,7" : "0";
                    hasChanges = true;
                }
            }
            
            if (hasChanges)
            {
                _logger.LogInformation("Null RecurringDays değerleri düzeltildi, değişiklikler kaydediliyor...");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Değişiklikler başarıyla kaydedildi.");
            }
            else
            {
                _logger.LogInformation("Null RecurringDays değeri bulunamadı.");
            }
        }
        
        /// <summary>
        /// Bir kuralın belirli bir tarih ve saatte aktif olup olmadığını kontrol eder
        /// </summary>
        private bool IsRuleActive(ScheduleRule rule, DateTime currentDateTime)
        {
            // Tekrarlayan olmayan kurallar için basit tarih aralığı kontrolü
            if (!rule.IsRecurring)
            {
                return currentDateTime >= rule.StartDateTime && currentDateTime <= rule.EndDateTime;
            }
            
            // Tekrarlayan kurallar için
            // Tarih aralığı kontrolü
            if (currentDateTime < rule.StartDateTime || currentDateTime > rule.EndDateTime)
            {
                return false;
            }
            
            // Haftanın günü kontrolü
            if (!string.IsNullOrEmpty(rule.RecurringDays))
            {
                // RecurringDays formatı: "1,2,5" -> 1=Pazartesi, 7=Pazar
                var dayOfWeek = (int)currentDateTime.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7; // Pazar için 0 yerine 7 kullan
                
                var days = rule.RecurringDays.Split(',').Select(int.Parse).ToList();
                
                // Bugün belirtilen günlerden biri değilse, aktif değil
                if (!days.Contains(dayOfWeek))
                {
                    return false;
                }
            }
            else
            {
                // RecurringDays null veya boş ise, tekrarlayan kural için her gün olarak kabul et
                _logger.LogWarning("Kural ID {RuleId} için RecurringDays alanı null veya boş. Her gün aktif olarak kabul ediliyor.", rule.Id);
            }
            
            // Tüm kontrolleri geçtiyse kural aktiftir
            return true;
        }
    }
} 