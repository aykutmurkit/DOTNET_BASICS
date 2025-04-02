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
            queryBuilder.AppendLine("INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [PasswordSalt], [RoleId], [CreatedDate], [TwoFactorEnabled], [TwoFactorCodeExpirationMinutes], [FirstName], [LastName], [PhoneNumber], [Language]) VALUES");

            // Kullanıcı bilgileri
            var users = new List<(string username, string email, string password, int roleId, int id, string firstName, string lastName, string phoneNumber, string language)>
            {
                ("admin", "admin@example.com", "Admin123!", 3, 1, "Admin", "User", "+905551112233", "tr"),
                ("developer", "developer@example.com", "Developer123!", 2, 2, "Developer", "User", "+905551112244", "en"),
                ("user", "user@example.com", "User123!", 1, 3, "Test", "User", "+905551112255", "tr"),
                ("aykut", "aykutmurkit.dev@gmail.com", "aykut1234*", 3, 4, "Aykut", "Murkit", "+905551112266", "tr")
            };

            // SQL komutunu oluştur
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                string salt = PasswordHelper.CreateSalt();
                string passwordHash = PasswordHelper.HashPassword(user.password, salt);
                string createdDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                queryBuilder.Append($"({user.id}, '{user.username}', '{user.email}', '{passwordHash}', '{salt}', {user.roleId}, '{createdDate}', 0, 10, '{user.firstName}', '{user.lastName}', '{user.phoneNumber}', '{user.language}')");
                
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