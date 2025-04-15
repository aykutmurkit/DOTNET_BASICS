using Data.Context;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Periyodik mesajlar için örnek verileri ekler
    /// </summary>
    public class PeriodicMessageSeeder : ISeeder
    {
        // Order özelliği SeederOrder enum değeri kullanılarak tanımlandı
        public int Order => (int)SeederOrder.PeriodicMessages;
       
        /// <summary>
        /// Seed işlemini gerçekleştirir
        /// </summary>
        public async Task SeedAsync(AppDbContext context)
        {
            // Eğer PeriodicMessages tablosunda veri varsa çık
            if (await context.PeriodicMessages.AnyAsync())
            {
                return;
            }
            
            // Önce cihazların varlığını kontrol et
            var deviceIds = await context.Devices.Select(d => d.Id).ToListAsync();
            if (!deviceIds.Any())
            {
                throw new Exception("Cihazlar bulunamadı. Önce DeviceSeeder çalıştırılmalıdır.");
            }

            var queryBuilder = new StringBuilder();
            
            // IDENTITY_INSERT'i aç (ID'leri belirtebilmek için)
            queryBuilder.AppendLine("SET IDENTITY_INSERT [PeriodicMessages] ON;");
            
            // Örnek veriler - Sadece var olan cihazlar için (DeviceId 1 ve 2)
            queryBuilder.AppendLine(@"
INSERT INTO [PeriodicMessages] ([Id], [TemperatureLevel], [HumidityLevel], [GasLevel], [FrontLightLevel], [BackLightLevel], [LedFailureCount], [CabinStatus], [FanStatus], [ShowStatus], [Rs232Status], [PowerSupplyStatus], [CreatedAt], [ForecastedAt ], [DeviceId])
VALUES (1, 28, 65, 12, 80, 75, 0, 1, 1, 1, 1, 1, GETDATE(), NULL, 1);

INSERT INTO [PeriodicMessages] ([Id], [TemperatureLevel], [HumidityLevel], [GasLevel], [FrontLightLevel], [BackLightLevel], [LedFailureCount], [CabinStatus], [FanStatus], [ShowStatus], [Rs232Status], [PowerSupplyStatus], [CreatedAt], [ForecastedAt ], [DeviceId])
VALUES (2, 29, 70, 15, 90, 85, 2, 1, 1, 1, 0, 1, GETDATE(), NULL, 2);
            ");
            
            // IDENTITY_INSERT'i kapat
            queryBuilder.AppendLine("SET IDENTITY_INSERT [PeriodicMessages] OFF;");
            
            try
            {
                // SQL komutunu çalıştır
                await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
                
                // Context'te cached entity'leri temizle
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PeriodicMessageSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 