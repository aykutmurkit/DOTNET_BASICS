using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using DeviceApi.Business.Services.Concrete;
using DeviceApi.Business.Services.Interfaces;

namespace DeviceApi.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB için client
            services.AddSingleton<IMongoClient>(_ => 
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });
            
            // JWT Authentication ekle
            AddJwtAuthentication(services, configuration);
            
            // HttpContextAccessor ekle
            services.AddHttpContextAccessor();
            
            // Loglama servisleri
            services.AddCoreLoggingServices();
            
            return services;
        }
        
        private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
        
        public static IServiceCollection AddCoreLoggingServices(this IServiceCollection services)
        {
            // Loglama servisleri
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            services.AddScoped<IApiLogService, ApiLogService>();
            
            return services;
        }
    }
} 