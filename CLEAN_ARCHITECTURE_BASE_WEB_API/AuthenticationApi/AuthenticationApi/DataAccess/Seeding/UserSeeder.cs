using Core.Security;
using Data.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    /// <summary>
    /// Kullanıcılar için seed data
    /// </summary>
    public class UserSeeder : ISeeder
    {
        /// <summary>
        /// Kullanıcılar rolleri ekledikten sonra oluşturulmalıdır
        /// </summary>
        public int Order => 2;

        public async Task SeedAsync(AppDbContext context)
        {
            // Kullanıcılar zaten varsa işlem yapma
            if (await context.Users.AnyAsync())
            {
                return;
            }

            // Tüm kullanıcıları tek seferde ekle
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [Users] ON;");
            queryBuilder.AppendLine("INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [PasswordSalt], [RoleId], [CreatedDate], [TwoFactorEnabled], [TwoFactorCodeExpirationMinutes]) VALUES");

            // Kullanıcı bilgileri
            var users = new List<(string username, string email, string password, int roleId, int id)>
            {
                ("admin", "admin@example.com", "Admin123!", 3, 1),
                ("developer", "developer@example.com", "Developer123!", 2, 2),
                ("user", "user@example.com", "User123!", 1, 3),
                ("aykut", "aykutmurkit.dev@gmail.com", "aykut1234*", 3, 4)
            };

            // SQL komutunu oluştur
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                string salt = PasswordHelper.CreateSalt();
                string passwordHash = PasswordHelper.HashPassword(user.password, salt);
                string createdDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                queryBuilder.Append($"({user.id}, '{user.username}', '{user.email}', '{passwordHash}', '{salt}', {user.roleId}, '{createdDate}', 0, 10)");
                
                if (i < users.Count - 1)
                    queryBuilder.AppendLine(",");
                else
                    queryBuilder.AppendLine(";");
            }

            queryBuilder.AppendLine("SET IDENTITY_INSERT [Users] OFF;");

            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context cache'ini yenile
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
} 