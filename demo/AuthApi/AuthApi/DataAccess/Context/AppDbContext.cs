using Data.Configurations;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data.Context
{
    /// <summary>
    /// Author        : Aykut Mürkit
    /// Title         : Ar-Ge Mühendisi
    /// Unit          : Akıllı Şehircilik ve Coğrafi Bilgi Sistemleri Şefliği
    /// Unit Chief    : Mahmut Bulut
    /// Department    : Ar-Ge Müdürlüğü
    /// Company       : İSBAK A.Ş. (İstanbul Büyükşehir Belediyesi)
    /// Email         : aykutmurkit@outlook.com / amurkit@isbak.com.tr
    /// CreatedDate   : 2025-04-16
    /// LastModified  : 2025-04-16
    /// 
    /// © 2025 İSBAK A.Ş. – Tüm hakları saklıdır.
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