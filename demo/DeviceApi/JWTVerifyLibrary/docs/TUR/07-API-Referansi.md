# API Referansı

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu belge, JWTVerifyLibrary'nin tüm genel API'lerini detaylı olarak açıklar.

## JwtService

`JwtService` sınıfı, JWT token'larını doğrulamak ve işlemek için temel işlevler sağlar.

### Interface

```csharp
public interface IJwtService
{
    bool ValidateToken(string token, out JwtSecurityToken? validatedToken);
    IEnumerable<Claim> GetTokenClaims(string token);
    ClaimsPrincipal GetClaimsPrincipal(string token);
}
```

### Metodlar

#### ValidateToken

```csharp
bool ValidateToken(string token, out JwtSecurityToken? validatedToken)
```

**Açıklama**: Bir JWT token'ın geçerliliğini doğrular.

**Parametreler**:
- `token` (string): Doğrulanacak JWT token.
- `validatedToken` (out JwtSecurityToken?): Eğer doğrulama başarılıysa, doğrulanmış token nesnesi.

**Dönüş Değeri**:
- `bool`: Token doğrulaması başarılıysa `true`, değilse `false`.

**Örnekler**:

```csharp
// JWT token'ını doğrulama
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

bool isValid = jwtService.ValidateToken(jwtToken, out var validatedToken);

if (isValid)
{
    Console.WriteLine("Token is valid!");
    Console.WriteLine($"Expiration: {validatedToken.ValidTo}");
}
else
{
    Console.WriteLine("Token is invalid or expired.");
}
```

**Hata Durumları**:
- `ArgumentNullException`: Token null ya da boş ise.
- `SecurityTokenException`: Token formatı geçersizse veya doğrulama başarısız olursa.

#### GetTokenClaims

```csharp
IEnumerable<Claim> GetTokenClaims(string token)
```

**Açıklama**: Doğrulanmış bir JWT token'dan tüm claim'leri çıkarır.

**Parametreler**:
- `token` (string): Claim'lerin alınacağı JWT token.

**Dönüş Değeri**:
- `IEnumerable<Claim>`: Token'dan çıkarılan claim'lerin listesi.

**Örnekler**:

```csharp
// JWT token'dan claim'leri alma
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

IEnumerable<Claim> claims = jwtService.GetTokenClaims(jwtToken);

foreach (var claim in claims)
{
    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
}
```

**Hata Durumları**:
- `ArgumentNullException`: Token null ya da boş ise.
- `SecurityTokenException`: Token formatı geçersizse veya doğrulama başarısız olursa.

#### GetClaimsPrincipal

```csharp
ClaimsPrincipal GetClaimsPrincipal(string token)
```

**Açıklama**: Doğrulanmış bir JWT token'dan ClaimsPrincipal nesnesi oluşturur.

**Parametreler**:
- `token` (string): ClaimsPrincipal'in oluşturulacağı JWT token.

**Dönüş Değeri**:
- `ClaimsPrincipal`: Token'a dayalı oluşturulan kimlik bilgisi.

**Örnekler**:

```csharp
// JWT token'dan ClaimsPrincipal oluşturma
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

ClaimsPrincipal principal = jwtService.GetClaimsPrincipal(jwtToken);

// Kullanıcı ID alma
string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
```

**Hata Durumları**:
- `ArgumentNullException`: Token null ya da boş ise.
- `SecurityTokenException`: Token formatı geçersizse veya doğrulama başarısız olursa.

## JwtMiddleware

`JwtMiddleware` sınıfı, HTTP isteklerini yakalamak ve JWT token'ları doğrulamak için bir ASP.NET Core middleware bileşenidir.

### Sınıf Yapısı

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
        // Middleware işlem mantığı
    }
}
```

### Metodlar

#### InvokeAsync

```csharp
public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
```

**Açıklama**: Her HTTP isteği için çalışan middleware'in ana metodu.

**Parametreler**:
- `context` (HttpContext): Geçerli HTTP isteğinin bağlamı.
- `jwtService` (IJwtService): JWT token işlemleri için servis.

**İşlem Akışı**:
1. Authorization başlığından Bearer token'ı çıkarır.
2. Token'ı doğrular.
3. Doğrulama başarılıysa, kullanıcı kimliğini oluşturur ve `HttpContext.User`'a atar.
4. Pipeline'daki sonraki middleware'e geçer.

## ServiceCollectionExtensions

`ServiceCollectionExtensions` sınıfı, JWT doğrulama servislerini ASP.NET Core uygulamasına eklemek için extension metodlar sağlar.

### Metodlar

#### AddJwtVerification

```csharp
public static IServiceCollection AddJwtVerification(
    this IServiceCollection services, 
    IConfiguration configuration)
