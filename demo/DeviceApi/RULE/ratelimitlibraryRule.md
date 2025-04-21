# RateLimitLibrary Kullanım Kılavuzu

## Mimari Genel Bakış

RateLimitLibrary, ASP.NET Core uygulamaları için kapsamlı bir hız sınırlama (rate limiting) çözümüdür. Bu kütüphane, API'lerinizi aşırı trafikten koruyarak, sistem kaynaklarının adil dağıtımını sağlar ve olası DoS saldırılarına karşı koruma ekler. Kütüphane, farklı seviyelerde hız sınırlama stratejileri uygulayarak, uygulamanızın kararlılığını ve performansını korur.

### Temel Bileşenler

1. **Modeller Katmanı**
   - `RateLimitSettings`: Hız sınırlama yapılandırmalarını temsil eden ana sınıf
   - `IpRateLimitingSettings`: IP bazlı sınırlamalar için özel ayarlar
   - `EndpointRateLimit`: Endpoint bazlı limitleri tanımlayan sınıf

2. **Uzantılar Katmanı**
   - `RateLimitExtensions`: DI container'a hizmetleri kaydetmek ve middleware yapılandırması için uzantı metotları içerir:
     - `AddRateLimiting()`: Rate limiting servislerinin kaydedilmesi 
     - `UseRateLimiting()`: Middleware pipeline'ına rate limiting eklenmesi
     - `GetRateLimitPolicyMappings()`: Yapılandırılmış limitlerin listesini almak için yardımcı metot

3. **Middleware Katmanı**
   - `RateLimitMiddleware`: Custom rate limiting mantığı eklemek için middleware
   - ASP.NET Core'un yerleşik RateLimiter middleware'ini kullanır

4. **Yapılandırma**
   - `RateLimitLibrarySettings.json`: Varsayılan kütüphane ayarları
   - Uygulama `appsettings.json` dosyası ile geçersiz kılınabilir

## Hız Sınırlama Stratejileri

RateLimitLibrary, birden fazla hız sınırlama stratejisini bir arada kullanmanıza olanak tanır:

1. **Global Hız Sınırlama**
   - Tüm API için geçerli olan toplam istek limiti
   - Tüm kullanıcılar ve tüm endpointler için ortak limit
   - Sistem kaynaklarını korumak için kullanılır

2. **IP Bazlı Hız Sınırlama**
   - Her IP adresi için ayrı istek limiti
   - Tek bir kullanıcının veya istemcinin aşırı talep oluşturmasını engeller
   - DDoS saldırılarına karşı koruma sağlar

3. **Endpoint Bazlı Hız Sınırlama**
   - Belirli API endpoint'leri için özel limitler
   - Yüksek maliyetli işlemler için daha sıkı limitler uygulanabilir
   - URL yoluna göre limitleri özelleştirebilirsiniz

4. **Eşzamanlılık Limitleri**
   - Aynı anda işlenebilecek istek sayısını sınırlar
   - Kaynakları yoğun kullanan işlemler için idealdir
   - Sistem stabilitesini korur

## Çalışma Prensibi

RateLimitLibrary, aşağıdaki adımlarla çalışır:

1. **Yapılandırma Yükleme**: Kütüphane ilk olarak kendi varsayılan ayarlarını `RateLimitLibrarySettings.json` dosyasından yükler.
2. **Ayar Birleştirme**: Uygulama yapılandırmasındaki ayarlar (`appsettings.json`), varsayılan ayarları geçersiz kılabilir.
3. **Servis Kaydı**: Ayarlara göre uygun rate limiter'lar oluşturulur ve DI container'a kaydedilir.
4. **Middleware Entegrasyonu**: ASP.NET Core pipeline'ına rate limiter middleware'i eklenir.
5. **İstek İşleme**: Her gelen istek için:
   - İstek, global limitere tabi tutulur (etkinleştirilmişse)
   - İstek, IP limitlerine tabi tutulur (etkinleştirilmişse)
   - İstek, endpoint bazlı limitlere tabi tutulur (eşleşen endpoint varsa)
6. **Limit Aşımı**: Eğer bir istek limiti aşarsa, 429 Too Many Requests (Çok Fazla İstek) yanıtı döndürülür.

## Kullanılan Algoritma Türleri

