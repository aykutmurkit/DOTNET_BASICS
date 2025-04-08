using Data.Context;
using Data.Seeding;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DeviceApi.DataAccess.Seeding
{
    /// <summary>
    /// Hizalama türleri için seed verisi ekleyen sınıf
    /// </summary>
    public class AlignmentTypeSeeder : ISeeder
    {
        // Seeder order değeri (seeder çalışma sırası)
        public int Order => 2; // Tüm entity türlerinden önce çalışsın
        
        /// <summary>
        /// Hizalama türlerini seed eder
        /// </summary>
        public async Task SeedAsync(AppDbContext context)
        {
            // Eğer zaten veri varsa işlem yapma
            if (await context.AlignmentTypes.AnyAsync())
            {
                return;
            }
            
            // SQL komutu ile IDENTITY_INSERT kullanarak ID'leri belirterek veri ekleme
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [AlignmentTypes] ON;");
            
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES (1, 0, 'Center');");
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES (2, 1, 'Left');");
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES (3, 2, 'Right');");
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES (4, 3, 'Top');");
            queryBuilder.AppendLine("INSERT INTO [AlignmentTypes] ([Id], [Key], [Name]) VALUES (5, 4, 'Bottom');");
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [AlignmentTypes] OFF;");
            
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context cache'ini temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 