using Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthenticationApi.Core.Extensions
{
    /// <summary>
    /// Graylog servisi entegrasyonu için extension metotları
    /// </summary>
    public static class GraylogExtensions
    {
        /// <summary>
        /// Graylog loglama hizmetini ekler
        /// </summary>
        public static ILoggingBuilder AddGraylog(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<ILoggerProvider, GraylogLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Uygulamanın log yapılandırmasını yapar
        /// </summary>
        public static IServiceCollection AddGraylogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                
                // Graylog loglama
                builder.AddGraylog(configuration);
                
                // Konsol loglama (geliştirme ortamında)
                if (configuration.GetValue<bool>("Logging:EnableConsoleLogging", false))
                {
                    builder.AddConsole();
                }
                
                // Minimum log seviyesini ayarla
                var minimumLevel = configuration.GetValue<LogLevel>("Logging:LogLevel:Default", LogLevel.Information);
                builder.SetMinimumLevel(minimumLevel);
            });
            
            return services;
        }
    }
} 