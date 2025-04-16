# 03 - Kimlik Doğrulama ve Yetkilendirme

Bu bölüm, AuthApi'nin kimlik doğrulama ve yetkilendirme altyapısını detaylı olarak açıklamaktadır.

## JWT Tabanlı Kimlik Doğrulama

AuthApi, kimlik doğrulama için JWT (JSON Web Token) kullanmaktadır. JWT, kullanıcı kimlik bilgilerini güvenli bir şekilde iletmek ve doğrulamak için endüstri standardı bir yöntemdir.

### Token Tipleri

Sistem iki tip token kullanır:

1. **Access Token**: Kısa ömürlü, API isteklerinde kullanılan ana token
2. **Refresh Token**: Uzun ömürlü, access token'ın süresi dolduğunda yenisini almak için kullanılan token

### JWT Yapılandırması

JWT ayarları `appsettings.json` dosyasında tanımlanır:

```json
"JwtSettings": {
  "Secret": "VerySecureSecretKey12345678901234567890",
  "Issuer": "DenemeApi",
  "Audience": "DenemeApiClient",
  "AccessTokenExpirationInMinutes": 660,
  "RefreshTokenExpirationInDays": 7
}
```

### JWT Helper

JWT işlemleri, `JwtHelper` sınıfı tarafından yönetilir:

```csharp
public class JwtHelper
{
    private readonly JwtOptions _jwtOptions;
    
    public JwtHelper(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    // Access token üretme
    public AccessToken GenerateAccessToken(User user, List<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        // Rolleri claim olarak ekle
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationInMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        return new AccessToken
        {
            Token = tokenString,
            ExpiresAt = tokenDescriptor.Expires.Value
        };
    }
    
    // Refresh token üretme
    public RefreshToken GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays)
        };
    }
}
```

### Token Yenileme Akışı

Access token'ın süresi dolduğunda, refresh token kullanılarak yeni bir token çifti alınabilir:

```csharp
// AuthController.cs'den refresh token örneği
[HttpPost("refresh-token")]
public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
{
    try
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Ok(ApiResponse<AuthResponse>.Success(result, "Token başarıyla yenilendi"));
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("Geçersiz") || ex.Message.Contains("süresi dolmuş"))
        {
            return StatusCode(401, ApiResponse<AuthResponse>.Unauthorized(ex.Message));
        }
        return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
    }
}
```

## İki Faktörlü Kimlik Doğrulama (2FA)

AuthApi, ek bir güvenlik katmanı olarak iki faktörlü kimlik doğrulama (2FA) sunar. Bu sistem, kullanıcının e-posta adresine gönderilen tek kullanımlık kod kullanılarak çalışır.

### 2FA Sistemi Özellikleri

- Sistem genelinde 2FA'yı etkinleştirme/devre dışı bırakma
- Tüm kullanıcılar için zorunlu kılma seçeneği
- Kullanıcı bazında etkinleştirme/devre dışı bırakma
- Zaman sınırlı doğrulama kodları
- Güvenli rastgele kod üretimi

### 2FA Yapılandırması

2FA ayarları `appsettings.json` dosyasında tanımlanır:

```json
"TwoFactorSettings": {
  "SystemEnabled": true,
  "RequiredForAllUsers": false,
  "CodeLength": 6,
  "ExpirationMinutes": 10
}
```

### 2FA Servisi

2FA işlemleri, `TwoFactorService` sınıfı tarafından yönetilir:

