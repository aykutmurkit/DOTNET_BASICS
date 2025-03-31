using Business.Interfaces;
using Business.Services;
using Core.Extensions;
using Core.Security;
using Core.Utilities;
using Data.Context;
using Data.Extensions;
using Data.Interfaces;
using Data.Repositories;
using Data.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using deneme.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı servislerini ekle
builder.Services.AddDatabaseServices(builder.Configuration);

// JWT doğrulama yapılandırması
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Rate limiting servislerini ekle
builder.Services.AddRateLimitingServices(builder.Configuration);

// Servisleri kaydet
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

// Add MongoDB client
builder.Services.AddSingleton<IMongoClient>(_ => 
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

// Add logging services
builder.Services.AddLoggingServices();

// Controller'lara validasyon filtresi ve API yanıt formatını özelleştirme
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Deneme API", Version = "v1" });
    
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

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Veritabanını başlangıçta yapılandır
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Veritabanı sıfırlanıyor...");
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        logger.LogInformation("Veritabanı başlangıç verileri ekleniyor...");
        await seeder.SeedAsync();
        logger.LogInformation("Veritabanı başlangıç verileri başarıyla eklendi.");
    }
}

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

// Endpoint spesifik rate limit'leri uygula
app.MapControllers().RequireRateLimiting("ip");

// Use request/response logging middleware
app.UseRequestResponseLogging();

// Uygulamayı çalıştır
if (app.Environment.IsDevelopment())
{
    // Geliştirme ortamında rate limit bilgilerini logla
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var endpointPolicyMappings = RateLimitExtensions.GetEndpointPolicyMappings(builder.Configuration);
        
        logger.LogInformation("Rate limit politikaları yapılandırıldı:");
        foreach (var mapping in endpointPolicyMappings)
        {
            logger.LogInformation(mapping);
        }
    }
}

app.Run();
