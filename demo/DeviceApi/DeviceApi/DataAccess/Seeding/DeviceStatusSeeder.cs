using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Cihaz durum bilgileri için seed verileri
    /// </summary>
    public class DeviceStatusSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext context)
        {
            // Cihaz durumları zaten varsa işlem yapma
            if (await context.DeviceStatuses.AnyAsync())
            {
                return;
            }
            
            // Önce cihazları kontrol et
            var devices = await context.Devices.ToListAsync();
            if (!devices.Any())
            {
                return; // Cihazlar yoksa işlem yapma
            }
            
            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [DeviceStatuses] ON;");
            queryBuilder.AppendLine("INSERT INTO [DeviceStatuses] ([Id], [FullScreenMessageStatus], [ScrollingScreenMessageStatus], [BitmapScreenMessageStatus], [CreatedAt], [UpdatedAt], [DeviceId]) VALUES");
            
            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var counter = 1;
            
            // Her cihaz için durum bilgisi oluştur
            for (int i = 0; i < devices.Count; i++)
            {
                // Random durum bilgileri oluştur (bazıları aktif bazıları pasif)
                var fullScreenStatus = i % 3 == 0 ? "1" : "0";
                var scrollingScreenStatus = i % 2 == 0 ? "1" : "0";
                var bitmapScreenStatus = i % 4 == 0 ? "1" : "0";
                
                queryBuilder.Append($"({counter}, {fullScreenStatus}, {scrollingScreenStatus}, {bitmapScreenStatus}, '{currentDateTime}', NULL, {devices[i].Id})");
                
                if (i < devices.Count - 1)
                {
                    queryBuilder.AppendLine(",");
                }
                else
                {
                    queryBuilder.AppendLine(";");
                }
                
                counter++;
            }
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [DeviceStatuses] OFF;");
            
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
                Console.WriteLine($"DeviceStatusSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 