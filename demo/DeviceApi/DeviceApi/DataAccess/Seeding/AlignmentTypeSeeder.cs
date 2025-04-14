using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Hizalama türü seed verilerini yükler
    /// </summary>
    public class AlignmentTypeSeeder : ISeeder
    {
        // SeederOrder enum değeri kullanılacak
        public int Order => (int)SeederOrder.AlignmentTypes;
        
        public async Task SeedAsync(AppDbContext context)
        {
            if (await context.AlignmentTypes.AnyAsync())
            {
                return; // Zaten veri varsa işlem yapma
            }

            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [AlignmentTypes] ON;");
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES");
            queryBuilder.AppendLine("(1, 1, 'Center'),");
            queryBuilder.AppendLine("(2, 2, 'Left'),");
            queryBuilder.AppendLine("(3, 3, 'Right');");
            queryBuilder.AppendLine("SET IDENTITY_INSERT [AlignmentTypes] OFF;");

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
                Console.WriteLine($"AlignmentTypeSeeder hatası: {ex.Message}");
                throw;
            }
        }
    }
} 