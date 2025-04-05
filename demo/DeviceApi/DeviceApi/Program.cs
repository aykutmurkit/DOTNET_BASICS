using DeviceApi.API.Extensions;
using DeviceApi.API.Middleware;
using DeviceApi.Business.Extensions;
using DeviceApi.Core.Extensions;
using DeviceApi.DataAccess.Extensions;
using Core.Utilities;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using JWTVerifyLibrary.Extensions;
using RateLimitLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure logging for console only
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Controllers ve API davranış ayarları
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Özel model validasyon hatası yanıtı
    options.InvalidModelStateResponseFactory = context =>
    {
        // Hata sözlüğü oluşturma
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

        // Standart hata yanıtı oluşturma
        var response = ApiResponse<object>.Error(
            errors, 
            "Lütfen form alanlarını kontrol ediniz", 
            StatusCodes.Status400BadRequest
        );

        return new BadRequestObjectResult(response);
    };
})
.AddJsonOptions(options =>
{
    // JSON yanıtlarda null değerli özellikleri gizle
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    // Enum değerlerini string olarak dön
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// JWT doğrulama servisi ekle - bu JWT doğrulamasını JWTVerifyLibrary kullanarak yapacak
builder.Services.AddJwtVerification(builder.Configuration);

// Swagger yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DeviceApi API", Version = "v1" });
    
    // JWT doğrulaması için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header kullanarak kimlik doğrulama. Örnek: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// DataAccess Layer servisleri
builder.Services.AddDataAccessServices(builder.Configuration);

// Business Layer servisleri
builder.Services.AddBusinessServices();

// Core layer servisleri
builder.Services.AddCoreServices(builder.Configuration);

// Rate limiting servisleri - RateLimitLibrary'den gelen extension metotları kullanarak
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rate limiting middleware'ini ekle - RateLimitLibrary'den
app.UseRateLimiting();

// JWT doğrulama middleware'ini kullan
app.UseJwtVerification();

app.UseAuthentication();

app.UseAuthorization();

// Endpoint spesifik rate limit'leri uygula
app.MapControllers().RequireRateLimiting("ip");

// Veritabanını sıfırlama ve seed etme
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var context = serviceProvider.GetRequiredService<Data.Context.AppDbContext>();
            var seeder = serviceProvider.GetRequiredService<Data.Seeding.DatabaseSeeder>();
            
            logger.LogInformation("Veritabanı sıfırlanıyor...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            logger.LogInformation("Veritabanı başlangıç verileri ekleniyor...");
            await seeder.SeedAsync();
            logger.LogInformation("Veritabanı başlangıç verileri başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Veritabanı başlatma sırasında hata oluştu");
            throw;
        }
    }
}

app.Run();
