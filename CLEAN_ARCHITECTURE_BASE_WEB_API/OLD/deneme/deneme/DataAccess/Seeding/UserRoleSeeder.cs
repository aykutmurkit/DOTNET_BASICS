using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Seeding
{
    /// <summary>
    /// Kullanıcı rolleri için seed data
    /// </summary>
    public class UserRoleSeeder : ISeeder
    {
        /// <summary>
        /// Roller, diğer verileri eklemeden önce oluşturulmalıdır
        /// </summary>
        public int Order => 1;

        public async Task SeedAsync(AppDbContext context)
        {
            // Roller zaten varsa işlem yapma
            if (await context.UserRoles.AnyAsync())
            {
                return;
            }

            // Doğrudan SQL kullanarak rolleri ekle - bu yaklaşım IDENTITY_INSERT ile ilgili sorunları aşar
            await context.Database.ExecuteSqlRawAsync(@"
                SET IDENTITY_INSERT [UserRoles] ON;
                
                INSERT INTO [UserRoles] ([Id], [Name])
                VALUES (1, 'User'), (2, 'Developer'), (3, 'Admin');
                
                SET IDENTITY_INSERT [UserRoles] OFF;
            ");
            
            // Context cache'ini yenile
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 