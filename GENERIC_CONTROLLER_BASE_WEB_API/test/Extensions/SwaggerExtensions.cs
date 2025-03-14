using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using test.Configuration;

namespace test.Extensions
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds Swagger services to the service collection
        /// </summary>
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Device Management API", 
                    Version = "v1",
                    Description = "API for managing devices and their APN settings"
                });

                // Enable annotations for Swagger
                c.EnableAnnotations();

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Custom order for HTTP methods
                c.OrderActionsBy(apiDesc =>
                {
                    var method = apiDesc.HttpMethod;
                    var order = method switch
                    {
                        "GET" => "1",
                        "POST" => "2",
                        "PUT" => "3",
                        "PATCH" => "4",
                        "DELETE" => "5",
                        _ => "6"
                    };
                    return $"{order}_{apiDesc.RelativePath}";
                });

                // Hide schemas
                c.UseAllOfToExtendReferenceSchemas();
            });

            return services;
        }

        /// <summary>
        /// Configures the Swagger middleware
        /// </summary>
        public static IApplicationBuilder UseSwaggerServices(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Device Management API V1");
                c.RoutePrefix = "swagger";
                
                // Customize the UI
                c.DefaultModelsExpandDepth(-1);
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.List);
                c.EnableDeepLinking();
                c.DisplayOperationId();

                // Apply custom CSS and JavaScript from SwaggerConfiguration
                c.HeadContent = SwaggerConfiguration.GetSwaggerUICustomization();
            });

            return app;
        }
    }
} 