```csharp
public class TwoFactorService : ITwoFactorService
{
    private readonly IConfiguration _configuration;

    public TwoFactorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // 2FA sistemin aktif olup olmadığını kontrol eder
    public bool IsTwoFactorEnabled()
    {
        return _configuration.GetValue<bool>("TwoFactorSettings:SystemEnabled");
    }

    // 2FA'nın tüm kullanıcılar için zorunlu olup olmadığını kontrol eder
    public bool IsTwoFactorRequired()
    {
        return _configuration.GetValue<bool>("TwoFactorSettings:RequiredForAllUsers");
    }

    // Kullanıcı için 2FA gerekip gerekmediğini belirler
    public bool IsTwoFactorRequiredForUser(User user)
    {
        if (!IsTwoFactorEnabled())
            return false;

        if (IsTwoFactorRequired())
            return true;

        return user.TwoFactorEnabled;
    }

    // Yeni bir 2FA kodu oluşturur
    public string GenerateNewCodeForUser(User user)
    {
        var codeLength = _configuration.GetValue<int>("TwoFactorSettings:CodeLength");
        var expirationMinutes = _configuration.GetValue<int>("TwoFactorSettings:ExpirationMinutes");

        var code = GenerateRandomNumericCode(codeLength);

        user.TwoFactorCode = code;
        user.TwoFactorCodeCreatedAt = DateTime.UtcNow;
        user.TwoFactorCodeExpirationMinutes = expirationMinutes;

        return code;
    }

    // 2FA kodunu doğrular
    public bool ValidateCodeForUser(User user, string code)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(user.TwoFactorCode) || !user.TwoFactorCodeCreatedAt.HasValue)
            return false;

        var expirationTime = user.TwoFactorCodeCreatedAt.Value.AddMinutes(user.TwoFactorCodeExpirationMinutes);
        if (DateTime.UtcNow > expirationTime)
            return false;

        return user.TwoFactorCode == code;
    }

    // Rastgele sayısal kod üretir
    private string GenerateRandomNumericCode(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = (char)('0' + (bytes[i] % 10));
        }

        return new string(result);
    }
}
```

### 2FA Akışı

