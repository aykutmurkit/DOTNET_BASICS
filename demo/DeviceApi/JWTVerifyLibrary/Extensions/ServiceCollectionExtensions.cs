using JWTVerifyLibrary.Models;
using JWTVerifyLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTVerifyLibrary.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtVerification(this IServiceCollection services, IConfiguration configuration)
        {
            // Load JWT settings from the library's settings file
            var libraryConfig = configuration.LoadJwtVerifyLibrarySettings();
            
            // Add JwtSettings from configuration
            services.Configure<JwtSettings>(libraryConfig.GetSection("JwtSettings"));

            // Register JwtService
            services.AddScoped<IJwtService, JwtService>();

            // Configure JWT authentication
            var jwtSettings = libraryConfig.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings != null)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Custom token extraction logic if needed
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // Handle authentication failure
                            return Task.CompletedTask;
                        }
                    };
                });
            }

            return services;
        }
    }
} 