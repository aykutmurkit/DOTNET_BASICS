using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Cihaz seed verilerini yükler
    /// </summary>
    public class DeviceSeeder : ISeeder
    {
        // Seeder çalışma sırası - önce Platform oluşmalı (2), sonra Device (3)
        public int Order => 5; // PlatformSeeder'dan (Order=4) sonra çalışsın

        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.Devices.AnyAsync())
            {
                return; // Zaten veri varsa işlem yapma
            }

            // Önce tüm platformları al
            var platforms = await context.Platforms.ToListAsync();
            if (!platforms.Any())
            {
                return; // Platformlar yoksa işlem yapma
            }

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] ON;");
            queryBuilder.AppendLine("INSERT INTO [Devices] ([Id], [Name], [Ip], [Port], [Latitude], [Longitude], [PlatformId]) VALUES");

            // Örnek cihaz verileri
            var devices = new List<(int id, string name, string ip, int port, double latitude, double longitude, int platformId)>();
            
            int deviceId = 1;
            // Her platform için 2 cihaz ekle
            foreach (var platform in platforms)
            {
                devices.Add((
                    deviceId++, 
                    $"Cihaz {platform.Id}-1", 
                    $"192.168.1.{deviceId * 10}", 
                    8000 + deviceId, 
                    platform.Latitude + 0.0005, 
                    platform.Longitude + 0.0005, 
                    platform.Id
                ));

                devices.Add((
                    deviceId++, 
                    $"Cihaz {platform.Id}-2", 
                    $"192.168.2.{deviceId * 10}", 
                    9000 + deviceId, 
                    platform.Latitude - 0.0005, 
                    platform.Longitude - 0.0005, 
                    platform.Id
                ));
            }

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

            try
            {
                // SQL komutunu çalıştır
                await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
                
                // Context cache'ini temizle
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeviceSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 