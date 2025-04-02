using DeviceApi.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DeviceApi.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
} 