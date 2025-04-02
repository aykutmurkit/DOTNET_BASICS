using Data.Context;
using Data.Interfaces;
using Data.Repositories;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceApi.DataAccess.Extensions
{
    /// <summary>
    /// Servis koleksiyonu için extension metotları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Veritabanı bağlantı ve servislerini ekler
        /// </summary>
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Veritabanı bağlantısını ekle
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository Services
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<IPlatformRepository, PlatformRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();

            // Seeding servisleri
            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
} 