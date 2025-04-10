using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DeviceApi.Business.Mappings.Extensions
{
    /// <summary>
    /// AutoMapper yapılandırmasını DI container'a ekleyen extension sınıfı
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AutoMapper servisini ve profil konfigürasyonlarını ekler
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <returns>Konfigüre edilmiş servis koleksiyonu</returns>
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}