# RateLimitLibrary Dokümantasyonu

RateLimitLibrary, ASP.NET Core uygulamaları için kolay kullanımlı bir rate limiting (hız sınırlama) kütüphanesidir. 
Bu kütüphane, API'lerinizi aşırı yükleme saldırılarından korur ve kaynakları verimli şekilde kullanmanızı sağlar.

## Dokümantasyon İçeriği

### Türkçe (TUR)
1. [Giriş](./TUR/01-Giris.md)
2. [Kurulum](./TUR/02-Kurulum.md)
3. [Hızlı Başlangıç](./TUR/03-Hizli-Baslangic.md)
4. [Yapılandırma](./TUR/04-Yapilandirma.md)
5. [API Referansı](./TUR/05-API-Referansi.md)
6. [Mimari Yapı](./TUR/06-Mimari-Yapi.md)
7. [İyi Uygulama Örnekleri](./TUR/07-Iyi-Uygulama-Ornekleri.md)

### İngilizce / English (ENG)
1. [Introduction](./ENG/01-Introduction.md)
2. [Installation](./ENG/02-Installation.md)
3. [Quick Start](./ENG/03-Quick-Start.md)
4. [Configuration](./ENG/04-Configuration.md)
5. [API Reference](./ENG/05-API-Reference.md)
6. [Architecture](./ENG/06-Architecture.md)
7. [Best Practices](./ENG/07-Best-Practices.md)

## Hızlı Başlangıç / Quick Start

```csharp
// Program.cs
using RateLimitLibrary.Extensions;

// Servisleri kaydet
builder.Services.AddRateLimiting(builder.Configuration);

// Middleware yapılandırma
var app = builder.Build();

// ...

// Rate limiting middleware'ini ekle
app.UseRateLimiting();

// ...

// Endpoint'lere rate limit politikalarını uygula
app.MapControllers().RequireRateLimiting("ip");
```

## Temel Özellikler / Key Features

- Global rate limiting (küresel hız sınırlama)
- IP tabanlı rate limiting
- Endpoint bazlı rate limiting
- Kolay yapılandırma
- .NET Core uygulamalarıyla sorunsuz entegrasyon
- Özelleştirilebilir zaman pencereleri ve limit değerleri 