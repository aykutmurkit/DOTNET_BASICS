using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Font türü seed verilerini yükler
    /// </summary>
    public class FontTypeSeeder : ISeeder
    {
        // SeederOrder enum değeri kullanılacak
        public int Order => (int)SeederOrder.FontTypes;
        
        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.FontTypes.AnyAsync())
            {
                return; // Zaten veri varsa işlem yapma
            }

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [FontTypes] ON;");
            queryBuilder.AppendLine("INSERT INTO [FontTypes] ([Id], [Key], [Name], [Width], [Height]) VALUES");
            queryBuilder.AppendLine("(1, 1, 'Small', 6, 10),");
            queryBuilder.AppendLine("(2, 2, 'Medium', 6, 10),");
            queryBuilder.AppendLine("(3, 3, 'Big', 6, 10);");
            queryBuilder.AppendLine("SET IDENTITY_INSERT [FontTypes] OFF;");

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
                Console.WriteLine($"FontTypeSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 