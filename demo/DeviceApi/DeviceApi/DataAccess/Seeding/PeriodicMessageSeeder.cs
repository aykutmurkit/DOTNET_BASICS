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

            var queryBuilder = new StringBuilder();
            
            // IDENTITY_INSERT'i aç (ID'leri belirtebilmek için)
            queryBuilder.AppendLine("SET IDENTITY_INSERT [PeriodicMessages] ON;");
            
            // Örnek veriler
            queryBuilder.AppendLine(@"
INSERT INTO [PeriodicMessages] ([Id], [TemperatureLevel], [HumidityLevel], [GasLevel], [FrontLightLevel], [BackLightLevel], [LedFailureCount], [CabinStatus], [FanStatus], [ShowStatus], [Rs232Status], [PowerSupplyStatus], [CreatedAt], [ForecastedAt ], [DeviceId])
VALUES (1, 28, 65, 12, 80, 75, 0, 1, 1, 1, 1, 1, GETDATE(), NULL, 1);

INSERT INTO [PeriodicMessages] ([Id], [TemperatureLevel], [HumidityLevel], [GasLevel], [FrontLightLevel], [BackLightLevel], [LedFailureCount], [CabinStatus], [FanStatus], [ShowStatus], [Rs232Status], [PowerSupplyStatus], [CreatedAt], [ForecastedAt ], [DeviceId])
VALUES (2, 29, 70, 15, 90, 85, 2, 1, 1, 1, 0, 1, GETDATE(), NULL, 2);

INSERT INTO [PeriodicMessages] ([Id], [TemperatureLevel], [HumidityLevel], [GasLevel], [FrontLightLevel], [BackLightLevel], [LedFailureCount], [CabinStatus], [FanStatus], [ShowStatus], [Rs232Status], [PowerSupplyStatus], [CreatedAt], [ForecastedAt ], [DeviceId])
VALUES (3, 26, 62, 10, 75, 70, 1, 1, 0, 1, 1, 1, GETDATE(), NULL, 3);
            ");
            
            // IDENTITY_INSERT'i kapat
            queryBuilder.AppendLine("SET IDENTITY_INSERT [PeriodicMessages] OFF;");
            
            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context'te cached entity'leri temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 