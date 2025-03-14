using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Repositories;
using test.Services;

namespace test.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds database context to the service collection
        /// </summary>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        /// <summary>
        /// Adds repository services to the service collection
        /// </summary>
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IRepositoryNonGeneric, RepositoryNonGeneric>();

            return services;
        }

        /// <summary>
        /// Adds application services to the service collection
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Generic Services
            services.AddScoped<IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>, DeviceService>();
            services.AddScoped<IService<ApnName, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>>, ApnNameService>();
            services.AddScoped<IService<ApnPassword, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>>, ApnPasswordService>();
            services.AddScoped<IService<ApnAddress, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>>, ApnAddressService>();

            // Register Non Generic Services
            services.AddScoped<IServiceNonGeneric, StationService>();

            return services;
        }
    }
} 