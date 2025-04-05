using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;

namespace DeviceApi.Core.Extensions
{
    public static class RateLimitExtensions
    {
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Global rate limiting etkinlik durumu
            bool enableGlobalRateLimit = configuration.GetValue<bool>("RateLimitSettings:EnableGlobalRateLimit");
            string globalPeriod = configuration.GetValue<string>("RateLimitSettings:GlobalRateLimitPeriod") ?? "1m";
            int globalRequests = configuration.GetValue<int>("RateLimitSettings:GlobalRateLimitRequests");

            // IP bazlı rate limiting etkinlik durumu
            bool enableIpRateLimit = configuration.GetValue<bool>("RateLimitSettings:IpRateLimiting:EnableIpRateLimiting");
            string ipPeriod = configuration.GetValue<string>("RateLimitSettings:IpRateLimiting:IpRateLimitPeriod") ?? "1m";
            int ipRequests = configuration.GetValue<int>("RateLimitSettings:IpRateLimiting:IpRateLimitRequests");

            // Endpoint bazlı rate limiting konfigürasyonlarını al
            var endpointLimits = configuration.GetSection("RateLimitSettings:EndpointLimits")
                .Get<List<EndpointRateLimit>>() ?? new List<EndpointRateLimit>();

            var rateLimitOptions = new RateLimiterOptions();

            // Global limit
            if (enableGlobalRateLimit)
            {
                rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
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
                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

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
            }

            return services;
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

        public static List<string> GetEndpointPolicyMappings(IConfiguration configuration)
        {
            var result = new List<string>();
            var endpointLimits = configuration.GetSection("RateLimitSettings:EndpointLimits")
                .Get<List<EndpointRateLimit>>() ?? new List<EndpointRateLimit>();
            
            foreach (var limit in endpointLimits)
            {
                var policyName = $"endpoint_{limit.Endpoint.Replace('/', '_')}";
                var limitType = limit.EnableConcurrencyLimit ? "Concurrency" : "Rate";
                var limitValue = limit.EnableConcurrencyLimit ? limit.ConcurrencyLimit.ToString() : $"{limit.Limit}/{limit.Period}";
                
                result.Add($"Endpoint: {limit.Endpoint} - {limitType} Limit: {limitValue}");
            }
            
            return result;
        }
    }

    public class EndpointRateLimit
    {
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public int Limit { get; set; }
        public bool EnableConcurrencyLimit { get; set; }
        public int ConcurrencyLimit { get; set; }
    }
} 