using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    /// <summary>
    /// Uygulama veritabanı bağlantı sınıfı
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Kullanıcılar tablosu
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Kullanıcı rolleri tablosu
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Burada veritabanı yapılandırmaları yapılabilir
            // Örneğin indeksler, ilişkiler vb.

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            // User ve UserRole arasındaki ilişki
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Rol silindiğinde kullanıcıyı silme
                
            // Seed data için roller
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, Name = "User" },
                new UserRole { Id = 2, Name = "Developer" },
                new UserRole { Id = 3, Name = "Admin" }
            );
        }
    }
} 