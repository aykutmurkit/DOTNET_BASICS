using AuthenticationApi.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;

namespace AuthenticationApi.Core.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds the logging services to the service collection
        /// </summary>
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            // Log servisi
            services.AddSingleton<ILogService, LogService>();
            
            // HTTP context accessor for accessing request context in service layer
            services.AddHttpContextAccessor();
            
            return services;
        }
        
        /// <summary>
        /// Configures Serilog with Graylog in the host builder
        /// </summary>
        public static IHostBuilder UseSerilogWithGraylog(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, services, configuration) => {
                var graylogSettings = context.Configuration.GetSection("GraylogSettings");
                var isEnabled = graylogSettings.GetValue<bool>("Enabled");
                var host = graylogSettings.GetValue<string>("Host");
                var port = graylogSettings.GetValue<int>("Port");
                var useUdp = graylogSettings.GetValue<bool>("UseUdp");
                var logSource = graylogSettings.GetValue<string>("LogSource");
                
                configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .Enrich.WithEnvironmentName();
                
                // Console log sink
                configuration.WriteTo.Console();
                
                // Graylog sink
                if (isEnabled && !string.IsNullOrEmpty(host))
                {
                    var options = new GraylogSinkOptions
                    {
                        HostnameOrAddress = host,
                        Port = port,
                        TransportType = useUdp 
                            ? Serilog.Sinks.Graylog.Core.Transport.TransportType.Udp 
                            : Serilog.Sinks.Graylog.Core.Transport.TransportType.Tcp,
                        Facility = logSource,
                        MinimumLogEventLevel = LogEventLevel.Information,
                        IncludeMessageTemplate = true
                    };
                    
                    configuration.WriteTo.Graylog(options);
                }
            });
        }
    }
} 