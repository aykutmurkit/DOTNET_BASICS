using deneme.Middleware;
using deneme.Services.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace deneme.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds the logging services to the service collection
        /// </summary>
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            // Log repositories
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            
            // Log services - Changed from scoped to singleton to fix middleware dependency issue
            services.AddSingleton<IApiLogService, ApiLogService>();
            
            // HTTP context accessor for accessing request context in service layer
            services.AddHttpContextAccessor();
            
            return services;
        }
        
        /// <summary>
        /// Adds the request/response logging middleware to the pipeline
        /// </summary>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            
            return app;
        }
    }
} 