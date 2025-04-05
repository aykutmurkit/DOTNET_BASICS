# Mimari Yapı

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölüm, JWTVerifyLibrary'nin iç mimarisini, bileşenleri ve çalışma prensiplerini açıklar.

## Genel Mimari

JWTVerifyLibrary, modüler bir yapıda tasarlanmış olup, aşağıdaki temel bileşenlerden oluşur:

![JWTVerifyLibrary Mimari](../images/architecture.png)

1. **Yapılandırma Katmanı**: JWT ayarlarını yönetir
2. **Servis Katmanı**: Temel JWT doğrulama işlemlerini gerçekleştirir
3. **Middleware Katmanı**: HTTP isteklerini yakalar ve JWT token'larını doğrular
4. **Uzantı Katmanı**: Kolay entegrasyon için extension metodları sağlar

## Bileşenler ve Sorumlulukları

### 1. Models (Modeller)

Bu klasör, kütüphanenin veri modelleri için tanımları içerir.

**JwtSettings.cs**

JWT yapılandırma ayarlarının tutulduğu model sınıfı:

```csharp
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
}
```

### 2. Services (Servisler)

JWT işlemleri için temel hizmetleri sağlar.

**JwtService.cs**

JWT token doğrulama ve işleme mantığını içerir:

```csharp
public interface IJwtService
{
    bool ValidateToken(string token, out JwtSecurityToken? validatedToken);
    IEnumerable<Claim> GetTokenClaims(string token);
}

public class JwtService : IJwtService
{
    // Token doğrulama metodları burada
}
```

Bu servis:
- Token'ın imzasını kontrol eder
- Süresi dolmuş token'ları tespit eder
- Geçerli bir token'dan claims bilgilerini çıkartır

### 3. Middleware (Ara Yazılım)

HTTP isteklerini yakalayıp JWT doğrulamasını gerçekleştirir.

**JwtMiddleware.cs**

```csharp
public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        // HTTP isteklerinden JWT token'ını çıkar
        // Token'ı doğrula
        // Doğrulama başarılıysa kullanıcı kimliğini oluştur
        // Sonraki middleware'e geç
    }
}
```

Bu middleware:
- HTTP isteğinden Bearer token'ı çıkarır
- Token'ı doğrular
- Geçerli bir token varsa, kullanıcı kimliğini oluşturur
- Kimlik bilgilerini HttpContext.User'a ekler

### 4. Extensions (Uzantılar)

Kolay entegrasyon için extension metodları sağlar.

**ConfigurationExtensions.cs**

JWT yapılandırma dosyasını yüklemek için extension metodları:

```csharp
public static class ConfigurationExtensions
{
    public static IConfiguration LoadJwtVerifyLibrarySettings(this IConfiguration configuration)
    {
        // Yapılandırma dosyasını yükler
    }
}
```

**ServiceCollectionExtensions.cs**

JWT servislerini uygulamaya eklemek için DI extension metodları:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtVerification(this IServiceCollection services, 
                                                      IConfiguration configuration)
    {
        // JWT servislerini DI container'a kaydeder
        // JWT kimlik doğrulamayı yapılandırır
    }
}
```

**ApplicationBuilderExtensions.cs**

JWT middleware'ini HTTP pipeline'ına eklemek için extension metodları:

```csharp
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseJwtVerification(this IApplicationBuilder app)
    {
        // JWT middleware'ini ekler
    }
}
```

## İşlem Akışı

JWTVerifyLibrary'nin çalışma akışı şu şekildedir:

1. **Yapılandırma**:
   - JwtSettings yapılandırması yüklenir
   - JWT doğrulama parametreleri oluşturulur

2. **Servis Kaydı**:
   - JWT servisleri DI container'a kaydedilir
   - AspNet Core kimlik doğrulama yapılandırılır

3. **HTTP İsteği İşleme**:
   - İstek JwtMiddleware tarafından yakalanır
   - Authorization header'dan JWT token çıkarılır
   - Token JwtService aracılığıyla doğrulanır
   - Geçerli token bulunursa, kullanıcı kimliği oluşturulur
   - İstek, işlem için sonraki middleware'e iletilir

## Veri Akışı Diyagramı

```
┌───────────┐     ┌───────────────┐     ┌────────────────┐
│           │     │               │     │                │
│  HTTP     │────▶│ JwtMiddleware │────▶│  JwtService    │
│  Request  │     │               │     │                │
│           │     └───────────────┘     └────────────────┘
└───────────┘              │                     │
                           │                     │
                           ▼                     │
                ┌────────────────────┐           │
                │                    │           │
                │ User Identity      │◀──────────┘
                │ Creation           │
                │                    │
                └────────────────────┘
                           │
                           │
                           ▼
                ┌────────────────────┐
                │                    │
                │ Next Middleware    │
                │ (Authorization)    │
                │                    │
                └────────────────────┘
```

## Bağımlılıklar

JWTVerifyLibrary aşağıdaki temel bağımlılıkları kullanır:

1. **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT token doğrulaması için temel altyapı
2. **Microsoft.Extensions.Configuration.Abstractions**: Yapılandırma yönetimi için
3. **Microsoft.Extensions.DependencyInjection.Abstractions**: Dependency Injection için
4. **Microsoft.Extensions.Options.ConfigurationExtensions**: Yapılandırma bağlama için

## Genişletilebilirlik

JWTVerifyLibrary, aşağıdaki yollarla genişletilebilir:

1. **Özel Claim Dönüştürücüleri**: Özel claim türleri için dönüştürücüler ekleyebilirsiniz
2. **İşleyici Olayları (Event Handlers)**: Token doğrulama sürecinin çeşitli aşamalarına özel işleyiciler ekleyebilirsiniz
3. **Özel Doğrulama Kuralları**: Standart doğrulama dışında ekstra kurallar ekleyebilirsiniz

### Örnek Genişletme: Özel Claim Eklemek

```csharp
services.AddJwtVerification(Configuration)
    .AddClaimTransformation(transformer =>
    {
        transformer.OnTokenValidated = context =>
        {
            // Token'dan gelen iddialara ek iddialar ekleyin
            var claims = context.Principal.Claims.ToList();
            
            // Örneğin, kullanıcı kimliğinden ek bilgiler alın
            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Veritabanından ek bilgiler alın ve yeni iddialar ekleyin
            claims.Add(new Claim("custom_claim", "custom_value"));
            
            return Task.CompletedTask;
        };
        
        return transformer;
    });
```

## Best Practices

JWTVerifyLibrary kullanırken aşağıdaki en iyi uygulamaları göz önünde bulundurun:

1. **Doğru Pipeline Sırası**: JWT middleware'ini, UseAuthentication() ve UseAuthorization()'dan önce ekleyin
2. **Hata Yönetimi**: JWT hatalarını uygun şekilde ele alın
3. **HTTPS**: Her zaman HTTPS üzerinden JWT kullanın
4. **Strong Typing**: JWT yapılandırma için güçlü tipler kullanın
5. **Secret Yönetimi**: Production ortamlarında secret key'i güvenli bir şekilde saklayın

---

[◀ Gelişmiş Özellikler](05-Gelismis-Ozellikler.md) | [Ana Sayfa](README.md) | [İleri: API Referansı ▶](07-API-Referansi.md) 