using System.Security.Claims;

namespace DeviceApi.API.Middleware
{
    public class JwtClaimsLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtClaimsLoggingMiddleware> _logger;

        public JwtClaimsLoggingMiddleware(RequestDelegate next, ILogger<JwtClaimsLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = context.User.FindFirst(ClaimTypes.Name)?.Value;
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                _logger.LogInformation(
                    "Authenticated request: Path: {Path}, Method: {Method}, UserId: {UserId}, Username: {Username}, Email: {Email}, Role: {Role}",
                    context.Request.Path,
                    context.Request.Method,
                    userId,
                    username,
                    email,
                    role);
            }
            
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JwtClaimsLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtClaimsLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtClaimsLoggingMiddleware>();
        }
    }
} 