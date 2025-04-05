using JWTVerifyLibrary.Services;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWTVerifyLibrary.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var token = GetTokenFromRequest(context);

            if (!string.IsNullOrEmpty(token) && jwtService.ValidateToken(token, out JwtSecurityToken? validatedToken))
            {
                // Add claims to context
                if (validatedToken != null)
                {
                    var claims = validatedToken.Claims.ToList();
                    var identity = new ClaimsIdentity(claims, "jwt");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            await _next(context);
        }

        private string? GetTokenFromRequest(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }
} 