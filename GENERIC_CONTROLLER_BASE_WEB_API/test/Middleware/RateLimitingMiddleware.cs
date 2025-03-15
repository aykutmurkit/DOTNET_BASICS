using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using test.Configuration;
using test.Core;

namespace test.Middleware
{
    /// <summary>
    /// Middleware for API rate limiting
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly RateLimitingConfiguration _options;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            IOptions<RateLimitingConfiguration> options)
        {
            _next = next;
            _cache = cache;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply rate limiting to API endpoints
            if (!context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Get client IP address
            var clientIp = GetClientIpAddress(context);
            var cacheKey = $"RateLimit_{clientIp}";

            // Check if client has existing request count
            if (!_cache.TryGetValue(cacheKey, out List<DateTime> requestTimestamps))
            {
                requestTimestamps = new List<DateTime>();
            }

            // Remove timestamps outside the time window
            var timeWindow = TimeSpan.FromMinutes(_options.TimeWindowMinutes);
            var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);
            requestTimestamps.RemoveAll(timestamp => timestamp < cutoffTime);

            // Check if client has exceeded the request limit
            if (requestTimestamps.Count >= _options.RequestLimit)
            {
                // Calculate time until next request is allowed
                var oldestTimestamp = requestTimestamps[0];
                var resetTime = oldestTimestamp.Add(timeWindow);
                var timeToReset = resetTime.Subtract(DateTime.UtcNow);

                // Set rate limit headers
                context.Response.Headers["X-RateLimit-Limit"] = _options.RequestLimit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = "0";
                context.Response.Headers["X-RateLimit-Reset"] = ((int)timeToReset.TotalSeconds).ToString();

                // Return rate limit exceeded response
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";

                var result = Result.Fail(
                    "API rate limit exceeded. Please try again later.",
                    new List<string> { $"Maximum of {_options.RequestLimit} requests per {_options.TimeWindowMinutes} minute(s) allowed." },
                    (int)HttpStatusCode.TooManyRequests);

                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            // Add current timestamp to the list
            requestTimestamps.Add(DateTime.UtcNow);

            // Set rate limit headers
            context.Response.Headers["X-RateLimit-Limit"] = _options.RequestLimit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = (_options.RequestLimit - requestTimestamps.Count).ToString();

            // Update cache
            _cache.Set(cacheKey, requestTimestamps, timeWindow);

            // Continue with the request
            await _next(context);
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Try to get IP from X-Forwarded-For header
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For may contain multiple IPs, take the first one
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                return ips[0].Trim();
            }

            // Fall back to remote IP address
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
} 