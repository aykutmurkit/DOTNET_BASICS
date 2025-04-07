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
        /// İstasyonlar tablosu
        /// </summary>
        public DbSet<Station> Stations { get; set; }
        
        /// <summary>
        /// Platformlar tablosu
        /// </summary>
        public DbSet<Platform> Platforms { get; set; }
        
        /// <summary>
        /// Cihazlar tablosu
        /// </summary>
        public DbSet<Device> Devices { get; set; }

        /// <summary>
        /// Cihaz Ayarları tablosu
        /// </summary>
        public DbSet<DeviceSettings> DeviceSettings { get; set; }

        /// <summary>
        /// Tren Tahminleri tablosu
        /// </summary>
        public DbSet<Prediction> Predictions { get; set; }

        /// <summary>
        /// Tam Ekran Mesajlar tablosu
        /// </summary>
        public DbSet<FullScreenMessage> FullScreenMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tüm yapılandırma sınıflarını otomatik olarak uygula
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Not: Seed data artık ayrı seeder sınıflarında yönetilmektedir
        }
    }
} 