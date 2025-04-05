# Hızlı Başlangıç

**Sürüm:** 1.0.0  
**Şirket:** DevOps 2025

---

Bu kılavuz, RateLimitLibrary'yi hızlı bir şekilde ASP.NET Core uygulamanıza entegre etmenize yardımcı olacaktır.

## Ön Koşullar

- .NET 8.0+ ASP.NET Core projesi
- RateLimitLibrary yüklü (bkz. [Kurulum](02-Kurulum.md))

## 5 Dakikada Entegrasyon

Aşağıdaki adımları izleyerek RateLimitLibrary'yi hızlıca projenize entegre edebilirsiniz:

### 1. Yapılandırma Dosyasını Oluşturun

Projenizin kök dizininde `RateLimitLibrarySettings.json` dosyası oluşturun:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 30
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "5m",
        "Limit": 5
      }
    ]
  }
}
```

Proje dosyanıza (.csproj) şu ayarı ekleyin:

```xml
<ItemGroup>
  <None Update="RateLimitLibrarySettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### 2. Program.cs Dosyasını Düzenleyin

RateLimitLibrary servislerini ve middleware'ini Program.cs dosyanıza ekleyin:

```csharp
using RateLimitLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Diğer servisler...

// Rate limiting servislerini ekleyin
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

// HTTP request pipeline yapılandırması
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rate limiting middleware'ini ekleyin (UseAuthentication'dan önce olmalıdır)
app.UseRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint'lere rate limit politikalarını uygulayın
app.MapControllers().RequireRateLimiting("ip");

app.Run();
```

### 3. Uygulamayı Çalıştırın ve Test Edin

Yapılandırmanın doğru çalışıp çalışmadığını test etmek için:

1. Uygulamayı başlatın:
```bash
dotnet run
```

2. Hızlı bir şekilde art arda istekler yapın (örneğin Postman, curl veya bir web tarayıcısı kullanarak)

3. Yapılandırdığınız limitlere ulaştığınızda 429 (Too Many Requests) yanıtı almalısınız.

### Curl ile Test Örneği

```bash
# Hızlıca 10 istek gönderin
for i in {1..10}; do
  curl -i http://localhost:5000/api/device
  sleep 0.2
done
```

## Spesifik Endpoint'lere Limitler Ekleme

Belirli endpoint'ler için özel sınırlar eklemek:

1. `RateLimitLibrarySettings.json` dosyasındaki `EndpointLimits` dizisine yeni girdiler ekleyin:

```json
"EndpointLimits": [
  {
    "Endpoint": "/api/auth/login",
    "Period": "5m",
    "Limit": 5
  },
  {
    "Endpoint": "/api/device",
    "Period": "10m",
    "Limit": 20
  },
  {
    "Endpoint": "/api/reports/generate",
    "Period": "1h",
    "Limit": 5,
    "EnableConcurrencyLimit": true,
    "ConcurrencyLimit": 2
  }
]
```

2. Uygulamanızı yeniden başlatın

## Endpoint'lere Rate Limit Politikaları Uygulama

Belirli controller eylemlerine veya endpoint'lere rate limit politikaları uygulamak için birkaç yol vardır:

### Tüm API'ye Uygulama

```csharp
app.MapControllers().RequireRateLimiting("ip");
```

### Belirli Endpoint'lere Uygulama (endpoint kullanarak)

```csharp
app.MapGet("/api/values", () => new[] { "value1", "value2" })
   .RequireRateLimiting("endpoint__api_values");
```

### Controller Seviyesinde Uygulama (özellik kullanarak)

```csharp
[EnableRateLimiting("ip")]
public class DeviceController : ControllerBase
{
    // Controller eylemleri...
}
```

### Belirli Bir Eylem İçin Uygulama

```csharp
public class AuthController : ControllerBase
{
    [EnableRateLimiting("endpoint__api_auth_login")]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // Login işlemi...
    }
}
```

## Dinamik Rate Limiting

Kodunuzda dinamik olarak rate limit ayarlarını değiştirebilirsiniz:

```csharp
builder.Services.AddRateLimiter(options =>
{
    // IP tabanlı rate limit'i doğrudan yapılandırma
    options.AddPolicy("dynamic_ip", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var isPremiumUser = CheckIfPremiumUser(ipAddress); // Kendi metodunuz
        
        // Premium kullanıcılar için daha yüksek limit
        var limit = isPremiumUser ? 200 : 50;
        var window = isPremiumUser ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(5);
        
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limit,
            Window = window,
            QueueLimit = 0
        });
    });
});
```

## Sınırı Aşıldığında Özel Davranış

RateLimitLibrary, varsayılan olarak 429 Too Many Requests yanıtı döndürür. Bu davranışı özelleştirebilirsiniz:

```csharp
builder.Services.AddRateLimiter(options => 
{
    // Varsayılan reddetme durum kodunu değiştirin
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    // Özel bir yanıt şablonu belirleyin
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        
        var response = new 
        {
            error = "Rate limit aşıldı",
            retryAfter = 60, // saniye cinsinden
            message = "Lütfen daha sonra tekrar deneyin"
        };
        
        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
    
    // Diğer rate limit yapılandırmaları...
});
```

## Hata Ayıklama İpuçları

- 429 yanıtı almıyorsanız, `UseRateLimiting()` middleware'inin doğru sırada çağrıldığından emin olun
- Yapılandırma değişikliklerinin etkili olması için uygulamayı yeniden başlatın
- Endpoint isimlerinde tam yolu kullandığınızdan emin olun, örneğin: "/api/auth/login"

---

[◀ Kurulum](02-Kurulum.md) | [Ana Sayfa](../README.md) | [İleri: Yapılandırma ▶](04-Yapilandirma.md) 