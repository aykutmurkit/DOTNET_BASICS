using DeviceApi.Business.Services.Concrete;
using DeviceApi.Business.Services.Interfaces;
using Core.Security;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceApi.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Auth ve User servisleri
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            
            // İstasyon, Platform ve Cihaz servisleri
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IPlatformService, PlatformService>();
            services.AddScoped<IDeviceService, DeviceService>();
            
            // Security servisleri
            services.AddScoped<JwtHelper>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            
            // Utility servisleri
            services.AddScoped<EmailService>();
            
            return services;
        }
    }
} 