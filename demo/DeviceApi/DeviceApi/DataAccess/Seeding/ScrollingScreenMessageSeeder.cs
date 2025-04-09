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
INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt])
VALUES (1, 'İyi yolculuklar dileriz, lütfen sefer saati yaklaşan metro araçlarına öncelik veriniz.', 'Have a nice trip, please give priority to subway vehicles approaching departure time.', GETDATE(), NULL);

INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt])
VALUES (2, 'Metro seferleri 06:00-00:00 saatleri arasında hizmet vermektedir.', 'Metro services operate between 06:00-00:00.', GETDATE(), NULL);

INSERT INTO [ScrollingScreenMessages] ([Id], [TurkishLine], [EnglishLine], [CreatedAt], [UpdatedAt])
VALUES (3, 'Lütfen metro içerisinde maske takınız ve sosyal mesafeye dikkat ediniz.', 'Please wear a mask in the metro and pay attention to social distancing.', GETDATE(), NULL);
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
            
            // Cihazlara kayan ekran mesajlarını atama
            try {
                // Mesajları belirli cihazlara ata
                var updateQuery = new StringBuilder();
                updateQuery.AppendLine("UPDATE [Devices] SET [ScrollingScreenMessageId] = CASE");
                updateQuery.AppendLine("  WHEN [Id] % 3 = 1 THEN 1"); // ID'si 1, 4, 7, ... olan cihazlara 1. mesajı ata
                updateQuery.AppendLine("  WHEN [Id] % 3 = 2 THEN 2"); // ID'si 2, 5, 8, ... olan cihazlara 2. mesajı ata
                updateQuery.AppendLine("  WHEN [Id] % 3 = 0 THEN 3"); // ID'si 3, 6, 9, ... olan cihazlara 3. mesajı ata
                updateQuery.AppendLine("END");
                updateQuery.AppendLine("WHERE [Id] <= (SELECT COUNT(*) FROM [Devices])"); // Sadece mevcut cihazlar için
                
                // Güncelleme SQL komutunu çalıştır
                await context.Database.ExecuteSqlRawAsync(updateQuery.ToString());
                
                // Context'i tekrar temizle
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Cihazlara ScrollingScreenMessage atanırken hata: {ex.Message}");
            }
        }
    }
} 