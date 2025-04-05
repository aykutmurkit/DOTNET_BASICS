# RateLimitLibrary

## Genel Bakış

RateLimitLibrary, ASP.NET Core uygulamaları için hız sınırlama işlevselliği sağlayan basit bir kütüphanedir. Bu kütüphane, API'leri fazla trafikten korumak ve kaynakları etkin bir şekilde yönetmek için global, IP tabanlı ve endpoint bazlı hız sınırlamalarını yapılandırmanıza olanak tanır.

## Özellikler

- Kolay entegrasyon
- Global hız sınırlama 
- IP tabanlı hız sınırlama
- Endpoint bazlı hız sınırlama
- Eşzamanlılık limitleri
- Özelleştirilebilir zaman pencereleri

## Hızlı Başlangıç

RateLimitLibrary'yi ASP.NET Core uygulamanızda kullanmak için:

1. Bu kütüphaneyi projenize referans olarak ekleyin
2. `RateLimitLibrarySettings.json` dosyasını projenize ekleyin veya uygulamanızın kendi ayarlarını kullanın
3. Program.cs dosyanıza aşağıdaki kodları ekleyin:

```csharp
using RateLimitLibrary.Extensions;

// Servis kayıtları
builder.Services.AddRateLimiting(builder.Configuration);

// Middleware yapılandırması
var app = builder.Build();

// ...

// Rate limiting middleware'ini ekleyin
app.UseRateLimiting();

// ...

// Endpoint'lere rate limit politikaları uygulama
app.MapControllers().RequireRateLimiting("ip");
```

## Yapılandırma

Kütüphane, varsayılan olarak `RateLimitLibrarySettings.json` dosyasından ayarları okur, ancak uygulamanızın `appsettings.json` dosyasındaki değerler önceliklidir.

Örnek yapılandırma:

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
      }
    ]
  }
}
```

## API Referansı

### Extensions

- `AddRateLimiting(IConfiguration)`: Rate limiting servislerini ekler
- `UseRateLimiting()`: Rate limiting middleware'ini yapılandırır

### RateLimitHelper

- `GetConfiguredPolicies(IConfiguration)`: Yapılandırılmış rate limit politikalarının bir listesini döndürür

## Lisans

Bu kütüphane MIT Lisansı altında lisanslanmıştır. 