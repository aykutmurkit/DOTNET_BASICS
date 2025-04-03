using LogLib.Repositories;
using LogLib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LogLib.Core.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection to easily add LogLib services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all required LogLib services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="connectionString">MongoDB connection string</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddLogLib(this IServiceCollection services, string connectionString)
        {
            // Register MongoDB client
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
            
            // Register LogLib repository and service
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            services.AddSingleton<ILogService, LogService>();
            
            // Register HttpContextAccessor (needed for getting correlation ID, user info, etc.)
            services.AddHttpContextAccessor();
            
            return services;
        }
        
        /// <summary>
        /// Adds all required LogLib services to the dependency injection container using configuration
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddLogLib(this IServiceCollection services, IConfiguration configuration)
        {
            // Get MongoDB connection string from configuration
            var connectionString = configuration.GetConnectionString("MongoDb");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured. Please add 'ConnectionStrings:MongoDb' to your configuration.");
            }
            
            return AddLogLib(services, connectionString);
        }
    }
} 