1. **Fixed Window** (Sabit Pencere): Belirli bir zaman diliminde (örn. dakika, saat) izin verilen istek sayısını sınırlar.
2. **Concurrency** (Eşzamanlılık): Aynı anda işlenebilecek paralel istek sayısını sınırlar.

## RateLimitLibrary'nin Projenizde Kullanımı

### 1. Kurulum

Projenize RateLimitLibrary'yi eklemek için:

```xml
<!-- .csproj dosyanıza ekleyin -->
<ItemGroup>
  <PackageReference Include="RateLimitLibrary" Version="1.0.0" />
</ItemGroup>
```

### 2. Yapılandırma

`appsettings.json` dosyanıza rate limiting ayarlarını ekleyin:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 100
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/Auth/login",
        "Period": "5m",
        "Limit": 10,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 5
      },
      {
        "Endpoint": "/api/users",
        "Period": "1m",
        "Limit": 30,
        "EnableConcurrencyLimit": false
      }
    ]
  }
}
```

### 3. Servis Kaydı ve Middleware Yapılandırması

Program.cs dosyanızda rate limiting servislerini ve middleware'ini ekleyin:

```csharp
using RateLimitLibrary.Extensions;

// Web API builder
var builder = WebApplication.CreateBuilder(args);

// Controller'ları ekle
builder.Services.AddControllers();

// Rate limiting servislerini ekle
builder.Services.AddRateLimiting(builder.Configuration);

// Swagger ve diğer servisler...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uygulamayı oluştur
var app = builder.Build();

// Middleware pipeline yapılandırması
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rate limiting middleware'ini ekle
app.UseRateLimiting();

app.UseAuthorization();

// Controller'ları yapılandır ve rate limit politikalarını uygula
app.MapControllers().RequireRateLimiting("ip"); // IP bazlı limitleri tüm controller'lara uygula

app.Run();
```

### 4. Özel Endpoint Limitleri

Belirli controller'lar veya action'lar için endpoint bazlı limitleri etkinleştirme:

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("endpoint_/api/Auth/login")] // Endpoint bazlı limiti etkinleştir
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Login işlemleri...
        return Ok(new { token = "..." });
    }
}
```

## Zaman Penceresi Formatı

Rate limiting periyotları için aşağıdaki formatlar desteklenir:

- `10s` - 10 saniye
- `5m` - 5 dakika
- `2h` - 2 saat
- `1d` - 1 gün

## Özelleştirme Seçenekleri

### 1. Global Limitler

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "5m",
    "GlobalRateLimitRequests": 1000
  }
}
```

### 2. IP Bazlı Limitler

```json
{
  "RateLimitSettings": {
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 60
    }
  }
}
```

### 3. Endpoint Bazlı Limitler

```json
{
  "RateLimitSettings": {
    "EndpointLimits": [
      {
        "Endpoint": "/api/Orders/create",
        "Period": "1h",
        "Limit": 100,
        "EnableConcurrencyLimit": false
      }
    ]
  }
}
```

### 4. Eşzamanlılık Limitleri

```json
{
  "RateLimitSettings": {
    "EndpointLimits": [
      {
        "Endpoint": "/api/reports/generate",
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 3
      }
    ]
  }
}
```

## İleri Düzey Kullanım

### 1. Rate Limit Politikalarını Listeleme

```csharp
using RateLimitLibrary.Extensions;
using Microsoft.Extensions.Configuration;

public class RateLimitDiagnostics
{
    private readonly IConfiguration _configuration;
    
