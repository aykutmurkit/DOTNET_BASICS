using Data.Context;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Kayan ekran mesajları için örnek verileri ekler
    /// </summary>
    public class ScrollingScreenMessageSeeder : ISeeder
    {
       
        /// <summary>
        /// Seed işlemini gerçekleştirir
        /// </summary>
        public async Task SeedAsync(AppDbContext context)
        {
            // Eğer ScrollingScreenMessages tablosunda veri varsa çık
            if (await context.ScrollingScreenMessages.AnyAsync())
            {
                return;
            }

            var queryBuilder = new StringBuilder();
            
            // IDENTITY_INSERT'i aç (ID'leri belirtebilmek için)
            queryBuilder.AppendLine("SET IDENTITY_INSERT [ScrollingScreenMessages] ON;");
            
            // Örnek veriler
            queryBuilder.AppendLine(@"
INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (1, 'İyi yolculuklar dileriz, lütfen sefer saati yaklaşan metro araçlarına öncelik veriniz.', 'Have a nice trip, please give priority to subway vehicles approaching departure time.', GETDATE(), NULL, 1);

INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (2, 'Metro seferleri 06:00-00:00 saatleri arasında hizmet vermektedir.', 'Metro services operate between 06:00-00:00.', GETDATE(), NULL, 2);

INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt], [DeviceId])
VALUES (3, 'Lütfen metro içerisinde maske takınız ve sosyal mesafeye dikkat ediniz.', 'Please wear a mask in the metro and pay attention to social distancing.', GETDATE(), NULL, 3);
            ");
            
            // IDENTITY_INSERT'i kapat
            queryBuilder.AppendLine("SET IDENTITY_INSERT [ScrollingScreenMessages] OFF;");
            
            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context'te cached entity'leri temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 