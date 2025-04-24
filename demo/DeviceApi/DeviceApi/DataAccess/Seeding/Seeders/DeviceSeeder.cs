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
        public int Order => (int)SeederOrder.Devices; // 7
        
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

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] ON;");
            queryBuilder.AppendLine("INSERT INTO [Devices] ([Id], [Name], [Ip], [Port], [IMEI], [Latitude], [Longitude], [PlatformId]) VALUES");

            // Sadece Yusufpaşa istasyonundaki platformlar için cihaz tanımlıyoruz (PlatformId 1 ve 2)
            var devices = new List<(int id, string name, string ip, int port, string imei, double latitude, double longitude, int platformId)>();
            
            // Sabit IMEI numaraları (test için)
            var testImeis = new string[] {
                "356158061391111",
                "356158061392222"
            };

            // Yusufpaşa istasyonundaki platformlar için cihaz ekle
            for (int i = 0; i < 2; i++)
            {
                int platformId = i + 1; // PlatformId 1 ve 2
                int deviceId = i + 1;
                
                // Sabit test IMEI numarası kullan
                string imei = testImeis[i];
                
                // Farklı IP adres formatları oluştur
                string ip = $"192.168.{deviceId}.{10 + deviceId}";
                
                // Yusufpaşa platformları
                var platform = platforms.FirstOrDefault(p => p.Id == platformId);
                if (platform != null)
                {
                    devices.Add((
                        deviceId, 
                        $"Yusufpaşa Cihaz {platformId}", 
                        ip, 
                        8000 + deviceId,
                        imei,
                        platform.Latitude + 0.0002, 
                        platform.Longitude + 0.0002, 
                        platformId
                    ));
                }
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