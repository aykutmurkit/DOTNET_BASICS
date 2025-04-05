using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DeviceApi.Business.Services.Concrete;
using DeviceApi.Business.Services.Interfaces;

namespace DeviceApi.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // HttpContextAccessor ekle
            services.AddHttpContextAccessor();
            
            return services;
        }
    }
} 