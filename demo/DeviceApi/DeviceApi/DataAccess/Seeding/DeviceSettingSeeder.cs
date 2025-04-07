using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Cihaz ayarları için seed verileri
    /// </summary>
    public class DeviceSettingSeeder : ISeeder
    {
        /// <summary>
        /// Cihaz ayarları, cihazlardan sonra oluşturulmalıdır
        /// </summary>
        public int Order => 6; // DeviceSeeder'dan (Order=5) sonra çalışsın
        
        public async Task SeedAsync(AppDbContext context)
        {
            // Cihaz ayarları zaten varsa işlem yapma
            if (await context.DeviceSettings.AnyAsync())
            {
                return;
            }
            
            // Önce cihazları kontrol et
            var devices = await context.Devices.Take(3).ToListAsync();
            if (!devices.Any())
            {
                return; // Cihazlar yoksa işlem yapma
            }
            
            // 3 örnek cihaz için ayarları hazırla
            if (devices.Count >= 3)
            {
                // SQL komutu oluşturma
                var queryBuilder = new StringBuilder();
                queryBuilder.AppendLine("SET IDENTITY_INSERT [DeviceSettings] ON;");
                queryBuilder.AppendLine("INSERT INTO [DeviceSettings] ([Id], [ApnName], [ApnUsername], [ApnPassword], [ServerIP], [TcpPort], [UdpPort], [FtpStatus], [DeviceId]) VALUES");
                
                // Sabit örnek değerler
                var settingsData = new List<(int id, string apnName, string apnUsername, string apnPassword, string serverIP, int tcpPort, int udpPort, bool ftpStatus, int deviceId)>
                {
                    (1, "internet.turkcell", "turkcell", "secure123", "185.56.145.1", 8080, 9090, true, devices[0].Id),
                    (2, "vodafone.net", "vodafone", "voda456", "193.140.13.1", 8181, 9191, false, devices[1].Id),
                    (3, "avea.web", "ttnet", "tt789", "194.27.12.1", 8282, 9292, true, devices[2].Id)
                };
                
                // Değerleri ekle
                for (int i = 0; i < settingsData.Count; i++)
                {
                    var (id, apnName, apnUsername, apnPassword, serverIP, tcpPort, udpPort, ftpStatus, deviceId) = settingsData[i];
                    var ftpValue = ftpStatus ? "1" : "0";
                    
                    queryBuilder.Append($"({id}, '{apnName}', '{apnUsername}', '{apnPassword}', '{serverIP}', {tcpPort}, {udpPort}, {ftpValue}, {deviceId})");
                    
                    if (i < settingsData.Count - 1)
                    {
                        queryBuilder.AppendLine(",");
                    }
                    else
                    {
                        queryBuilder.AppendLine(";");
                    }
                }
                
                queryBuilder.AppendLine("SET IDENTITY_INSERT [DeviceSettings] OFF;");
                
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
                    Console.WriteLine($"DeviceSettingSeeder hatası: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 