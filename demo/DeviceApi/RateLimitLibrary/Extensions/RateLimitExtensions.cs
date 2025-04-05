using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateLimitLibrary.Middleware;
using RateLimitLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;

namespace RateLimitLibrary.Extensions
{
    public static class RateLimitExtensions
    {
        /// <summary>
        /// Adds rate limiting services with configurations from RateLimitLibrarySettings.json
        /// </summary>
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // Load settings from the configuration (from RateLimitLibrarySettings.json)
            var libraryConfig = LoadLibraryConfiguration();
            
            // Global rate limiting etkinlik durumu
            bool enableGlobalRateLimit = libraryConfig.GetValue<bool>("RateLimitSettings:EnableGlobalRateLimit");
            string globalPeriod = libraryConfig.GetValue<string>("RateLimitSettings:GlobalRateLimitPeriod") ?? "1m";
            int globalRequests = libraryConfig.GetValue<int>("RateLimitSettings:GlobalRateLimitRequests");

            // IP bazlı rate limiting etkinlik durumu
            bool enableIpRateLimit = libraryConfig.GetValue<bool>("RateLimitSettings:IpRateLimiting:EnableIpRateLimiting");
            string ipPeriod = libraryConfig.GetValue<string>("RateLimitSettings:IpRateLimiting:IpRateLimitPeriod") ?? "1m";
            int ipRequests = libraryConfig.GetValue<int>("RateLimitSettings:IpRateLimiting:IpRateLimitRequests");

            // Endpoint bazlı rate limiting konfigürasyonlarını al
            var endpointLimits = libraryConfig.GetSection("RateLimitSettings:EndpointLimits")
                .Get<List<EndpointRateLimit>>() ?? new List<EndpointRateLimit>();

            // Override library settings with app's settings if provided
            if (configuration != null)
            {
                // Try to get application-specific overrides
                var appSettings = configuration.GetSection("RateLimitSettings").Get<RateLimitSettings>();
                if (appSettings != null)
                {
                    // Apply overrides if present in app configuration
                    if (configuration.GetSection("RateLimitSettings:EnableGlobalRateLimit").Exists())
                        enableGlobalRateLimit = appSettings.EnableGlobalRateLimit;
                    
                    if (configuration.GetSection("RateLimitSettings:GlobalRateLimitPeriod").Exists())
                        globalPeriod = appSettings.GlobalRateLimitPeriod;
                    
                    if (configuration.GetSection("RateLimitSettings:GlobalRateLimitRequests").Exists())
                        globalRequests = appSettings.GlobalRateLimitRequests;
                    
                    if (configuration.GetSection("RateLimitSettings:IpRateLimiting:EnableIpRateLimiting").Exists())
                        enableIpRateLimit = appSettings.IpRateLimiting.EnableIpRateLimiting;
                    
                    if (configuration.GetSection("RateLimitSettings:IpRateLimiting:IpRateLimitPeriod").Exists())
                        ipPeriod = appSettings.IpRateLimiting.IpRateLimitPeriod;
                    
                    if (configuration.GetSection("RateLimitSettings:IpRateLimiting:IpRateLimitRequests").Exists())
                        ipRequests = appSettings.IpRateLimiting.IpRateLimitRequests;

                    // Merge endpoint settings if present in app config
                    if (configuration.GetSection("RateLimitSettings:EndpointLimits").Exists())
                    {
                        var appEndpointLimits = configuration.GetSection("RateLimitSettings:EndpointLimits")
                            .Get<List<EndpointRateLimit>>();
                        
                        if (appEndpointLimits != null && appEndpointLimits.Any())
                        {
                            // Replace library endpoint settings with app's settings
                            endpointLimits = appEndpointLimits;
                        }
                    }
                }
            }

