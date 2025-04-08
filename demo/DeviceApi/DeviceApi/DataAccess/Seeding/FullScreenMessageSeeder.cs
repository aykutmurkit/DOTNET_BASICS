using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Seeding
{
    /// <summary>
    /// Tam ekran mesaj verilerini yükler
    /// </summary>
    public class FullScreenMessageSeeder : ISeeder
    {
       
        public async Task SeedAsync(AppDbContext context)
        {
            // Zaten mesaj var mı kontrol et
            if (await context.FullScreenMessages.AnyAsync())
            {
                return; // Eğer veri varsa işlem yapma
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

            var fullScreenMessages = new List<FullScreenMessage>();

            for (int i = 0; i < Math.Min(devices.Count, 3); i++)
            {
                fullScreenMessages.Add(new FullScreenMessage
                {
                    DeviceId = devices[i].Id,
                    TurkishLine1 = turkishMessages[i].Line1,
                    TurkishLine2 = turkishMessages[i].Line2,
                    TurkishLine3 = turkishMessages[i].Line3,
                    TurkishLine4 = turkishMessages[i].Line4,
                    EnglishLine1 = englishMessages[i].Line1,
                    EnglishLine2 = englishMessages[i].Line2,
                    EnglishLine3 = englishMessages[i].Line3,
                    EnglishLine4 = englishMessages[i].Line4,
                    CreatedAt = DateTime.Now
                });
            }

            await context.FullScreenMessages.AddRangeAsync(fullScreenMessages);
            await context.SaveChangesAsync();
        }
    }
} 