1. Kullanıcı, geçerli kullanıcı adı ve şifreyle giriş yapar
2. Sistem, kullanıcı için 2FA gerekiyorsa özel bir yanıt döndürür
3. Kullanıcının e-posta adresine bir doğrulama kodu gönderilir
4. Kullanıcı, kodu girerek kimlik doğrulamasını tamamlar
5. Başarılı doğrulama sonrası normal giriş yanıtı (JWT token'ları) döndürülür

```csharp
// AuthController.cs'den 2FA doğrulama örneği
[HttpPost("verify-2fa")]
[EnableRateLimiting("api_auth_verify-2fa")]
public async Task<ActionResult<ApiResponse<AuthResponse>>> VerifyTwoFactor([FromBody] TwoFactorVerifyRequest request)
{
    await _logService.LogInfoAsync(
        message: "2FA doğrulama işlemi başlatıldı", 
        source: "AuthController.VerifyTwoFactor",
        data: new { UserId = request.UserId });
        
    try
    {
        var result = await _authService.VerifyTwoFactorAsync(request);
        
        await _logService.LogInfoAsync(
            message: "2FA doğrulama başarılı", 
            source: "AuthController.VerifyTwoFactor",
            userId: result.User.Id.ToString(),
            userName: result.User.Username);
            
        return Ok(ApiResponse<AuthResponse>.Success(result, "İki faktörlü kimlik doğrulama başarılı"));
    }
    catch (Exception ex)
    {
        await _logService.LogErrorAsync(
            message: "2FA doğrulama başarısız", 
            source: "AuthController.VerifyTwoFactor",
            exception: ex,
            userId: request.UserId.ToString());
            
        if (ex.Message.Contains("Doğrulama kodu geçersiz"))
        {
            return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
        }
        return StatusCode(500, ApiResponse<AuthResponse>.ServerError(ex.Message));
    }
}
```

## Rol Tabanlı Erişim Kontrolü (RBAC)

AuthApi, farklı kullanıcı rollerine göre erişim kontrolü sağlar. Bu, .NET Core'un `[Authorize]` özelliği ile uygulanır.

### Kullanıcı Rolleri

Sistem, standart olarak üç rol tanımlar:

1. **Admin**: Tam yönetici erişimi (tüm API'lere erişebilir)
2. **Developer**: Geliştirici erişimi (kısıtlı yönetim API'lerine erişebilir)
3. **User**: Standart kullanıcı erişimi (sadece kullanıcı işlemlerine erişebilir)

### Rol Bazlı Erişim Örneği

```csharp
// Sadece Admin rolüne sahip kullanıcıların erişebileceği endpoint
[HttpGet]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
{
    // ...
}

// Admin veya Developer rolüne sahip kullanıcıların erişebileceği endpoint
[HttpGet("{id}")]
[Authorize(Roles = "Admin,Developer")]
public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
{
    // ...
}

// Herhangi bir kimliği doğrulanmış kullanıcının erişebileceği endpoint
[HttpGet("profile")]
[Authorize]
public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
{
    // ...
}
```

## Şifre Yönetimi

AuthApi, güvenli şifre yönetimi için çeşitli özellikler sunar.

### Şifre Hashleme

Şifreler, güvenli bir şekilde hash'lenerek saklanır:

```csharp
public class PasswordHelper
{
    // Şifre hashleme
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    // Şifre doğrulama
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    // Rastgele şifre oluşturma
    public static string GenerateRandomPassword(int length = 12)
    {
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
        
        var charGroups = new[] { lowerChars, upperChars, numbers, specialChars };
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        
        rng.GetBytes(bytes);
        
        var result = new char[length];
        
        // En az bir karakter her gruptan olacak şekilde garanti et
        for (int i = 0; i < charGroups.Length; i++)
        {
            var group = charGroups[i];
            result[i] = group[bytes[i] % group.Length];
        }
        
        // Kalan karakterleri tüm gruplardan rastgele seç
        for (int i = charGroups.Length; i < length; i++)
        {
            var allChars = string.Join("", charGroups);
            result[i] = allChars[bytes[i] % allChars.Length];
        }
        
        // Karakterlerin sırasını karıştır
        for (int i = 0; i < length; i++)
        {
            int swapIndex = bytes[i] % length;
            (result[i], result[swapIndex]) = (result[swapIndex], result[i]);
        }
        
        return new string(result);
    }
}
```

### Şifre Sıfırlama

Kullanıcılar, şifrelerini unuttuklarında sıfırlama işlemi yapabilirler:

1. Kullanıcı, şifre sıfırlama isteği gönderir
2. Kullanıcının e-posta adresine sıfırlama kodu gönderilir
3. Kullanıcı, kod ile birlikte yeni şifresini belirler

```csharp
// AuthController.cs'den şifre sıfırlama örneği
[HttpPost("forgot-password")]
[EnableRateLimiting("api_auth_forgot-password")]
public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
{
    try
    {
        await _authService.ForgotPasswordAsync(request.Email);
        return Ok(ApiResponse<object>.Success(null, "Şifre sıfırlama talimatları e-posta adresinize gönderildi"));
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("bulunamadı"))
        {
            // Güvenlik nedeniyle aynı mesajı döndürüyoruz
            return Ok(ApiResponse<object>.Success(null, "Şifre sıfırlama talimatları e-posta adresinize gönderildi"));
        }
        return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
    }
}
```

## Rate Limiting

AuthApi, brute-force saldırılarına karşı koruma sağlamak için rate limiting mekanizması içerir. Bu özellik, belirli bir IP adresinden veya spesifik endpoint'lere yapılan istekleri sınırlar.

### Rate Limiting Yapılandırması

```json
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
      "Endpoint": "/api/Auth/register",
      "Period": "10m",
      "Limit": 3
    }
  ]
}
```

### Rate Limiting Uygulaması

```csharp
// Program.cs'den rate limiting örneği
builder.Services.AddRateLimitingServices(builder.Configuration);

// ...

// Rate limiting middleware'ini ekle
app.UseRateLimiter();

// Endpoint spesifik rate limit'leri uygula
app.MapControllers().RequireRateLimiting("ip");
``` 