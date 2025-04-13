using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Seeding
{
    /// <summary>
    /// Cihaz ayarları için seed verileri
    /// </summary>
    public class DeviceSettingsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext context)
        {
            // Cihaz ayarları zaten varsa işlem yapma
            if (await context.DeviceSettings.AnyAsync())
            {
                return;
            }
            
            // Önce cihazları kontrol et
            var devices = await context.Devices.ToListAsync();
            if (!devices.Any())
            {
                throw new Exception("Cihazlar bulunamadı. Önce DeviceSeeder çalıştırılmalıdır.");
            }
            
            // 3 örnek cihaz için ayarları hazırla
            var deviceSettings = new List<DeviceSettings>();
            
            // İlk cihaz için ayarlar (varsa)
            if (devices.Count >= 1)
            {
                deviceSettings.Add(new DeviceSettings
                {
                    ApnName = "internet.turkcell",
                    ApnUsername = "turkcell",
                    ApnPassword = "secure123",
                    ServerIP = "185.56.145.1",
                    TcpPort = 8080,
                    UdpPort = 9090,
                    FtpStatus = true,
                    DeviceId = devices[0].Id
                });
            }
            
            // İkinci cihaz için ayarlar (varsa)
            if (devices.Count >= 2)
            {
                deviceSettings.Add(new DeviceSettings
                {
                    ApnName = "vodafone.net",
                    ApnUsername = "vodafone",
                    ApnPassword = "voda456",
                    ServerIP = "193.140.13.1",
                    TcpPort = 8181,
                    UdpPort = 9191,
                    FtpStatus = false,
                    DeviceId = devices[1].Id
                });
            }
            
            // Üçüncü cihaz için ayarlar (varsa)
            if (devices.Count >= 3)
            {
                deviceSettings.Add(new DeviceSettings
                {
                    ApnName = "avea.web",
                    ApnUsername = "ttnet",
                    ApnPassword = "tt789",
                    ServerIP = "194.27.12.1",
                    TcpPort = 8282,
                    UdpPort = 9292,
                    FtpStatus = true,
                    DeviceId = devices[2].Id
                });
            }
            
            // Veritabanına ekle
            try
            {
                await context.DeviceSettings.AddRangeAsync(deviceSettings);
                await context.SaveChangesAsync();
                Console.WriteLine($"{deviceSettings.Count} adet cihaz ayarı başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeviceSettingsSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 