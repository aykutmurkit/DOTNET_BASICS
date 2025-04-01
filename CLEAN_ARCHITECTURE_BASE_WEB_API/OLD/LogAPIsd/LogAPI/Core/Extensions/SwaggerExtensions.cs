using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace LogAPI.Core.Extensions
{
    /// <summary>
    /// Swagger API dokümantasyonu için extension metotlarını içerir
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Swagger servislerini JWT kimlik doğrulama desteği ile birlikte ekler
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <returns>Güncellenmiş servis koleksiyonu</returns>
        public static IServiceCollection AddSwaggerWithJwtAuth(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            
            var swaggerSettings = configuration.GetSection("SwaggerSettings");
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    swaggerSettings["Version"] ?? "v1", 
                    new OpenApiInfo
                    {
                        Title = swaggerSettings["ApiName"] ?? "LogAPI",
                        Version = swaggerSettings["Version"] ?? "v1",
                        Description = "MongoDB'ye kaydedilen logları yönetmek için API",
                        Contact = new OpenApiContact
                        {
                            Name = "Deneme API Ekibi",
                            Email = "info@denemeapi.com"
                        }
                    });
                
                // XML belgeleri Swagger UI'a ekle
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                
                // JWT için gerekli tanımlamalar
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header kullanımı: \"Authorization: Bearer {token}\""
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            
            return services;
        }
        
        /// <summary>
        /// Swagger middleware'ini ve UI'ı yapılandırır
        /// </summary>
        /// <param name="app">Uygulama builder</param>
        /// <returns>Güncellenmiş uygulama builder</returns>
        public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var swaggerSettings = configuration.GetSection("SwaggerSettings");
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    $"/swagger/{swaggerSettings["Version"] ?? "v1"}/swagger.json", 
                    swaggerSettings["ApiName"] ?? "LogAPI");
                
                // RoutePrefix ayarını kullan (boş string anasayfada Swagger'ı gösterir)
                c.RoutePrefix = swaggerSettings["RoutePrefix"] ?? string.Empty;
                
                // Swagger UI başlığını ayarla
                c.DocumentTitle = swaggerSettings["DocumentTitle"] ?? "Log API Dokümantasyonu";
            });
            
            return app;
        }
    }
} 