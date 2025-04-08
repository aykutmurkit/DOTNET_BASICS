using DeviceApi.Business.Services.Concrete;
using DeviceApi.Business.Services.Interfaces;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceApi.Business.Extensions
{
    /// <summary>
    /// Servis koleksiyonu için extension metodları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Business katmanı servislerini ekler
        /// </summary>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Kullanıcı kimlik doğrulama ve yetkilendirme servisleri
            services.AddScoped<IAuthService, AuthService>();
            
            // İstasyon, Platform ve Cihaz servisleri
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IPlatformService, PlatformService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceSettingsService, DeviceSettingsService>();
            services.AddScoped<IDeviceStatusService, DeviceStatusService>();
            services.AddScoped<IScrollingScreenMessageService, ScrollingScreenMessageService>();
            services.AddScoped<IBitmapScreenMessageService, BitmapScreenMessageService>();
            services.AddScoped<IPeriodicMessageService, PeriodicMessageService>();
            
            // Yeni eklenen servisler
            services.AddScoped<IFullScreenMessageService, FullScreenMessageService>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IAlignmentTypeService, AlignmentTypeService>();
            
            // Utility servisleri
            services.AddScoped<EmailService>();
            
            return services;
        }
    }
} 