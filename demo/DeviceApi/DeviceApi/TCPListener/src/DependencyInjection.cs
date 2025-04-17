using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeviceApi.TCPListener.Configuration;
using DeviceApi.TCPListener.Connection;
using DeviceApi.TCPListener.Messaging;
using DeviceApi.TCPListener.Security;

namespace DeviceApi.TCPListener
{
    /// <summary>
    /// TCP Listener için servis kayıt extensionları
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// TCP Listener servislerini ve bağımlılıklarını kaydeder
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
            services.AddSingleton<IDeviceVerifier, DeviceVerifier>();

            // MessageHandler'ı kaydet
            services.AddSingleton<IMessageHandler, MessageHandler>();

            // TCP Server servisini kaydet
            services.AddSingleton<ITcpServer, TcpServer>();
            services.AddHostedService(provider => (TcpServer)provider.GetRequiredService<ITcpServer>());

            return services;
        }
    }
} 