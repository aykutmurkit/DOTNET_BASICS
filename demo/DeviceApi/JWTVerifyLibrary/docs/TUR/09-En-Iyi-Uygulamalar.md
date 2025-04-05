# En İyi Uygulamalar

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölüm, JWTVerifyLibrary'yi projelerinizde en verimli ve güvenli şekilde kullanmak için en iyi uygulamaları ve önerileri içerir.

## Güvenlik

### Token İmza Anahtarı (Secret Key)

- **Strong Secret Key Kullanın**: En az 256 bit (32 karakter) uzunluğunda, rastgele oluşturulmuş bir secret key kullanın.

```csharp
// ❌ Zayıf secret key - KULLANMAYIN
"MysecretKey123"

// ✅ Güçlü secret key - KULLANIN
"a5J#9$rTp2!kLzX&cB7E@qYm8NwDvF3h"
```

- **Ortam Değişkenlerinde Saklayın**: Secret key'i, appsettings.json dosyasında saklamak yerine ortam değişkenlerinde saklayın.

```csharp
// Program.cs
builder.Configuration["JwtSettings:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
```

- **Düzenli Olarak Değiştirin**: Secret key'inizi üretim ortamında düzenli olarak değiştirin (örneğin, her 90 günde bir).

### HTTPS Kullanımı

- **Her Zaman HTTPS Üzerinden Kullanın**: JWT token'ları ağ üzerinde hiçbir zaman şifrelenmemiş olarak gönderilmemelidir.

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
```

- **Development Ortamında Bile HTTPS Kullanın**: Yerel geliştirme ortamında bile HTTPS kullanmak, üretim ortamındaki sorunları erken tespit etmenize yardımcı olur.

### Ömür Süresi ve Yenileme

- **Kısa Ömürlü Access Token'ları Kullanın**: Access token'ların ömrünü 15-60 dakika gibi kısa bir süreyle sınırlandırın.

```json
// appsettings.json
{
  "JwtSettings": {
    "AccessTokenExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 7
  }
}
```

- **Refresh Token Stratejisi Uygulayın**: Güvenli bir refresh token stratejisi uygulayarak, access token'ların kısa ömürlü olmasına rağmen kullanıcıların sürekli oturum açmasını önleyin.

- **Absolute ve Sliding Expiration Kullanın**: Access token'lar için absolute expiration, refresh token'lar için sliding expiration kullanmayı düşünün.

### Claims ve Yetkilendirme

- **Minimum Gerekli Claims Kullanın**: Token'larda yalnızca gerçekten ihtiyaç duyduğunuz claim'leri saklayın.

```csharp
// ❌ Fazla veri içeren token - KULLANMAYIN
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.GivenName, user.FirstName),
    new Claim(ClaimTypes.Surname, user.LastName),
    new Claim("PhoneNumber", user.PhoneNumber),
    new Claim("Address", user.Address),
    // Kullanıcıya dair daha birçok bilgi...
};

// ✅ Sadece gerekli claims içeren token - KULLANIN
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.Role)
};
```

- **Role-based ve Policy-based Yetkilendirme Kullanın**: Controller veya endpoint seviyesinde güvenlik için [Authorize] özniteliğini policy'lerle birlikte kullanın.

```csharp
// Controller sınıfı
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : ControllerBase
{
    // Controller metodları
}

// Program.cs'de policy tanımı
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});
```

## Performans

### Servis Yapılandırması

- **Singleton Kullanın**: JwtService'i DI container'da singleton olarak kaydedin.

```csharp
// Program.cs
services.AddSingleton<IJwtService, JwtService>();
```

- **TokenValidationParameters'ı Önbelleğe Alın**: Validasyon parametrelerini her defasında oluşturmak yerine, bir kez oluşturup yeniden kullanın.

```csharp
// JwtService.cs
private readonly TokenValidationParameters _validationParameters;

public JwtService(IOptions<JwtSettings> settings)
{
    var jwtSettings = settings.Value;
    _validationParameters = new TokenValidationParameters
    {
        // Parametreler...
    };
}
```

### Bellek Optimizasyonu

- **Gereksiz Token Doğrulamalarından Kaçının**: Token'ı birden fazla kez doğrulamaktan kaçının. Doğrulama sonuçlarını HttpContext.Items içinde saklayın.

```csharp
// Middleware.cs
if (!context.Items.ContainsKey("TokenValidated"))
{
    // Token doğrulama
    context.Items["TokenValidated"] = true;
}
```

- **StringSegment Kullanın**: Authorization header'ı işlerken string bölme ve birleştirme işlemlerini minimize etmek için StringSegment kullanın.

```csharp
// ❌ Gereksiz string işlemleri - KULLANMAYIN
var authHeader = context.Request.Headers["Authorization"].ToString();
if (authHeader.StartsWith("Bearer "))
{
    var token = authHeader.Substring(7);
    // İşlem...
}

