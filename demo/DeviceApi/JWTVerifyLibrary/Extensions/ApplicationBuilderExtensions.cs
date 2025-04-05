using JWTVerifyLibrary.Middleware;
using Microsoft.AspNetCore.Builder;

namespace JWTVerifyLibrary.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseJwtVerification(this IApplicationBuilder app)
        {
            // Add JWT middleware to the pipeline
            app.UseMiddleware<JwtMiddleware>();
            
            return app;
        }
    }
} 