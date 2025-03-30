using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;

namespace Core.Extensions
{
    public static class RateLimitExtensions
    {
        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rateLimitSettings = configuration.GetSection("RateLimitSettings");
            
            if (!rateLimitSettings.Exists() || !rateLimitSettings.GetValue<bool>("EnableGlobalRateLimit", false))
            {
                return services;
            }

            services.AddRateLimiter(options =>
            {
                // Global Rate Limit Policy
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = rateLimitSettings.GetValue<int>("GlobalRateLimitRequests", 200),
                            Window = ParsePeriod(rateLimitSettings.GetValue<string>("GlobalRateLimitPeriod", "1m"))
                        });
                });

                // IP-based Rate Limiting
                var ipRateLimitingSection = rateLimitSettings.GetSection("IpRateLimiting");
                if (ipRateLimitingSection.GetValue<bool>("EnableIpRateLimiting", false))
                {
                    options.AddPolicy("ip", httpContext =>
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = ipRateLimitingSection.GetValue<int>("IpRateLimitRequests", 100),
                                Window = ParsePeriod(ipRateLimitingSection.GetValue<string>("IpRateLimitPeriod", "1m"))
                            });
                    });
                }

                // Endpoint-specific Rate Limiting Policies
                var endpointLimits = rateLimitSettings.GetSection("EndpointLimits").Get<List<EndpointRateLimitConfig>>() ?? new List<EndpointRateLimitConfig>();
                foreach (var endpointLimit in endpointLimits)
                {
                    string policyName = GetPolicyNameFromEndpoint(endpointLimit.Endpoint);
                    
                    if (endpointLimit.EnableConcurrencyLimit)
                    {
                        options.AddConcurrencyLimiter(policyName, options =>
                        {
                            options.PermitLimit = endpointLimit.ConcurrencyLimit > 0 ? endpointLimit.ConcurrencyLimit : 10;
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            options.QueueLimit = 5;
                        });
                    }
                    else
                    {
                        options.AddFixedWindowLimiter(policyName, options =>
                        {
                            options.PermitLimit = endpointLimit.Limit;
                            options.Window = ParsePeriod(endpointLimit.Period);
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            options.QueueLimit = 2;
                        });
                    }
                }

                // Rejection response
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterSeconds)
                        ? retryAfterSeconds.TotalSeconds
                        : 60;
                        
                    context.HttpContext.Response.Headers.Append("Retry-After", ((int)retryAfter).ToString());
                    
                    var response = new
                    {
                        statusCode = StatusCodes.Status429TooManyRequests,
                        isSuccess = false,
                        message = "İstek limiti aşıldı. Lütfen daha sonra tekrar deneyin.",
                        retryAfter = (int)retryAfter
                    };
                    
                    await context.HttpContext.Response.WriteAsJsonAsync(response, token);
                };
            });

            return services;
        }

        private static TimeSpan ParsePeriod(string period)
        {
            if (string.IsNullOrEmpty(period))
                return TimeSpan.FromMinutes(1);

            var value = int.Parse(period.Substring(0, period.Length - 1));
            var unit = period[period.Length - 1];

            return unit switch
            {
                's' => TimeSpan.FromSeconds(value),
                'm' => TimeSpan.FromMinutes(value),
                'h' => TimeSpan.FromHours(value),
                'd' => TimeSpan.FromDays(value),
                _ => TimeSpan.FromMinutes(value)
            };
        }

        private static string GetPolicyNameFromEndpoint(string endpoint)
        {
            // Endpoint adını / karakterleri olmadan daha uygun bir policy ismine çevir
            return endpoint.Trim('/').Replace("/", "_").ToLowerInvariant();
        }

        public static IEnumerable<string> GetEndpointPolicyMappings(IConfiguration configuration)
        {
            var endpointLimits = configuration.GetSection("RateLimitSettings:EndpointLimits")
                .Get<List<EndpointRateLimitConfig>>() ?? new List<EndpointRateLimitConfig>();

            return endpointLimits.Select(el => new
            {
                Endpoint = el.Endpoint,
                Policy = GetPolicyNameFromEndpoint(el.Endpoint)
            }).Select(mapping => $"{mapping.Endpoint} -> {mapping.Policy}");
        }
    }

    public class EndpointRateLimitConfig
    {
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public int Limit { get; set; }
        public bool EnableConcurrencyLimit { get; set; }
        public int ConcurrencyLimit { get; set; }
    }
} 