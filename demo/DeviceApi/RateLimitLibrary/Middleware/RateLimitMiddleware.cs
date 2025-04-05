using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RateLimitLibrary.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Rate limiter middlewares are already in the pipeline from the extensions
            // This is a hook for any custom rate limit logic we might want to add
            
            _logger.LogDebug("Request being processed by rate limit middleware: {Path}", context.Request.Path);
            
            // Continue with the pipeline
            await _next(context);
        }
    }
} 