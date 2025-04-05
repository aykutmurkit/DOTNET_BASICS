# Hızlı Başlangıç

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölüm, JWTVerifyLibrary'yi hızlı bir şekilde projenize entegre etmeniz ve temel kullanım örneklerini görmeniz için tasarlanmıştır.

## 1. Kütüphaneyi Ekleyin

İlk olarak, JWTVerifyLibrary'yi projenize ekleyin. Detaylı kurulum bilgileri için [Kurulum](02-Kurulum.md) bölümüne bakınız.

```powershell
Install-Package JWTVerifyLibrary
```

## 2. Yapılandırma Ekleyin

appsettings.json dosyanıza JWT ayarlarını ekleyin:

```json
{
  "JwtSettings": {
    "Secret": "SizinGizliAnahtariniz12345678901234567890",
    "Issuer": "UygulamaninAdi",
    "Audience": "HedefKitle",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

## 3. Program.cs Dosyasında Servisleri Kaydedin

Program.cs veya Startup.cs dosyanızda, aşağıdaki adımları uygulayın:

```csharp
using JWTVerifyLibrary.Extensions;

// ...

// JWT servisleri ve doğrulama ekle
builder.Services.AddJwtVerification(builder.Configuration);

// ...

// JWT middleware'i pipeline'a ekle (UseAuthentication'dan önce)
app.UseJwtVerification();
app.UseAuthentication();
app.UseAuthorization();
```

## 4. Controller'larda [Authorize] Özniteliğini Kullanın

JWT doğrulaması ile korunacak controller'lar veya action'lar için [Authorize] özniteliğini kullanın:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PublicEndpoint()
        {
            return Ok("Bu endpoint herkese açık!");
        }

        [HttpGet("protected")]
        [Authorize] // JWT doğrulaması gerektirir
        public IActionResult ProtectedEndpoint()
        {
            return Ok("Sadece kimliği doğrulanmış kullanıcılar görebilir!");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // Belirli role sahip kullanıcılar için
        public IActionResult AdminEndpoint()
        {
            return Ok("Sadece Admin rolündeki kullanıcılar görebilir!");
        }
    }
}
```

## 5. JWT İle İstek Gönderme

JWT token'ınızı HTTP isteklerinde Authorization header'ı ile gönderin:

```http
GET /api/sample/protected HTTP/1.1
Host: yourapi.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Curl Örneği

```bash
curl -X GET "https://yourapi.com/api/sample/protected" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Swagger UI Kullanımı

Swagger UI üzerinden JWT kimlik doğrulamasını kullanmak için:

1. Swagger UI arayüzünü açın
2. Sağ üst köşedeki "Authorize" düğmesine tıklayın
3. Açılan pencerede "Bearer" alanına JWT token'ınızı girin
4. "Authorize" düğmesine tıklayın ve pencereyi kapatın
5. Artık korumalı endpoint'leri test edebilirsiniz

## 6. Swagger Yapılandırması

JWT kimlik doğrulamasını Swagger ile entegre etmek için:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "YourApi", Version = "v1" });
    
    // JWT doğrulaması için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
```

## 7. Tam Örnek

Aşağıda, JWTVerifyLibrary'nin temel kurulumunu ve kullanımını gösteren tam bir MinimalAPI örneği bulabilirsiniz:

```csharp
using JWTVerifyLibrary.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Swagger/OpenAPI ekleme
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Test API", Version = "v1" });
    
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

// JWTVerifyLibrary ekleme
builder.Services.AddJwtVerification(builder.Configuration);

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT doğrulama middleware'i
app.UseJwtVerification();

app.UseAuthentication();
app.UseAuthorization();

// Test endpoint'leri
app.MapGet("/public", () => "Bu endpoint herkese açık!")
   .WithName("GetPublic");

app.MapGet("/protected", [Authorize] (HttpContext context) =>
{
    var userId = context.User.FindFirst("nameid")?.Value;
    var username = context.User.FindFirst("unique_name")?.Value;
    
    return $"Hoş geldiniz, {username}! Kullanıcı ID: {userId}";
})
.WithName("GetProtected");

app.MapGet("/admin", [Authorize(Roles = "Admin")] () => "Bu endpoint sadece Admin rolüne sahip kullanıcılar için!")
   .WithName("GetAdmin");

app.Run();
```

## Sonraki Adımlar

Temel kurulumu ve kullanımı öğrendiğinize göre, şimdi şunlara göz atabilirsiniz:

- [Temel Kullanım](04-Temel-Kullanim.md) - Daha fazla kullanım örneği
- [Gelişmiş Özellikler](05-Gelismis-Ozellikler.md) - Kütüphanenin gelişmiş özellikleri
- [Mimari Yapı](06-Mimari-Yapi.md) - Kütüphanenin iç yapısı
- [API Referansı](07-API-Referansi.md) - Tüm sınıflar ve metotlar

---

[◀ Kurulum](02-Kurulum.md) | [Ana Sayfa](README.md) | [İleri: Temel Kullanım ▶](04-Temel-Kullanim.md) 