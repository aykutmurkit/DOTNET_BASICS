using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Platformlar için seed data
    /// </summary>
    public class PlatformSeeder : ISeeder
    {
        /// <summary>
        /// Platformlar, istasyonlardan sonra cihazlardan önce oluşturulmalıdır
        /// </summary>
        public int Order => 4; // StationSeeder'dan sonra

        public async Task SeedAsync(AppDbContext context)
        {
            // Platformlar zaten varsa işlem yapma
            if (await context.Platforms.AnyAsync())
            {
                return;
            }

            // SQL komutu oluşturma ve yürütme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Platforms] ON;");
            queryBuilder.AppendLine("INSERT INTO [Platforms] ([Id], [Latitude], [Longitude], [StationId]) VALUES");

            // Platform bilgileri
            var platforms = new List<(int id, double latitude, double longitude, int stationId)>
            {
                // Merkez İstasyon platformları
                (1, 41.0082, 28.9782, 1),
                (2, 41.0083, 28.9785, 1),
                
                // Doğu İstasyonu platformları
                (3, 41.0214, 29.0096, 2),
                (4, 41.0216, 29.0098, 2),
                
                // Batı İstasyonu platformları
                (5, 40.9936, 28.9526, 3),
                (6, 40.9938, 28.9528, 3)
            };

            // Değerleri ekle
            for (int i = 0; i < platforms.Count; i++)
            {
                var (id, lat, lon, stationId) = platforms[i];
                queryBuilder.Append($"({id}, {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {stationId})");
                
                if (i < platforms.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Platforms] OFF;");

            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context cache'ini temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 