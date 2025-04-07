using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCPListenerLibrary.Core;
using TCPListenerLibrary.Models;
using TCPListenerLibrary.Services;

namespace TCPListenerLibrary.Extensions
{
    /// <summary>
    /// IServiceCollection için eklenti metotlarını içeren sınıf
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// TCP Listener servisini ve gerekli bağımlılıkları kaydeder
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <param name="configuration">Yapılandırma nesnesi</param>
        /// <returns>Servis koleksiyonu</returns>
        public static IServiceCollection AddTcpListener(this IServiceCollection services, IConfiguration configuration)
        {
            // Ayarlardan TCP Listener yapılandırmasını yükle
            var tcpListenerConfig = configuration.LoadTcpListenerSettings();
            
            // TcpListenerSettings'i DI container'a kaydet
            services.Configure<TcpListenerSettings>(tcpListenerConfig.GetSection("TcpListenerSettings"));

            // MessageHandler'ı kaydet
            services.AddSingleton<MessageHandler>();

            // TCP Listener servisini kaydet
            services.AddSingleton<ITcpListenerService, TcpListenerService>();
            services.AddHostedService(provider => (TcpListenerService)provider.GetRequiredService<ITcpListenerService>());

            return services;
        }
    }
} 