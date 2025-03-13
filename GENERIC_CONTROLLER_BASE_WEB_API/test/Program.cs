using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Repositories;
using test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// Register Services
builder.Services.AddScoped<IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>, DeviceService>();
builder.Services.AddScoped<IService<ApnName, ApnNameDto, CreateApnNameDto, UpdateApnNameDto>, SettingService>();
builder.Services.AddScoped<IService<ApnPassword, ApnPasswordDto, CreateApnPasswordDto, UpdateApnPasswordDto>, ApnPasswordService>();
builder.Services.AddScoped<IService<ApnAddress, ApnAddressDto, CreateApnAddressDto, UpdateApnAddressDto>, ApnAddressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Device Management API V1");
        c.RoutePrefix = "swagger";
        
        // Customize the UI
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.EnableDeepLinking();
        c.DisplayOperationId();

        // Custom styling to show only API title and hide other information
        c.HeadContent = @"
            <style>
                .swagger-ui .topbar { display: none; }
                .swagger-ui .information-container { 
                    padding: 10px !important; 
                    margin-top: 20px !important;
                    display: flex !important;
                    justify-content: space-between !important;
                    align-items: center !important;
                }
                .swagger-ui .information-container .info { margin: 0 !important; }
                .swagger-ui .information-container .info .title { margin: 0 !important; }
                .swagger-ui .information-container .info > div:not(.title) { display: none !important; }
                .swagger-ui .information-container .info .link { display: none !important; }
                .swagger-ui .information-container .info .version { display: none !important; }
                .swagger-ui .information-container .info .title span { display: none !important; }
                .swagger-ui .scheme-container { display: none; }
                .swagger-ui .servers-container { display: none; }
                .custom-button {
                    background-color: #4CAF50 !important;
                    border: none !important;
                    color: white !important;
                    padding: 10px 20px !important;
                    text-align: center !important;
                    text-decoration: none !important;
                    display: inline-block !important;
                    font-size: 16px !important;
                    margin: 4px 2px !important;
                    cursor: pointer !important;
                    border-radius: 4px !important;
                    transition: background-color 0.3s !important;
                }
                .custom-button:hover {
                    background-color: #45a049 !important;
                }
            </style>
            <script>
                function addButton() {
                    try {
                        const container = document.querySelector('.information-container');
                        if (!container) {
                            setTimeout(addButton, 100);
                            return;
                        }
                        
                        if (!document.querySelector('.custom-button')) {
                            const button = document.createElement('button');
                            button.className = 'custom-button';
                            button.textContent = 'View JSON';
                            button.onclick = function() {
                                window.location.href = '/swagger/v1/swagger.json';
                            };
                            container.appendChild(button);
                        }
                    } catch (error) {
                        console.error('Error adding button:', error);
                    }
                }

                // Try to add button when DOM is loaded
                if (document.readyState === 'loading') {
                    document.addEventListener('DOMContentLoaded', addButton);
                } else {
                    addButton();
                }

                // Also try after window load
                window.addEventListener('load', addButton);
            </script>";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted(); // Drop existing database
    context.Database.EnsureCreated(); // Create new database with seed data
}

app.Run();
