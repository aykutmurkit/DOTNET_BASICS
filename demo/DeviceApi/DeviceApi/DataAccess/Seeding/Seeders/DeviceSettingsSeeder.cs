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
        // Order özelliği SeederOrder enum değeri kullanılarak tanımlandı
        public int Order => (int)SeederOrder.DeviceSettings;
        
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
            
            // 2 cihaz için ayarları hazırla
            var deviceSettings = new List<DeviceSettings>();
            
            // İlk cihaz için ayarlar (varsa)
            if (devices.Count >= 1)
            {
                deviceSettings.Add(new DeviceSettings
                {
                    ApnName = "internet.turkcell",
                    ApnUsername = "turkcell",
                    ApnPassword = "secure123",
                    ServerIp = "185.56.145.1",
                    TcpPort = 8080,
                    UdpPort = 9090,
                    FtpStatus = true,
                    FtpIp = "185.56.145.10",
                    FtpPort = 21,
                    FtpUsername = "ftpuser1",
                    FtpPassword = "ftppass1",
                    ConnectionTimeoutDuration = 30,
                    CommunicationHardwareVersion = "HW-v1.2.3",
                    CommunicationSoftwareVersion = "SW-v2.1.0",
                    GraphicsCardHardwareVersion = "GHW-v3.1",
                    GraphicsCardSoftwareVersion = "GSW-v3.2.1",
                    ScrollingTextSpeed = 5,
                    TramDisplayType = "LCD",
                    BusScreenPageCount = 3,
                    TimeDisplayFormat = "24h",
                    TramFont = "Arial",
                    ScreenVerticalPixelCount = 768,
                    ScreenHorizontalPixelCount = 1024,
                    TemperatureAlarmThreshold = 80,
                    HumidityAlarmThreshold = 90,
                    GasAlarmThreshold = 50,
                    LightSensorStatus = true,
                    LightSensorOperationLevel = 3,
                    LightSensorLevel1 = 100,
                    LightSensorLevel2 = 200,
                    LightSensorLevel3 = 300,
                    SocketType = "TCP",
                    StopName = "Yusufpaşa 1",
                    StartupLogoFilename = "logo_startup_1.png",
                    StartupLogoCrc16 = "ABCD1234",
                    VehicleLogoFilename = "vehicle_logo_1.png",
                    VehicleLogoCrc16 = "EFAB5678",
                    CommunicationType = "4G",
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
                    ServerIp = "193.140.13.1",
                    TcpPort = 8181,
                    UdpPort = 9191,
                    FtpStatus = true,
                    FtpIp = "193.140.13.10",
                    FtpPort = 22,
                    FtpUsername = "ftpuser2",
                    FtpPassword = "ftppass2",
                    ConnectionTimeoutDuration = 45,
                    CommunicationHardwareVersion = "HW-v2.0.1",
                    CommunicationSoftwareVersion = "SW-v2.5.0",
                    GraphicsCardHardwareVersion = "GHW-v3.5",
                    GraphicsCardSoftwareVersion = "GSW-v4.0.0",
                    ScrollingTextSpeed = 7,
                    TramDisplayType = "LED",
                    BusScreenPageCount = 5,
                    TimeDisplayFormat = "12h",
                    TramFont = "Tahoma",
                    ScreenVerticalPixelCount = 1080,
                    ScreenHorizontalPixelCount = 1920,
                    TemperatureAlarmThreshold = 85,
                    HumidityAlarmThreshold = 95,
                    GasAlarmThreshold = 60,
                    LightSensorStatus = true,
                    LightSensorOperationLevel = 4,
                    LightSensorLevel1 = 150,
                    LightSensorLevel2 = 250,
                    LightSensorLevel3 = 350,
                    SocketType = "UDP",
                    StopName = "Yusufpaşa 2",
                    StartupLogoFilename = "logo_startup_2.png",
                    StartupLogoCrc16 = "1A2B3C4D",
                    VehicleLogoFilename = "vehicle_logo_2.png",
                    VehicleLogoCrc16 = "5E6F7G8H",
                    CommunicationType = "5G",
                    DeviceId = devices[1].Id
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