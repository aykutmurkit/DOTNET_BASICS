using Data.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using LogLibrary.Core.Interfaces;

namespace Data.Seeding
{
    /// <summary>
    /// Veritabanı seed işlemlerini yönetir
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly ILogService _logService;

        public DatabaseSeeder(
            IServiceProvider serviceProvider, 
            ILogger<DatabaseSeeder> logger,
            ILogService logService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm seed işlemlerini gerçekleştirir
        /// </summary>
        public async Task SeedAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // ISeeder interface'ini implement eden tüm sınıfları bul
            var seeders = GetSeeders();

            foreach (var seeder in seeders.OrderBy(s => s.Order))
            {
                try
                {
                    // Console logger'ı da kullanalım (önceki kodu koruyalım)
                    _logger.LogInformation("Seeding: {SeederName} başlatılıyor...", seeder.GetType().Name);
                    
                    // LogLibrary ile log
                    await _logService.LogInfoAsync(
                        $"Seeding: {seeder.GetType().Name} başlatılıyor...",
                        "DatabaseSeeder.SeedAsync",
                        new { SeederName = seeder.GetType().Name, SeederOrder = seeder.Order });
                    
                    await seeder.SeedAsync(context);
                    
                    // Console logger'ı da kullanalım (önceki kodu koruyalım)
                    _logger.LogInformation("Seeding: {SeederName} başarıyla tamamlandı.", seeder.GetType().Name);
                    
                    // LogLibrary ile log
                    await _logService.LogInfoAsync(
                        $"Seeding: {seeder.GetType().Name} başarıyla tamamlandı.",
                        "DatabaseSeeder.SeedAsync",
                        new { SeederName = seeder.GetType().Name, SeederOrder = seeder.Order });
                }
                catch (Exception ex)
                {
                    // Console logger'ı da kullanalım (önceki kodu koruyalım)
                    _logger.LogError(ex, "Seeding: {SeederName} sırasında hata oluştu: {ErrorMessage}", 
                        seeder.GetType().Name, ex.Message);
                    
                    // LogLibrary ile hata logu
                    await _logService.LogErrorAsync(
                        $"Seeding: {seeder.GetType().Name} sırasında hata oluştu",
                        "DatabaseSeeder.SeedAsync",
                        ex);
                    
                    throw;
                }
            }
        }

        /// <summary>
        /// Uygulamadaki tüm ISeeder implementasyonlarını bulur ve örneklerini oluşturur
        /// </summary>
        private List<ISeeder> GetSeeders()
        {
            var seeders = new List<ISeeder>();
            
            // ISeeder interface'ini implement eden tüm tipleri bul
            var seederTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && typeof(ISeeder).IsAssignableFrom(t));

            // Her tip için bir örnek oluştur
            foreach (var seederType in seederTypes)
            {
                var seeder = Activator.CreateInstance(seederType) as ISeeder;
                if (seeder != null)
                {
                    seeders.Add(seeder);
                }
            }

            return seeders;
        }
    }
} 