using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeviceApi.TCPListener.Core.Interfaces;
using DeviceApi.TCPListener.Core.Services;
using DeviceApi.TCPListener.Models.Configurations;

namespace DeviceApi.TCPListener.Extensions
{
    /// <summary>
    /// TCP Listener servisleri için extension metotları
    /// </summary>
    public static class TcpListenerServiceExtensions
    {
        /// <summary>
        /// TCP Listener servislerini dependency injection container'a ekler
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddTcpListener(this IServiceCollection services, IConfiguration configuration)
        {
            // Yapılandırmayı ekle
            services.Configure<TcpListenerSettings>(configuration.GetSection("TcpListenerSettings"));

            // Servisleri ekle
            services.AddSingleton<IDeviceVerificationService, DeviceVerificationService>();
            services.AddSingleton<IMessageHandler, MessageHandler>();
            
            // TcpListenerService'i hem IHostedService hem de ITcpListenerService olarak ekle
            services.AddSingleton<TcpListenerService>();
            services.AddSingleton<ITcpListenerService>(provider => provider.GetRequiredService<TcpListenerService>());
            services.AddHostedService(provider => provider.GetRequiredService<TcpListenerService>());

            return services;
        }
    }
} 