    public RateLimitDiagnostics(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void DisplayRateLimitPolicies()
    {
        var policies = RateLimitExtensions.GetRateLimitPolicyMappings(_configuration);
        
        Console.WriteLine("Yapılandırılmış Rate Limit Politikaları:");
        foreach (var policy in policies)
        {
            Console.WriteLine($"- {policy}");
        }
    }
}
```

### 2. Custom Rate Limit Middleware Uzantısı

RateLimitMiddleware sınıfını genişleterek özel loglama veya metrikler ekleyebilirsiniz:

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimitLibrary.Middleware;
using System.Threading.Tasks;

public class CustomRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomRateLimitMiddleware> _logger;

    public CustomRateLimitMiddleware(RequestDelegate next, ILogger<CustomRateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Rate limit uygulanmadan önce
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        _logger.LogInformation("Rate limit kontrolünden geçiriliyor: {IpAddress} - {Path}", 
            ipAddress, context.Request.Path);
        
        try
        {
            // Standart pipeline'a devam et
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limiting işlemi sırasında hata oluştu");
            throw;
        }
        
        // Rate limit uygulandıktan sonra
        if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
        {
            _logger.LogWarning("Rate limit aşıldı: {IpAddress} - {Path}", 
                ipAddress, context.Request.Path);
            
            // Metrik gönderme veya analitik işlemler burada yapılabilir
        }
    }
}

// Middleware'i kullanma
public static class CustomRateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomRateLimiting(this IApplicationBuilder app)
    {
        // Önce standart rate limiting middleware'ini ekle
        app.UseRateLimiter();
        
        // Sonra özel middleware'i ekle
        app.UseMiddleware<CustomRateLimitMiddleware>();
        
        return app;
    }
}
```

## Performans Değerlendirmeleri

1. **Bellek Kullanımı**
   - Rate limiting, aktif istek sayıları için bellek içi sayaçlar kullanır
   - IP bazlı rate limiting, aktif IP adreslerinin sayısına bağlı olarak bellek kullanımını artırabilir
   - Çok yüksek trafik durumlarında bellek kullanımını izleyin

2. **CPU Kullanımı**
   - Rate limiting, her istek için minimum CPU kullanır
   - Yüksek concurrency durumlarında bile düşük overhead sağlar

3. **Latency Etkisi**
   - Rate limiting middleware'i, istek pipeline'ına minimum gecikme ekler (genellikle <1ms)
   - Eşzamanlılık limitlemesi, request queue dolduğunda latency'i artırabilir

## Sorun Giderme

### Rate Limit Aşıldığında

429 Too Many Requests hataları, rate limit'in aşıldığını gösterir. Bu durumlarda:

1. **Response Headers'ı kontrol edin:**
   - `Retry-After`: Kaç saniye sonra tekrar denenmesi gerektiğini belirtir
   - `X-RateLimit-Limit`: Toplam izin verilen istek sayısı
   - `X-RateLimit-Remaining`: Kalan istek sayısı
   - `X-RateLimit-Reset`: Limitin ne zaman sıfırlanacağı

2. **Loglar:**
   - Server loglarını kontrol edin
   - Hangi rate limit türünün (global, IP, endpoint) aşıldığını belirleyin

### Yaygın Sorunlar ve Çözümleri

1. **Aşırı Kısıtlayıcı Limitler:**
   - Uygulamanızın gerçek trafik ihtiyaçlarına göre limitleri ayarlayın
   - Üretim öncesi ortamlarda test edin

2. **Proxy Arkasında IP Tanıma Sorunları:**
   - X-Forwarded-For header'larını kullanacak şekilde yapılandırın
   - Doğru IP adresi tanımlama için ağ yapılandırmanızı kontrol edin

3. **API Gateway İle Çakışmalar:**
   - API Gateway'in kendi rate limiting çözümü varsa, RateLimitLibrary ile birlikte kullanırken dikkatli olun
   - Duplicate limitleme yapılandırmalarından kaçının

## En İyi Uygulama Önerileri

1. **Kademeli Limitleme Stratejisi:**
   - Global limitleri yüksek tutun (tüm sistem için güvenlik ağı)
   - IP bazlı limitleri orta seviyede tutun (kötüye kullanım koruması)
   - Endpoint bazlı limitleri işlem maliyetine göre ayarlayın

2. **İstemci Farkındalığı:**
   - API yanıtlarında limit bilgilerini içeren header'lar ekleyin
   - İstemcilere retry-after bilgisini sağlayın

3. **Analitik ve İzleme:**
   - Rate limit aşımlarını izleyin ve analiz edin
   - Sürekli aşımlar, yetersiz kapasite veya potansiyel güvenlik tehditlerine işaret edebilir

4. **Limit Değerlerini Optimize Edin:**
   - Gerçek kullanım verilerine dayanarak limit değerlerini ayarlayın
   - Farklı müşteri segmentleri için farklı limitler düşünün

## Entegrasyon Örnekleri

### ASP.NET Core Web API

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiting();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Blazor Server

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiting();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

### Minimal API

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRateLimiting();

// API endpoints
app.MapGet("/api/users", () => Results.Ok(new[] { new { Id = 1, Name = "User 1" } }))
   .RequireRateLimiting("ip");

app.MapPost("/api/auth/login", () => Results.Ok(new { Token = "..." }))
   .RequireRateLimiting("endpoint_/api/Auth/login");

app.Run();
``` 