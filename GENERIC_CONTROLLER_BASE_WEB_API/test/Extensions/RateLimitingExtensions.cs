using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using test.Configuration;
using test.Middleware;

namespace test.Extensions
{
    /// <summary>
    /// Extension methods for rate limiting
    /// </summary>
    public static class RateLimitingExtensions
    {
        /// <summary>
        /// Adds rate limiting services to the service collection
        /// </summary>
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register memory cache if not already registered
            services.AddMemoryCache();

            // Configure rate limiting options
            services.Configure<RateLimitingConfiguration>(options =>
            {
                // Default values are set in the RateLimitingConfiguration class
                // Override with values from configuration if provided
                configuration.GetSection("RateLimiting").Bind(options);
            });

            return services;
        }

        /// <summary>
        /// Adds rate limiting middleware to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitingMiddleware>();
        }
    }
} 