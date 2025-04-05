using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// İstasyonlar için seed data
    /// </summary>
    public class StationSeeder : ISeeder
    {
        /// <summary>
        /// İstasyonlar, platform ve cihazlardan önce oluşturulmalıdır
        /// </summary>
        public int Order => 3; // UserRoles ve Users'dan sonra

        public async Task SeedAsync(AppDbContext context)
        {
            // İstasyonlar zaten varsa işlem yapma
            if (await context.Stations.AnyAsync())
            {
                return;
            }

            // SQL komutu oluşturma ve yürütme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] ON;");
            queryBuilder.AppendLine("INSERT INTO [Stations] ([Id], [Name], [Latitude], [Longitude]) VALUES");

            // İstasyon bilgileri
            var stations = new List<(int id, string name, double latitude, double longitude)>
            {
                (1, "Merkez İstasyon", 41.0082, 28.9784),
                (2, "Doğu İstasyonu", 41.0215, 29.0097),
                (3, "Batı İstasyonu", 40.9937, 28.9527)
            };

            // Değerleri ekle
            for (int i = 0; i < stations.Count; i++)
            {
                var (id, name, lat, lon) = stations[i];
                queryBuilder.Append($"({id}, '{name}', {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)})");
                
                if (i < stations.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] OFF;");

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