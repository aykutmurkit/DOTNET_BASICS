using Data.Configurations;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

            // Tüm yapılandırma sınıflarını otomatik olarak uygula
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Not: Seed data artık ayrı seeder sınıflarında yönetilmektedir
        }
    }
} 