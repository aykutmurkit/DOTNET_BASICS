using Data.Context;
using Data.Seeding;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Tam ekran mesaj verilerini yükler
    /// </summary>
    public class FullScreenMessageSeeder : ISeeder
    {
        // SeederOrder'a göre AlignmentTypeSeeder'dan sonra çalışmalı
        public int Order => 8;
       
        public async Task SeedAsync(AppDbContext context)
        {
            // Zaten mesaj var mı kontrol et
            if (await context.FullScreenMessages.AnyAsync())
            {
                return; // Eğer veri varsa işlem yapma
            }

            // AlignmentType'ların var olduğunu kontrol et
            if (!await context.AlignmentTypes.AnyAsync())
            {
                throw new Exception("Hizalama türleri bulunamadı. Önce AlignmentTypeSeeder çalıştırılmalı.");
            }

            // Türkçe ve İngilizce örnek mesaj içerikleri
            var turkishMessages = new[]
            {
                new
                {
                    Line1 = "Dikkat!",
                    Line2 = "Şehir Hatları seferleri",
                    Line3 = "kötü hava koşulları nedeniyle",
                    Line4 = "iptal edilmiştir."
                },
                new
                {
                    Line1 = "Duyuru!",
                    Line2 = "Teknik bakım çalışması",
                    Line3 = "nedeniyle hizmet",
                    Line4 = "verilememektedir."
                },
                new
                {
                    Line1 = "Önemli Bilgilendirme",
                    Line2 = "İstasyon çıkışları",
                    Line3 = "geçici olarak",
                    Line4 = "kapatılmıştır."
                }
            };

            var englishMessages = new[]
            {
                new
                {
                    Line1 = "Attention!",
                    Line2 = "City Lines ferries",
                    Line3 = "have been cancelled due to",
                    Line4 = "severe weather conditions."
                },
                new
                {
                    Line1 = "Notice!",
                    Line2 = "Service is unavailable",
                    Line3 = "due to technical",
                    Line4 = "maintenance work."
                },
                new
                {
                    Line1 = "Important Information",
                    Line2 = "Station exits",
                    Line3 = "are temporarily",
                    Line4 = "closed."
                }
            };

            // 3 cihaz için tam ekran mesajlar ekleyelim
            // Önce cihazları alalım (ilk 3 cihaz)
            var devices = await context.Devices.Take(3).ToListAsync();
            if (devices.Count == 0)
            {
                return; // Cihaz yoksa işlem yapma
            }

            // AlignmentTypes'ları alalım
            var alignmentTypes = await context.AlignmentTypes.ToListAsync();
            
            // SQL komutu ile IDENTITY_INSERT kullanarak ID'leri belirterek veri ekleme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [FullScreenMessages] ON;");
            
            for (int i = 0; i < Math.Min(devices.Count, 3); i++)
            {
                // İlk iki mesaj için Center (ID: 1), üçüncü mesaj için Left (ID: 2) hizalama türü kullan
                int alignmentTypeId = i < 2 ? 1 : 2;
                
                queryBuilder.AppendLine($@"
                INSERT INTO [FullScreenMessages] 
                (
                    [Id], [TurkishLine1], [TurkishLine2], [TurkishLine3], [TurkishLine4], 
                    [EnglishLine1], [EnglishLine2], [EnglishLine3], [EnglishLine4], 
                    [AlignmentTypeId], [CreatedAt], [DeviceId]
                ) 
                VALUES 
                (
                    {i + 1}, 
                    N'{turkishMessages[i].Line1}', 
                    N'{turkishMessages[i].Line2}', 
                    N'{turkishMessages[i].Line3}', 
                    N'{turkishMessages[i].Line4}', 
                    N'{englishMessages[i].Line1}', 
                    N'{englishMessages[i].Line2}', 
                    N'{englishMessages[i].Line3}', 
                    N'{englishMessages[i].Line4}', 
                    {alignmentTypeId}, 
                    GETDATE(), 
                    {devices[i].Id}
                );");
            }
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [FullScreenMessages] OFF;");
            
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context cache'ini temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 