# JWTVerifyLibrary Dokümantasyonu

## Genel Bakış

JWTVerifyLibrary, ASP.NET Core uygulamalarında JWT (JSON Web Token) doğrulamasını basitleştirmek için tasarlanmış özel bir .NET kütüphanesidir. Bu kütüphane, JWT token'larını doğrulamak, claims (iddialar) bilgilerini çıkarmak ve API endpoint'lerini minimal konfigürasyonla güvence altına almak için kolay kullanımlı bir çözüm sunar.

## Özellikler

- Herhangi bir ASP.NET Core uygulamasında JWT doğrulaması için tek satırlık kurulum
- Endüstri standardı güvenlik uygulamalarıyla token doğrulama
- Claims (iddialar) çıkarma ve kullanıcı kimliği atama
- ASP.NET Core kimlik doğrulama pipeline'ı ile entegre olan middleware
- appsettings.json üzerinden yapılandırma desteği

## Kurulum

1. Çözümünüze JWTVerifyLibrary projesine referans ekleyin:
   ```xml
   <ItemGroup>
     <ProjectReference Include="path\to\JWTVerifyLibrary\JWTVerifyLibrary.csproj" />
   </ItemGroup>
   ```

2. Uygulamanızın appsettings.json dosyasında JWT ayarlarının bulunduğundan emin olun:
   ```json
   {
     "JwtSettings": {
       "Secret": "GizliAnahtarınız",
       "Issuer": "TokenÜreticiniz",
       "Audience": "HedefKitleniz",
       "AccessTokenExpirationInMinutes": 60,
       "RefreshTokenExpirationInDays": 7
     }
   }
   ```

## Kullanım

### Temel Kurulum

JWTVerifyLibrary'yi ASP.NET Core uygulamanıza entegre etmek için aşağıdaki basit adımları izleyin:

1. Program.cs veya Startup.cs dosyanızın en üstüne gerekli using ifadesini ekleyin:
   ```csharp
   using JWTVerifyLibrary.Extensions;
   ```

2. `ConfigureServices` metodunda veya builder konfigürasyonunda JWT doğrulama servislerini kaydedin:
   ```csharp
   // Program.cs (minimal API) içinde
   builder.Services.AddJwtVerification(builder.Configuration);

   // VEYA Startup.cs içinde
   services.AddJwtVerification(Configuration);
   ```

3. JWT doğrulama middleware'ini uygulama pipeline'ına ekleyin:
   ```csharp
   // Program.cs (minimal API) içinde
   app.UseJwtVerification();

   // Middleware pipeline'ında doğru sırayla eklediğinizden emin olun
   app.UseHttpsRedirection();
   app.UseJwtVerification();
   app.UseAuthentication();
   app.UseAuthorization();
   ```

### Gelişmiş Yapılandırma

Sağlanan servislerle doğrudan çalışarak JWT doğrulama sürecini özelleştirebilirsiniz:

```csharp
// JWT servisini controller'larınıza veya servislerinize enjekte edin
private readonly IJwtService _jwtService;

public SizinController(IJwtService jwtService)
{
    _jwtService = jwtService;
}

// Bir token'ı manuel olarak doğrulayın
public IActionResult TokenDogrula(string token)
{
    bool gecerliMi = _jwtService.ValidateToken(token, out var dogrulanmisToken);
    
    if (gecerliMi)
    {
        // Token geçerli
        var iddialar = _jwtService.GetTokenClaims(token);
        return Ok(iddialar);
    }
    
    return Unauthorized();
}
```

### Controller'ları ve Action'ları Koruma

Kütüphaneyi kurduktan sonra, API endpoint'lerinizi korumak için standart ASP.NET Core özniteliklerini kullanabilirsiniz:

```csharp
[Authorize]
public class GuvenliController : ControllerBase
{
    [HttpGet]
    public IActionResult GuvenliVeriAl()
    {
        return Ok("Bu güvenli veridir!");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult SadeceAdmin()
    {
        return Ok("Bu sadece adminler içindir!");
    }
}
```

## Yapılandırma Seçenekleri

Kütüphane, appsettings.json'dan aşağıdaki yapılandırma seçeneklerini kullanır:

| Ayar | Açıklama |
|------|----------|
| Secret | JWT token'larını imzalamak ve doğrulamak için kullanılan gizli anahtar |
| Issuer | JWT token'ın üreticisi (genellikle kimlik doğrulama sunucunuz) |
| Audience | JWT token'ın hedeflenen alıcısı (API'niz) |
| AccessTokenExpirationInMinutes | Erişim token'larının dakika cinsinden sona erme süresi |
| RefreshTokenExpirationInDays | Yenileme token'larının gün cinsinden sona erme süresi |

## Güvenlik Hususları

JWTVerifyLibrary çeşitli güvenlik en iyi uygulamalarını uygular:

1. **Token İmza Doğrulaması**: Token'ın değiştirilmediğinden emin olur
2. **Issuer ve Audience Doğrulaması**: Token'ın güvenilir bir kaynaktan geldiğini ve uygulamanız için amaçlandığını onaylar
3. **Ömür Doğrulaması**: Süresi dolmuş token'ları otomatik olarak reddeder
4. **Sıfır Saat Farkı (Zero Clock Skew)**: Token sona erme kontrollerinde herhangi bir tolerans uygulanmaz

## Sorun Giderme

Token doğrulamayla ilgili sorunlarla karşılaşırsanız:

1. appsettings.json'daki JWT gizli anahtarınızın token'ları oluşturmak için kullanılanla eşleştiğini kontrol edin
2. Issuer ve Audience değerlerinin token oluşturma ve doğrulama arasında eşleştiğini doğrulayın
3. Token'ların süresinin dolmadığından emin olun
4. Token'ın HMAC SHA256 algoritmasını kullandığını kontrol edin

## Uygulama Detayları

Kütüphane şunlardan oluşur:

1. **JwtService**: Token doğrulama ve claims çıkarma için temel servis
2. **JwtMiddleware**: İstekleri yakalayan, token'ları doğrulayan ve kullanıcı kimliği atayan middleware
3. **Extension Metodları**: ASP.NET Core'un bağımlılık enjeksiyonu ve middleware pipeline'ı ile kolay entegrasyon için

## Örnek Uygulama

İşte JWTVerifyLibrary'yi bir ASP.NET Core uygulamasında nasıl kuracağınıza dair tam bir örnek:

```csharp
// Program.cs
using JWTVerifyLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Container'a servisler ekleyin
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT doğrulama ekleyin
builder.Services.AddJwtVerification(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT doğrulama kullanın
app.UseJwtVerification();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
``` 