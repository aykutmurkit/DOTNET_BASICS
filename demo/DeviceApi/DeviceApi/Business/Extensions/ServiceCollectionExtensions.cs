using DeviceApi.Business.Services.Concrete;
using DeviceApi.Business.Services.Interfaces;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceApi.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Kullanıcı kimlik doğrulama ve yetkilendirme servisleri
            services.AddScoped<IAuthService, AuthService>();
            
            // İstasyon, Platform ve Cihaz servisleri
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IPlatformService, PlatformService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceSettingsService, DeviceSettingsService>();
            services.AddScoped<IScrollingScreenMessageService, ScrollingScreenMessageService>();
            
            // Utility servisleri
            services.AddScoped<EmailService>();
            
            return services;
        }
    }
} 