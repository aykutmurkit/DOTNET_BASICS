using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeviceApi.TCPListener.Core;
using DeviceApi.TCPListener.Models;
using DeviceApi.TCPListener.Services;

namespace DeviceApi.TCPListener.Extensions
{
    /// <summary>
    /// IServiceCollection için eklenti metotlarını içeren sınıf
    /// </summary>
    public static class TcpServiceCollectionExtensions
    {
        /// <summary>
        /// TCP Listener servisini ve gerekli bağımlılıkları kaydeder
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <param name="configuration">Yapılandırma nesnesi</param>
        /// <returns>Servis koleksiyonu</returns>
        public static IServiceCollection AddTcpListener(this IServiceCollection services, IConfiguration configuration)
        {
            // TcpListenerSettings'i DI container'a kaydet
            var tcpListenerSection = configuration.GetSection("TcpListenerSettings");
            
            if (tcpListenerSection.Exists())
            {
                services.Configure<TcpListenerSettings>(tcpListenerSection);
            }
            else
            {
                // Varsayılan ayarlar
                services.Configure<TcpListenerSettings>(options =>
                {
                    options.Port = 3456;
                    options.IpAddress = "0.0.0.0";
                    options.MaxConnections = 100;
                    options.ConnectionTimeout = 30000;
                    options.BufferSize = 1024;
                    options.StartChar = '^';
                    options.DelimiterChar = '+';
                    options.EndChar = '~';
                });
            }

            // Cihaz doğrulama servisini kaydet
            services.AddSingleton<IDeviceVerificationService, DeviceVerificationService>();

            // MessageHandler'ı kaydet
            services.AddSingleton<MessageHandler>();

            // TCP Listener servisini kaydet
            services.AddSingleton<ITcpListenerService, TcpListenerService>();
            services.AddHostedService(provider => (TcpListenerService)provider.GetRequiredService<ITcpListenerService>());

            return services;
        }
    }
} 