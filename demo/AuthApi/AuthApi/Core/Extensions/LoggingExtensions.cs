using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApi.Core.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds the logging services to the service collection - Sadece konsol loglama
        /// </summary>
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            // MongoDB ve RequestResponseLogging bağlantıları kaldırıldı
            // Log repositories
            // services.AddSingleton<ILogRepository, MongoLogRepository>();
            
            // Log services
            // services.AddSingleton<IApiLogService, ApiLogService>();
            
            // HTTP context accessor for accessing request context in service layer
            services.AddHttpContextAccessor();
            
            return services;
        }
        
        // UseCoreRequestResponseLogging metodu kaldırıldı
    }
} 