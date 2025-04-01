using System;
using System.Text;
using LogAPI.Business.Services.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace LogAPI.Core.Extensions
{
    /// <summary>
    /// Servis koleksiyonu için extension metotlarını içerir
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Uygulamanın çekirdek servislerini (MongoDB, Log Repository, vb.) ekler
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <param name="configuration">Uygulama yapılandırması</param>
        /// <returns>Güncellenmiş servis koleksiyonu</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB bağlantısını yapılandır
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("MongoDB bağlantı dizesi (ConnectionStrings:MongoDb) yapılandırma dosyasında bulunamadı.");
                }
                return new MongoClient(connectionString);
            });
            
            // Log repository'sini ekle
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            
            return services;
        }
        
        /// <summary>
        /// JWT tabanlı kimlik doğrulama servislerini ekler
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <param name="configuration">Uygulama yapılandırması</param>
        /// <returns>Güncellenmiş servis koleksiyonu</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];
            
            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT Secret (JwtSettings:Secret) yapılandırma dosyasında bulunamadı.");
            }
            
            var issuer = jwtSettings["Issuer"] ?? "DenemeApi";
            var audience = jwtSettings["Audience"] ?? "DenemeApiClient";
            
            Console.WriteLine($"JWT Yapılandırması: Secret={secret}, Issuer={issuer}, Audience={audience}");
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // HTTPS zorunluluğunu kaldır (development için)
                options.SaveToken = true; // Token'ı HttpContext'te sakla
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.Zero // Token süresinin tam olarak uygulanması için
                };
                
                // Token doğrulama sürecinde detaylı loglama
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var token = context.SecurityToken;
                        Console.WriteLine($"Token başarıyla doğrulandı: {token.Id}, Süre: {token.ValidTo}");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Token doğrulama hatası: {context.Exception.GetType().Name} - {context.Exception.Message}");
                        
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                            Console.WriteLine("Token süresi dolmuş!");
                        }
                        else if (context.Exception is SecurityTokenInvalidSignatureException)
                        {
                            Console.WriteLine("Token imzası geçersiz! Secret key uyumsuzluğu olabilir.");
                        }
                        else if (context.Exception is SecurityTokenInvalidIssuerException)
                        {
                            Console.WriteLine("Token issuer'ı geçersiz!");
                        }
                        else if (context.Exception is SecurityTokenInvalidAudienceException)
                        {
                            Console.WriteLine("Token audience'ı geçersiz!");
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        Console.WriteLine($"Alınan Authorization Header: {authHeader}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Challenge tetiklendi: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });
            
            return services;
        }
    }
} 