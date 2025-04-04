using LogLib.Configuration.Settings;
using LogLib.Core.Interfaces;
using LogLib.Data.Context;
using LogLib.Data.Repositories;
using LogLib.Services.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace LogLib.Extensions
{
    /// <summary>
    /// LogLib servislerinin DI container'a kaydedilmesi için extension metotları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// LogLib servislerini DI container'a kaydeder
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddLogLib(this IServiceCollection services, IConfiguration configuration = null)
        {
            var configBuilder = new ConfigurationBuilder();
            
            // LogLib assembly'sindeki settings.json dosyasını oku
            var assembly = Assembly.GetExecutingAssembly();
            var settingsFile = "settings.json";
            
            // Embedded resource olarak paketlenmiş settings.json'ı ara
            var resourcePath = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(settingsFile, StringComparison.OrdinalIgnoreCase));
            
            // Eğer embedded resource olarak yoksa, doğrudan dosya olarak ara
            if (string.IsNullOrEmpty(resourcePath))
            {
                var assemblyLocation = assembly.Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                // Doğrudan exe/dll yanındaki settings.json
                var directSettingsPath = Path.Combine(assemblyDirectory, settingsFile);
                if (File.Exists(directSettingsPath))
                {
                    configBuilder.AddJsonFile(directSettingsPath, optional: true, reloadOnChange: true);
                }
                
                // Configuration/Settings klasöründeki settings.json
                var nestedSettingsPath = Path.Combine(assemblyDirectory, "Configuration", "Settings", settingsFile);
                if (File.Exists(nestedSettingsPath))
                {
                    configBuilder.AddJsonFile(nestedSettingsPath, optional: true, reloadOnChange: true);
                }
            }
            else
            {
                // Embedded resource olarak bulunduysa, onu kullan
                using var stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream != null)
                {
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    
                    configBuilder.AddJsonStream(memoryStream);
                }
            }
            
            // Eğer dışarıdan bir konfigürasyon geçilmişse (ana appsettings.json), o da eklenebilir
            if (configuration != null)
            {
                configBuilder.AddConfiguration(configuration);
            }
            
            var logLibConfig = configBuilder.Build();
            
            // Yapılandırma bulunamazsa varsayılan ayarları kullan
            var logSettings = new LogSettings();
            logLibConfig.GetSection("LogSettings").Bind(logSettings);
            
            // Varsayılan değerler ekle - section binding ile yapalım
            if (string.IsNullOrEmpty(logSettings.ConnectionString))
                logSettings.ConnectionString = "mongodb://localhost:27017";
            if (string.IsNullOrEmpty(logSettings.DatabaseName))
                logSettings.DatabaseName = "AuthApiLogs";
            if (string.IsNullOrEmpty(logSettings.CollectionName))
                logSettings.CollectionName = "Logs";
            
            // Güncellenmiş ayarları DI container'a kaydet
            services.Configure<LogSettings>(options => 
            {
                options.ConnectionString = logSettings.ConnectionString;
                options.DatabaseName = logSettings.DatabaseName;
                options.CollectionName = logSettings.CollectionName;
                options.ApplicationName = logSettings.ApplicationName;
                options.Environment = logSettings.Environment;
                options.RetentionDays = logSettings.RetentionDays;
                options.MaskSensitiveData = logSettings.MaskSensitiveData;
                options.EnableAsyncLogging = logSettings.EnableAsyncLogging;
            });
            
            // HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // MongoDB context
            services.AddSingleton<MongoDbContext>();
            
            // Repository
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            
            // LogService
            services.AddScoped<ILogService, LogService>();
            
            return services;
        }
    }
} 