// ✅ StringSegment kullanımı - KULLANIN
var authHeader = context.Request.Headers["Authorization"];
if (authHeader.Count > 0 && authHeader[0].StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
{
    var tokenSegment = new StringSegment(authHeader[0], 7, authHeader[0].Length - 7);
    // İşlem...
}
```

## Entegrasyon

### Middleware Pipeline

- **Doğru Sırada Middleware Ekleyin**: JWTVerifyLibrary middleware'ini UseAuthentication() ve UseAuthorization()'dan önce ekleyin.

```csharp
// Program.cs
app.UseJwtVerification();
app.UseAuthentication();
app.UseAuthorization();
```

### İstisna Yönetimi

- **İstisna Filtreleri Kullanın**: JWT doğrulama hatalarını yakalamak ve yönetmek için global istisna filtreleri ekleyin.

```csharp
// JwtExceptionFilter.cs
public class JwtExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is SecurityTokenException)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Geçersiz token" });
            context.ExceptionHandled = true;
        }
    }
}

// Program.cs
services.AddControllers(options =>
{
    options.Filters.Add<JwtExceptionFilter>();
});
```

### Loglama

- **Token Doğrulama Olaylarını Loglayın**: Potansiyel güvenlik sorunlarını tespit etmek için token doğrulama başarısızlıklarını loglayın.

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtMiddleware>>();
            logger.LogWarning("JWT validation failed: {Exception}", context.Exception.Message);
            return Task.CompletedTask;
        };
    });
```

- **PII (Personally Identifiable Information) İçeren Verileri Loglamayın**: Loglarınızda kişisel verilerin yer almamasına dikkat edin.

## Kurulum ve Yapılandırma

### Yapılandırma Yönetimi

- **IOptions Pattern Kullanın**: JWT ayarları için IOptions pattern kullanın.

```csharp
// Program.cs
services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

// JwtService.cs
public JwtService(IOptions<JwtSettings> settings)
{
    _settings = settings.Value;
}
```

- **Testing İçin Fake Implementation Kullanın**: Birim testleri için JWT servisinin sahte (fake) bir implementasyonunu kullanın.

```csharp
// Test sınıfı
public class AuthControllerTests
{
    [Fact]
    public void Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var fakeJwtService = new FakeJwtService();
        var controller = new AuthController(fakeJwtService);
        
        // Act & Assert
    }
}
```

### Referanslar ve Bağımlılıklar

- **Açık Versiyon Referansları Kullanın**: NuGet paketlerini referans gösterirken açık sürüm numaraları kullanın.

```xml
<!-- ❌ Belirsiz versiyon - KULLANMAYIN -->
<PackageReference Include="JWTVerifyLibrary" Version="1.*" />

<!-- ✅ Açık versiyon numarası - KULLANIN -->
<PackageReference Include="JWTVerifyLibrary" Version="1.0.0" />
```

## Best Practices Checklist

Projelerinizde JWTVerifyLibrary kullanırken bu kontrol listesini takip edin:

1. **Güvenlik**
   - [ ] Güçlü ve güvenli bir secret key kullanıldı (en az 32 karakter, rastgele)
   - [ ] Secret key güvenli bir şekilde saklanıyor (ortam değişkenleri, Azure Key Vault vb.)
   - [ ] HTTPS zorunlu hale getirildi
   - [ ] Token süresi optimal olarak ayarlandı (15-60 dakika)
   - [ ] Refresh token stratejisi uygulandı

2. **Performans**
   - [ ] Servisler doğru şekilde kaydedildi (singleton)
   - [ ] Önbelleğe alma stratejileri uygulandı
   - [ ] Gereksiz doğrulamalar önlendi

3. **Entegrasyon**
   - [ ] Middleware doğru sırada eklendi
   - [ ] Hata yönetimi uygun şekilde yapılandırıldı
   - [ ] Loglama stratejisi uygulandı

4. **Yapılandırma**
   - [ ] IOptions pattern kullanıldı
   - [ ] Yapılandırma test edilebilir şekilde tasarlandı
   - [ ] Paket referansları doğru şekilde belirtildi

## Antipatternler ve Kaçınılması Gerekenler

Aşağıdaki uygulamalardan kaçının:

- **Secret Key'i Hard-coded Olarak Tutmak**: Kod içinde veya yapılandırma dosyalarında açık metin olarak secret key tutmayın.
- **Uzun Süreli Token'lar**: Access token'ların ömrünü çok uzun tutmayın.
- **Token'da Aşırı Veri**: JWT token'ları büyük veri parçalarını saklamak için tasarlanmamıştır.
- **Client Tarafında Hassas Veri**: Token içinde client'a göndermek istemediğiniz hassas verileri saklamayın.
- **Yetersiz İstisna Yönetimi**: Doğrulama hatalarını düzgün şekilde ele alın ve loglayın.

## Daha Fazla Kaynak

- [JWT Resmi Websitesi](https://jwt.io/)
- [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
- [Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)

---

[◀ Versiyonlama](08-Versiyonlama.md) | [Ana Sayfa](README.md) | [İleri: Sık Sorulan Sorular ▶](10-Sikca-Sorulan-Sorular.md) 