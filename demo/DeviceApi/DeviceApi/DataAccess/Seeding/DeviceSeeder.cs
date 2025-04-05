using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Cihazlar için seed data
    /// </summary>
    public class DeviceSeeder : ISeeder
    {
        /// <summary>
        /// Cihazlar, diğer varlıklardan sonra oluşturulmalıdır
        /// </summary>
        public int Order => 5; // PlatformSeeder'dan sonra

        public async Task SeedAsync(AppDbContext context)
        {
            // Cihazlar zaten varsa işlem yapma
            if (await context.Devices.AnyAsync())
            {
                return;
            }

            // SQL komutu oluşturma ve yürütme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] ON;");
            queryBuilder.AppendLine("INSERT INTO [Devices] ([Id], [Name], [Ip], [Port], [Latitude], [Longitude], [PlatformId]) VALUES");

            // Cihaz bilgileri
            var devices = new List<(int id, string name, string ip, int port, double latitude, double longitude, int platformId)>
            {
                // Platform 1 cihazları (Merkez İstasyon - Platform 1)
                (1, "Kamera 1", "192.168.1.101", 8080, 41.0081, 28.9781, 1),
                (2, "Sensör 1", "192.168.1.102", 8081, 41.0082, 28.9782, 1),
                
                // Platform 2 cihazları (Merkez İstasyon - Platform 2)
                (3, "Kamera 2", "192.168.1.103", 8082, 41.0083, 28.9784, 2),
                (4, "Sensör 2", "192.168.1.104", 8083, 41.0084, 28.9785, 2),
                
                // Platform 3 cihazları (Doğu İstasyonu - Platform 1)
                (5, "Kamera 3", "192.168.1.105", 8084, 41.0213, 29.0095, 3),
                (6, "Sensör 3", "192.168.1.106", 8085, 41.0214, 29.0096, 3),
                
                // Platform 4 cihazları (Doğu İstasyonu - Platform 2)
                (7, "Kamera 4", "192.168.1.107", 8086, 41.0215, 29.0097, 4),
                (8, "Sensör 4", "192.168.1.108", 8087, 41.0216, 29.0098, 4),
                
                // Platform 5 cihazları (Batı İstasyonu - Platform 1)
                (9, "Kamera 5", "192.168.1.109", 8088, 40.9935, 28.9525, 5),
                (10, "Sensör 5", "192.168.1.110", 8089, 40.9936, 28.9526, 5),
                
                // Platform 6 cihazları (Batı İstasyonu - Platform 2)
                (11, "Kamera 6", "192.168.1.111", 8090, 40.9937, 28.9527, 6),
                (12, "Sensör 6", "192.168.1.112", 8091, 40.9938, 28.9528, 6)
            };

            // Değerleri ekle
            for (int i = 0; i < devices.Count; i++)
            {
                var (id, name, ip, port, lat, lon, platformId) = devices[i];
                queryBuilder.Append($"({id}, '{name}', '{ip}', {port}, {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {platformId})");
                
                if (i < devices.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] OFF;");

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