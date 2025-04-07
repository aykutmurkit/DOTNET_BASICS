using AuthApi.API.Extensions;
using AuthApi.Business.Extensions;
using AuthApi.Core.Extensions;
using AuthApi.DataAccess.Extensions;
using Core.Utilities;
using LogLibrary.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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

// Swagger yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthApi API", Version = "v1" });
    
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

// HTTPS yönlendirmesini zorlamak için
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
});

// DataAccess Layer servisleri
builder.Services.AddDataAccessServices(builder.Configuration);

// Business Layer servisleri
builder.Services.AddBusinessServices();

// Core layer servisleri
builder.Services.AddCoreServices(builder.Configuration);

// LogLibrary servislerini ekle
builder.Services.AddLogLibrary(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Rate limiting servisleri
builder.Services.AddRateLimitingServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rate limiting middleware'ini ekle
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// İstek/yanıt loglama middleware'i kaldırıldı - sadece konsol loglama kullanılacak

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
