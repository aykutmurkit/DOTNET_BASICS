using Data.Context;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions
{
    /// <summary>
    /// Servis koleksiyonu için extension metotları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Veritabanı bağlantı ve servislerini ekler
        /// </summary>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Veritabanı bağlantısını ekle
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Seeding servisleri
            services.AddTransient<DatabaseSeeder>();

            return services;
        }
    }
} 