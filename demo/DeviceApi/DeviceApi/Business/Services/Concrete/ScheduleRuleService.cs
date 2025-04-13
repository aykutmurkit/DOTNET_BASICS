using AutoMapper;
using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Zamanlanmış kurallar için servis implementasyonu
    /// </summary>
    public class ScheduleRuleService : IScheduleRuleService
    {
        private readonly IScheduleRuleRepository _scheduleRuleRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleRuleService> _logger;
        private readonly ILogService _logService;

        public ScheduleRuleService(
            IScheduleRuleRepository scheduleRuleRepository,
            IDeviceRepository deviceRepository,
            IMapper mapper,
            ILogger<ScheduleRuleService> logger,
            ILogService logService)
        {
            _scheduleRuleRepository = scheduleRuleRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _logger = logger;
            _logService = logService;
        }

        public async Task<List<ScheduleRuleDto>> GetAllRulesAsync()
        {
            var rules = await _scheduleRuleRepository.GetAllAsync();
            return _mapper.Map<List<ScheduleRuleDto>>(rules);
        }

        public async Task<ScheduleRuleDto> GetRuleByIdAsync(int id)
        {
            var rule = await _scheduleRuleRepository.GetByIdAsync(id);
            return _mapper.Map<ScheduleRuleDto>(rule);
        }

        public async Task<List<ScheduleRuleDto>> GetRulesByDeviceIdAsync(int deviceId)
        {
            var rules = await _scheduleRuleRepository.GetByDeviceIdAsync(deviceId);
            return _mapper.Map<List<ScheduleRuleDto>>(rules);
        }

        public async Task<ScheduleRuleDto> CreateRuleAsync(CreateScheduleRuleDto createScheduleRuleDto)
        {
            // Cihazın var olup olmadığını kontrol et
            var device = await _deviceRepository.GetByIdAsync(createScheduleRuleDto.DeviceId);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural oluşturuluyor",
                "ScheduleRuleService.CreateRuleAsync",
                new { DeviceId = createScheduleRuleDto.DeviceId, RuleName = createScheduleRuleDto.RuleName });

            // DTO'yu entity'ye dönüştür
            var scheduleRule = _mapper.Map<ScheduleRule>(createScheduleRuleDto);
            
            // Zamanlanmış kural oluştur
            var createdRule = await _scheduleRuleRepository.AddAsync(scheduleRule);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural oluşturuldu",
                "ScheduleRuleService.CreateRuleAsync",
                new { RuleId = createdRule.Id, DeviceId = createdRule.DeviceId, RuleName = createdRule.RuleName });
            
            return _mapper.Map<ScheduleRuleDto>(createdRule);
        }

        public async Task<ScheduleRuleDto> UpdateRuleAsync(int id, UpdateScheduleRuleDto updateScheduleRuleDto)
        {
            // Kuralın var olup olmadığını kontrol et
            var existingRule = await _scheduleRuleRepository.GetByIdAsync(id);
            
            // Cihazın var olup olmadığını kontrol et
            var device = await _deviceRepository.GetByIdAsync(updateScheduleRuleDto.DeviceId);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural güncelleniyor",
                "ScheduleRuleService.UpdateRuleAsync",
                new { RuleId = id, DeviceId = updateScheduleRuleDto.DeviceId, RuleName = updateScheduleRuleDto.RuleName });
            
            // DTO'yu entity'ye dönüştür ve ID ata
            var scheduleRule = _mapper.Map<ScheduleRule>(updateScheduleRuleDto);
            scheduleRule.Id = id;
            
            // Zamanlanmış kuralı güncelle
            var updatedRule = await _scheduleRuleRepository.UpdateAsync(scheduleRule);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural güncellendi",
                "ScheduleRuleService.UpdateRuleAsync",
                new { RuleId = updatedRule.Id, DeviceId = updatedRule.DeviceId, RuleName = updatedRule.RuleName });
            
            return _mapper.Map<ScheduleRuleDto>(updatedRule);
        }

        public async Task DeleteRuleAsync(int id)
        {
            // Kuralın var olup olmadığını kontrol et
            var existingRule = await _scheduleRuleRepository.GetByIdAsync(id);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural siliniyor",
                "ScheduleRuleService.DeleteRuleAsync",
                new { RuleId = id, DeviceId = existingRule.DeviceId, RuleName = existingRule.RuleName });
            
            // Zamanlanmış kuralı sil
            await _scheduleRuleRepository.DeleteAsync(id);
            
            await _logService.LogInfoAsync(
                "Zamanlanmış kural silindi",
                "ScheduleRuleService.DeleteRuleAsync",
                new { RuleId = id });
        }

        public async Task ApplyActiveRulesAsync()
        {
            try
            {
                _logger.LogInformation("Tüm cihazlar için aktif kurallar uygulanıyor...");
                
                // Önce tüm null RecurringDays değerlerini düzeltelim
                await _scheduleRuleRepository.FixNullRecurringDaysAsync();
                
                var currentDateTime = DateTime.Now;
                var devices = await _deviceRepository.GetAllAsync();
                
                foreach (var device in devices)
                {
                    await ApplyActiveRulesForDeviceAsync(device.Id);
                }
                
                _logger.LogInformation("Tüm cihazlar için aktif kurallar başarıyla uygulandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif kurallar uygulanırken hata oluştu");
                await _logService.LogErrorAsync(
                    "Aktif kurallar uygulanırken hata oluştu",
                    "ScheduleRuleService.ApplyActiveRulesAsync",
                    ex);
                throw;
            }
        }

        public async Task ApplyActiveRulesForDeviceAsync(int deviceId)
        {
            try
            {
                _logger.LogInformation("Cihaz ID {DeviceId} için aktif kurallar uygulanıyor...", deviceId);
                
                // Önce tüm null RecurringDays değerlerini düzeltelim
                await _scheduleRuleRepository.FixNullRecurringDaysAsync();
                
                var currentDateTime = DateTime.Now;
                
                // Cihazı getir
                var device = await _deviceRepository.GetByIdWithStatusAsync(deviceId);
                if (device == null)
                {
                    throw new Exception($"Cihaz bulunamadı. ID: {deviceId}");
                }
                
                // Aktif kuralları getir (öncelik sırasına göre)
                var activeRules = await _scheduleRuleRepository.GetActiveRulesForDeviceAsync(deviceId, currentDateTime);
                
                if (activeRules.Any())
                {
                    // En yüksek öncelikli kuralı al
                    var highestPriorityRule = activeRules.First();
                    
                    _logger.LogInformation(
                        "Cihaz ID {DeviceId} için en yüksek öncelikli kural uygulanıyor: {RuleName} (ID: {RuleId})", 
                        deviceId, highestPriorityRule.RuleName, highestPriorityRule.Id);
                    
                    // Cihaz durumunu güncelle
                    if (device.Status == null)
                    {
                        device.Status = new DeviceStatus
                        {
                            DeviceId = deviceId,
                            CreatedAt = DateTime.Now
                        };
                    }
                    
                    // Cihaz durumunu güncelle
                    device.Status.ScreenStatus = highestPriorityRule.ScreenStatus;
                    device.Status.UpdatedAt = DateTime.Now;
                    
                    // Mesaj ID'lerini güncelle
                    device.FullScreenMessageId = highestPriorityRule.FullScreenMessageId;
                    device.ScrollingScreenMessageId = highestPriorityRule.ScrollingScreenMessageId;
                    device.BitmapScreenMessageId = highestPriorityRule.BitmapScreenMessageId;
                    
                    // Mesaj türleri için durumları güncelle
                    device.Status.FullScreenMessageStatus = highestPriorityRule.FullScreenMessageId.HasValue;
                    device.Status.ScrollingScreenMessageStatus = highestPriorityRule.ScrollingScreenMessageId.HasValue;
                    device.Status.BitmapScreenMessageStatus = highestPriorityRule.BitmapScreenMessageId.HasValue;
                    
                    // Cihazı güncelle
                    await _deviceRepository.UpdateAsync(device);
                    
                    await _logService.LogInfoAsync(
                        $"Cihaz ID {deviceId} için kural uygulandı",
                        "ScheduleRuleService.ApplyActiveRulesForDeviceAsync",
                        new { 
                            DeviceId = deviceId,
                            RuleId = highestPriorityRule.Id,
                            RuleName = highestPriorityRule.RuleName,
                            ScreenStatus = highestPriorityRule.ScreenStatus
                        });
                }
                else
                {
                    _logger.LogInformation("Cihaz ID {DeviceId} için aktif kural bulunamadı", deviceId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz ID {DeviceId} için aktif kurallar uygulanırken hata oluştu", deviceId);
                await _logService.LogErrorAsync(
                    $"Cihaz ID {deviceId} için aktif kurallar uygulanırken hata oluştu",
                    "ScheduleRuleService.ApplyActiveRulesForDeviceAsync",
                    ex);
                throw;
            }
        }
    }
} 