```

**Açıklama**: JWT doğrulama servislerini ve yapılandırmasını uygulamaya ekler.

**Parametreler**:
- `services` (IServiceCollection): Servis koleksiyonu.
- `configuration` (IConfiguration): Uygulama yapılandırması.

**Dönüş Değeri**:
- `IServiceCollection`: Servis koleksiyonu, method chaining için.

**Örnekler**:

```csharp
// Program.cs'de servis kaydı
builder.Services.AddJwtVerification(builder.Configuration);
```

#### AddJwtVerification (Gelişmiş)

```csharp
public static IServiceCollection AddJwtVerification(
    this IServiceCollection services, 
    IConfiguration configuration,
    Action<JwtOptions> configureOptions)
```

**Açıklama**: JWT doğrulama servislerini uygulamaya ekler ve özel yapılandırma sağlar.

**Parametreler**:
- `services` (IServiceCollection): Servis koleksiyonu.
- `configuration` (IConfiguration): Uygulama yapılandırması.
- `configureOptions` (Action<JwtOptions>): JWT seçeneklerini yapılandırmak için callback.

**Dönüş Değeri**:
- `IServiceCollection`: Servis koleksiyonu, method chaining için.

**Örnekler**:

```csharp
// Program.cs'de gelişmiş servis kaydı
builder.Services.AddJwtVerification(builder.Configuration, options => 
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Özel parametreler
    };
});
```

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` sınıfı, JWT middleware'ini ASP.NET Core uygulama pipeline'ına eklemek için extension metodlar sağlar.

### Metodlar

#### UseJwtVerification

```csharp
public static IApplicationBuilder UseJwtVerification(this IApplicationBuilder app)
```

**Açıklama**: JWT doğrulama middleware'ini uygulama pipeline'ına ekler.

**Parametreler**:
- `app` (IApplicationBuilder): Uygulama oluşturucusu.

**Dönüş Değeri**:
- `IApplicationBuilder`: Uygulama oluşturucusu, method chaining için.

**Örnekler**:

```csharp
// Middleware kaydı
app.UseJwtVerification();
```

**Not**: Bu middleware'i `UseAuthentication()` ve `UseAuthorization()`'dan önce ekleyin.

## JwtSettings

`JwtSettings` sınıfı, JWT yapılandırma ayarlarını tutmak için bir model sınıfıdır.

### Özellikler

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

**Açıklama**:

- `Secret`: JWT imzalaması için kullanılan gizli anahtar.
- `Issuer`: Token'ı veren varlık (genellikle API veya uygulama adı/URL'si).
- `Audience`: Token'ın hedef kitlesi.
- `AccessTokenExpirationInMinutes`: Access token'ın dakika cinsinden geçerlilik süresi.
- `RefreshTokenExpirationInDays`: Refresh token'ın gün cinsinden geçerlilik süresi.

## Olay İşleyicileri

JWTVerifyLibrary, token doğrulama sürecinin çeşitli aşamalarını özelleştirmek için olay işleyicileri sağlar.

### TokenValidatedHandler

```csharp
public delegate Task TokenValidatedHandler(TokenValidatedContext context);
```

**Açıklama**: Token doğrulandıktan sonra tetiklenir.

**Kullanım**:

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidated = async context =>
        {
            // Token doğrulandıktan sonraki işlemler
            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Örnek: Kullanıcının veritabanında olup olmadığını kontrol etme
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            if (!await userService.IsActiveUser(userId))
            {
                context.Fail("User is not active");
            }
        };
    });
```

### TokenValidationFailedHandler

```csharp
public delegate Task TokenValidationFailedHandler(TokenValidationFailedContext context);
```

**Açıklama**: Token doğrulama başarısız olduğunda tetiklenir.

**Kullanım**:

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidationFailed = async context =>
        {
            // Token doğrulama başarısız olduğunda işlemler
            
            // Örnek: Hata ayrıntılarını loglama
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtMiddleware>>();
            logger.LogWarning("JWT validation failed: {Exception}", context.Exception.Message);
            
            // Özel yanıt gönderme
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Invalid token" }));
            
            // Sonraki middleware'leri çalıştırma
            context.Handled = true;
        };
    });
```

## Genişletilebilirlik Arayüzleri

JWTVerifyLibrary, özelleştirme ve genişletme için çeşitli arayüzler sağlar.

### IClaimsTransformer

```csharp
public interface IClaimsTransformer
{
    Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal);
}
```

**Açıklama**: JWT token'dan çıkarılan claims'leri dönüştürmek için kullanılır.

**Kullanım**:

```csharp
// Özel claims dönüştürücü
public class CustomClaimsTransformer : IClaimsTransformer
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        
        if (identity == null)
            return principal;
            
        // E-posta iddiasına dayalı ek rol iddiası ekleme
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email != null && email.EndsWith("@admin.com"))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
        }
        
        return principal;
    }
}

// Servis kaydı
services.AddSingleton<IClaimsTransformer, CustomClaimsTransformer>();
services.AddJwtVerification(Configuration);
```

---

[◀ Mimari Yapı](06-Mimari-Yapi.md) | [Ana Sayfa](README.md) | [İleri: Versiyonlama ▶](08-Versiyonlama.md) 