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
        // SeederOrder enum değeriyle aynı olması için 5 olarak değiştirildi
        public int Order => (int)SeederOrder.Devices; // 5
        
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
                // Platformlar yoksa hata fırlat
                throw new Exception("Platformlar bulunamadı. Önce PlatformSeeder çalıştırılmalıdır.");
            }

            // Rastgele IMEI oluşturma fonksiyonu
            string GenerateRandomIMEI()
            {
                var random = new Random();
                var imei = new StringBuilder("35");
                for (int i = 0; i < 13; i++)
                {
                    imei.Append(random.Next(0, 10));
                }
                return imei.ToString();
            }

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] ON;");
            queryBuilder.AppendLine("INSERT INTO [Devices] ([Id], [Name], [Ip], [Port], [IMEI], [Latitude], [Longitude], [PlatformId]) VALUES");

            // Örnek cihaz verileri
            var devices = new List<(int id, string name, string ip, int port, string imei, double latitude, double longitude, int platformId)>();
            
            int deviceId = 1;
            // Her platform için 2 cihaz ekle
            foreach (var platform in platforms)
            {
                // İlk iki cihaz için belirli IMEI numaralarını kullan
                string imei;
                if (deviceId == 1)
                {
                    // İlk cihaz için: 356158061391111
                    imei = "356158061391111";
                }
                else if (deviceId == 2)
                {
                    // İkinci cihaz için: 356158061391113
                    imei = "356158061391113";
                }
                else
                {
                    // Diğer cihazlar için rastgele IMEI üret
                    imei = GenerateRandomIMEI();
                }
                
                devices.Add((
                    deviceId++, 
                    $"Cihaz {platform.Id}-1", 
                    $"192.168.1.{deviceId * 10}", 
                    8000 + deviceId,
                    imei,
                    platform.Latitude + 0.0005, 
                    platform.Longitude + 0.0005, 
                    platform.Id
                ));

                // İkinci cihazlar için de kontrol et
                if (deviceId == 2)
                {
                    // İkinci cihaz için: 356158061391113
                    imei = "356158061391113";
                }
                else
                {
                    // Diğer cihazlar için rastgele IMEI üret
                    imei = GenerateRandomIMEI();
                }

                devices.Add((
                    deviceId++, 
                    $"Cihaz {platform.Id}-2", 
                    $"192.168.2.{deviceId * 10}", 
                    9000 + deviceId,
                    imei,
                    platform.Latitude - 0.0005, 
                    platform.Longitude - 0.0005, 
                    platform.Id
                ));
            }

            // Değerleri ekle
            for (int i = 0; i < devices.Count; i++)
            {
                var (id, name, ip, port, imei, lat, lon, platformId) = devices[i];
                queryBuilder.Append($"({id}, '{name}', '{ip}', {port}, '{imei}', {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {platformId})");
                
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