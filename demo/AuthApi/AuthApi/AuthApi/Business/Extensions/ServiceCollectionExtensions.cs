using AuthApi.Business.Services.Concrete;
using AuthApi.Business.Services.Interfaces;
using Core.Security;
using Core.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApi.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Auth ve User servisleri
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            
            // Security servisleri
            services.AddScoped<JwtHelper>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            
            // Utility servisleri
            services.AddScoped<EmailService>();
            
            return services;
        }
    }
} 