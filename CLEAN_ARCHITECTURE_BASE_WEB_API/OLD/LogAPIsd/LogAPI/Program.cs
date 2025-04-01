using LogAPI.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Web uygulaması builder'ını oluştur
var builder = WebApplication.CreateBuilder(args);

// HTTPS bağlantısını zorunlu kıl
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

// JSON formatında kontrolcüleri ekle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // JSON serilizasyon ayarları
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Swagger dokümantasyonu için gerekli servisleri ekle
builder.Services.AddSwaggerWithJwtAuth();
builder.Services.AddEndpointsApiExplorer();

// Uygulama için gerekli çekirdek servisleri ekle (MongoDB bağlantısı vb.)
builder.Services.AddCoreServices(builder.Configuration);

// JWT kimlik doğrulama yapılandırması
builder.Services.AddJwtAuthentication(builder.Configuration);

// Yetkilendirme politikalarını yapılandır
builder.Services.AddAuthorization(options =>
{
    // Admin rolü için özel politika
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// CORS politikasını appsettings.json'dan yapılandır
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Tüm kaynaklara izin verip vermeme ayarını kontrol et
        bool allowAll = builder.Configuration.GetValue<bool>("CorsSettings:AllowAll");
        
        if (allowAll)
        {
            // Tüm kaynaklara izin ver
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            
            Console.WriteLine("CORS: Tüm kaynaklara izin veriliyor (geçici ayar, production'da kullanılmamalı)");
        }
        else
        {
            // Sadece belirtilen kaynaklara izin ver
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                      
                Console.WriteLine($"CORS: {allowedOrigins.Length} kaynağa izin veriliyor");
            }
            else
            {
                // Eğer hiçbir kaynak belirtilmemişse, varsayılan olarak sadece aynı kaynağa izin ver
                Console.WriteLine("CORS: Hiçbir kaynak belirtilmemiş, sadece aynı kaynağa izin veriliyor");
            }
        }
    });
});

// Web uygulaması oluştur
var app = builder.Build();

// Swagger UI'ı yapılandır
app.UseSwaggerWithUI();

// HTTP isteklerini HTTPS'e yönlendir
app.UseHttpsRedirection();

// CORS middleware'ini etkinleştir
app.UseCors();

// Kimlik doğrulama ve yetkilendirme middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

// API kontrolcülerini rotalara bağla
app.MapControllers();

// Uygulamayı çalıştır
app.Run();
