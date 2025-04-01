using AuthenticationApi.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AuthenticationApi.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
} 