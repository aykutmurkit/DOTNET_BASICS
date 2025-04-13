using DeviceApi.Business.Services.Interfaces;
using LogLibrary.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Zamanlanmış kuralları arka planda periyodik olarak uygulayan servis
    /// </summary>
    public class ScheduleRuleBackgroundService : BackgroundService
    {
        private readonly ILogger<ScheduleRuleBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        // Kontrol aralığı (30 saniye)
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);
        
        public ScheduleRuleBackgroundService(
            ILogger<ScheduleRuleBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Schedule Rule Background Service başlatıldı");
            
            // Log servisini scope içinde kullan
            using (var scope = _serviceProvider.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                await logService.LogInfoAsync(
                    "Schedule Rule Background Service başlatıldı",
                    "ScheduleRuleBackgroundService.ExecuteAsync",
                    null);
            }
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Aktif kurallar kontrol ediliyor...");
                    
                    // Scoped servisler için scope oluştur
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduleRuleService = scope.ServiceProvider.GetRequiredService<IScheduleRuleService>();
                        
                        // Tüm cihazlar için aktif kuralları uygula
                        await scheduleRuleService.ApplyActiveRulesAsync();
                    }
                    
                    _logger.LogInformation("Aktif kurallar kontrol edildi. Bir sonraki kontrol {NextCheckTime}", 
                        DateTime.Now.Add(_checkInterval).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Aktif kurallar uygulanırken bir hata oluştu");
                    
                    // Log servisini scope içinde kullan
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                        await logService.LogErrorAsync(
                            "Aktif kurallar uygulanırken bir hata oluştu",
                            "ScheduleRuleBackgroundService.ExecuteAsync",
                            ex);
                    }
                }
                
                // Belirli aralıkla kontrol et
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Schedule Rule Background Service başlatılıyor...");
            
            // Log servisini scope içinde kullan
            using (var scope = _serviceProvider.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                await logService.LogInfoAsync(
                    "Schedule Rule Background Service başlatılıyor",
                    "ScheduleRuleBackgroundService.StartAsync",
                    null);
            }
                
            await base.StartAsync(cancellationToken);
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Schedule Rule Background Service durduruluyor...");
            
            // Log servisini scope içinde kullan
            using (var scope = _serviceProvider.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                await logService.LogInfoAsync(
                    "Schedule Rule Background Service durduruluyor",
                    "ScheduleRuleBackgroundService.StopAsync",
                    null);
            }
                
            await base.StopAsync(cancellationToken);
        }
    }
} 