            // Bütün rate limit ayarlarını tek bir RateLimiter özelliğine ekleyin
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Global limit
                if (enableGlobalRateLimit)
                {
                    // Global limiter'ı doğrudan RateLimiter options'a ekleyin
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var timeWindow = ParsePeriod(globalPeriod);
                        return RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = globalRequests,
                            Window = timeWindow,
                            QueueLimit = 0
                        });
                    });
                }

                // IP bazlı limit
                if (enableIpRateLimit)
                {
                    options.AddPolicy("ip", context =>
                    {
                        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        var timeWindow = ParsePeriod(ipPeriod);

                        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = ipRequests,
                            Window = timeWindow,
                            QueueLimit = 0
                        });
                    });
                }

                // Endpoint spesifik rate limit'ler
                foreach (var endpointLimit in endpointLimits)
                {
                    var policyName = $"endpoint_{endpointLimit.Endpoint.Replace('/', '_')}";
                    var timeWindow = ParsePeriod(endpointLimit.Period);

                    if (endpointLimit.EnableConcurrencyLimit)
                    {
                        options.AddConcurrencyLimiter(policyName, options =>
                        {
                            options.PermitLimit = endpointLimit.ConcurrencyLimit;
                            options.QueueLimit = 0;
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        });
                    }
                    else
                    {
                        options.AddFixedWindowLimiter(policyName, options =>
                        {
                            options.PermitLimit = endpointLimit.Limit;
                            options.Window = timeWindow;
                            options.QueueLimit = 0;
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        });
                    }
                }
            });

            return services;
        }

        /// <summary>
        /// Adds rate limiting middleware to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            // Add ASP.NET Core's rate limiter middleware
            app.UseRateLimiter();
            
            // Add our custom middleware that can add additional logic if needed
            app.UseMiddleware<RateLimitMiddleware>();
            
            return app;
        }

        /// <summary>
        /// Gets a list of endpoints configured for rate limiting
        /// </summary>
        public static List<string> GetRateLimitPolicyMappings(IConfiguration configuration)
        {
            var result = new List<string>();
            
            // Try to load from both the library settings and app settings
            var libraryConfig = LoadLibraryConfiguration();
            
            var endpointLimits = libraryConfig.GetSection("RateLimitSettings:EndpointLimits")
                .Get<List<EndpointRateLimit>>() ?? new List<EndpointRateLimit>();

            // Override with app settings if available
            if (configuration != null && configuration.GetSection("RateLimitSettings:EndpointLimits").Exists())
            {
                var appEndpointLimits = configuration.GetSection("RateLimitSettings:EndpointLimits")
                    .Get<List<EndpointRateLimit>>();
                
                if (appEndpointLimits != null && appEndpointLimits.Any())
                {
                    // Replace library endpoint settings with app's settings
                    endpointLimits = appEndpointLimits;
                }
            }
            
            foreach (var limit in endpointLimits)
            {
                var policyName = $"endpoint_{limit.Endpoint.Replace('/', '_')}";
                var limitType = limit.EnableConcurrencyLimit ? "Concurrency" : "Rate";
                var limitValue = limit.EnableConcurrencyLimit ? limit.ConcurrencyLimit.ToString() : $"{limit.Limit}/{limit.Period}";
                
                result.Add($"Endpoint: {limit.Endpoint} - {limitType} Limit: {limitValue}");
            }
            
            return result;
        }

        private static TimeSpan ParsePeriod(string period)
        {
            if (string.IsNullOrEmpty(period))
                return TimeSpan.FromMinutes(1); // Varsayılan olarak 1 dakika

            int value = int.Parse(period.Substring(0, period.Length - 1));
            char unit = period[period.Length - 1];

            return unit switch
            {
                's' => TimeSpan.FromSeconds(value),
                'm' => TimeSpan.FromMinutes(value),
                'h' => TimeSpan.FromHours(value),
                'd' => TimeSpan.FromDays(value),
                _ => TimeSpan.FromMinutes(value) // Bilinmeyen birim için varsayılan olarak dakika
            };
        }

        /// <summary>
        /// Loads the configuration from RateLimitLibrarySettings.json
        /// </summary>
        private static IConfiguration LoadLibraryConfiguration()
        {
            // Find the settings file in the library directory
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("RateLimitLibrarySettings.json", optional: true, reloadOnChange: true);
            
            return configBuilder.Build();
        }
    }
} 