# AuthApi Uygulama Kılavuzu

## Request/Response Yapısı

### Yanıt Formatı

Sistemimiz, tutarlı bir API deneyimi sağlamak için standart bir yanıt formatı kullanmaktadır:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public object Errors { get; set; }
    public int StatusCode { get; set; }

    // Başarılı yanıt oluşturan statik factory metodlar
    public static ApiResponse<T> Success(T data, string message = "İşlem başarılı", int statusCode = 200)
    {
        return new ApiResponse<T> 
        { 
            Success = true, 
            Message = message, 
            Data = data, 
            StatusCode = statusCode 
        };
    }

    // Created (201) yanıt oluşturan factory metod
    public static ApiResponse<T> Created(T data, string message = "Kayıt başarıyla oluşturuldu")
    {
        return Success(data, message, 201);
    }

    // Hata yanıtları oluşturan factory metodlar
    public static ApiResponse<T> Error(string message = "İşlem başarısız", int statusCode = 400)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Message = message, 
            StatusCode = statusCode 
        };
    }

    // Validasyon hatası yanıtı
    public static ApiResponse<T> Error(object errors, string message = "Validasyon hatası", int statusCode = 400)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Message = message, 
            Errors = errors, 
            StatusCode = statusCode 
        };
    }

    // HTTP durum kodlarına göre özel hata yanıtları
    public static ApiResponse<T> Unauthorized(string message = "Yetkisiz erişim")
    {
        return Error(message, 401);
    }

    public static ApiResponse<T> Forbidden(string message = "Bu işlem için yetkiniz yok")
    {
        return Error(message, 403);
    }

    public static ApiResponse<T> NotFound(string message = "Kayıt bulunamadı")
    {
        return Error(message, 404);
    }

    public static ApiResponse<T> Conflict(string message = "İstek mevcut durumla çakışıyor")
    {
        return Error(message, 409);
    }

    public static ApiResponse<T> ServerError(string message = "Sunucu hatası")
    {
        return Error(message, 500);
    }
}
```

Bu yapı sayesinde, tüm API yanıtları tutarlı bir format izler ve frontend geliştiriciler için öngörülebilir bir arayüz sağlar.

### İstek DTO Modelleri

Her endpoint için özel DTO (Data Transfer Object) sınıfları kullanılmaktadır:

```csharp
// Örnek DTO sınıfları
public class LoginRequest
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur")]
    public string Password { get; set; }
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [MinLength(3, ErrorMessage = "Kullanıcı adı en az 3 karakter olmalıdır")]
    public string Username { get; set; }

    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
     ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Ad soyad zorunludur")]
    public string FullName { get; set; }
}
```

## Validasyon Mekanizması

Sistemimiz iki seviyeli validasyon kullanmaktadır:

### 1. Model Bağlama Validasyonu (Data Annotation)

DTO sınıflarımızda `System.ComponentModel.DataAnnotations` kütüphanesinden attribute'lar kullanılarak gelen verilerin temel doğrulaması yapılır.

### 2. Özel Validasyon Filtresi

Program.cs dosyasında tanımlanan `ValidationFilter` sınıfı, controller'lara gelen tüm isteklerde model durumunu kontrol eder:

```csharp
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

            var response = ApiResponse<object>.Error(
                errors, 
                "Lütfen form alanlarını kontrol ediniz", 
                StatusCodes.Status400BadRequest
            );

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

Bu filter, controller'a ulaşmadan önce gerçekleşen model validasyon hatalarını yakalar ve standart API yanıt formatında döndürür.

## Exception Handling

Sistem içinde exception'lar aşağıdaki şekilde ele alınır:

### Controller Seviyesinde Try-Catch Blokları

Her controller metodu, olası hataları yakalamak ve uygun HTTP durum kodlarıyla yanıt vermek için try-catch blokları kullanır:

```csharp
[HttpPost("login")]
[EnableRateLimiting("api_auth_login")]
public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request)
{
    await _logService.LogInfoAsync(
        message: "Giriş işlemi başlatıldı", 
        source: "AuthController.Login",
        data: new { Username = request.Username });
    
    try
    {
        var result = await _authService.LoginAsync(request);
        
        // 2FA gerekiyorsa farklı bir yanıt döndür
        if (result is TwoFactorRequiredResponse twoFactorResponse)
        {
            await _logService.LogInfoAsync(
                message: "İki faktörlü kimlik doğrulama gerekli", 
                source: "AuthController.Login",
                data: new { Username = request.Username },
                userId: twoFactorResponse.UserId.ToString(),
                userName: request.Username);
                
            return Ok(ApiResponse<TwoFactorRequiredResponse>.Success(twoFactorResponse, "İki faktörlü kimlik doğrulama gerekli", 200));
        }
        
        // Normal giriş yanıtı
        var authResponse = (AuthResponse)result;
        
        await _logService.LogInfoAsync(
            message: "Giriş başarılı", 
            source: "AuthController.Login",
            data: new { Username = request.Username, TokenExpiration = authResponse.AccessToken.ExpiresAt },
            userId: authResponse.User.Id.ToString(),
            userName: authResponse.User.Username);
            
        return Ok(ApiResponse<AuthResponse>.Success(authResponse, "Giriş başarılı"));
    }
    catch (Exception ex)
    {
        await _logService.LogErrorAsync(
            message: "Giriş işlemi başarısız", 
            source: "AuthController.Login",
            exception: ex,
            userName: request.Username);
            
        // Hata mesajına göre HTTP durum kodu belirlenir
        if (ex.Message.Contains("Kullanıcı adı veya şifre"))
        {
            return StatusCode(401, ApiResponse<AuthResponse>.Unauthorized(ex.Message));
        }
        return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
    }
}
```

Her exception türü için hata mesajlarına bakılarak en uygun HTTP durum kodu ve yanıt formatı belirlenir.

### Özel Exception Sınıfları

Sistemde farklı hata durumlarını temsil eden özel exception sınıfları kullanılır:

```csharp
// Örnek özel exception sınıfları
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
```

## Rate Limiting

Appsettings.json'da tanımlanan rate limit konfigürasyonları, belirli endpointlerin aşırı kullanımını engeller:

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
    // Diğer endpoint kısıtlamaları...
  ]
}
```

Rate limiting'in uygulanması, `Program.cs` içindeki konfigürasyon ile gerçekleştirilir.

## JWT Kimlik Doğrulama

JWT token'ları, kullanıcı kimlik doğrulaması için kullanılır:

```csharp
// JWT yapılandırması örneği
public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret");
        var key = Encoding.ASCII.GetBytes(secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                ValidAudience = jwtSettings.GetValue<string>("Audience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
```

## Loglama

Sistem, yapılandırılabilir bir loglama mekanizması kullanmaktadır:

```csharp
// Loglama örneği (Controller'dan)
await _logService.LogInfoAsync(
    message: "Giriş başarılı", 
    source: "AuthController.Login",
    data: new { Username = request.Username, TokenExpiration = authResponse.AccessToken.ExpiresAt },
    userId: authResponse.User.Id.ToString(),
    userName: authResponse.User.Username);
```

LogLibrary, hem konsola hem de MongoDB veritabanına loglama yapabilir ve yapılandırılabilir özelliklere sahiptir.

## İstek/Yanıt Örnekleri

### Giriş İsteği ve Yanıtı

**İstek (POST /api/Auth/login):**
```json
{
  "username": "johndoe",
  "password": "StrongP@ss123"
}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "Giriş başarılı",
  "data": {
    "user": {
      "id": 1,
      "username": "johndoe",
      "email": "john@example.com",
      "fullName": "John Doe",
      "profilePictureUrl": "https://example.com/profiles/johndoe.jpg",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2023-04-30T22:00:00Z"
    },
    "refreshToken": "6c9c0e7a-1c1a-4b9c-9e9a-1e3d8c2f6e8a"
  },
  "statusCode": 200
}
```

**2FA Gerekli Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "İki faktörlü kimlik doğrulama gerekli",
  "data": {
    "userId": 1,
    "twoFactorType": "Email"
  },
  "statusCode": 200
}
```

**Başarısız Yanıt (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Kullanıcı adı veya şifre hatalı",
  "statusCode": 401
}
```

### Kullanıcı Kaydı İsteği ve Yanıtları

**İstek (POST /api/Auth/register):**
```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "StrongP@ss123",
  "confirmPassword": "StrongP@ss123",
  "fullName": "New User"
}
```

**Başarılı Yanıt (201 Created):**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla kaydedildi",
  "data": {
    "user": {
      "id": 2,
      "username": "newuser",
      "email": "newuser@example.com",
      "fullName": "New User",
      "profilePictureUrl": null,
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2023-04-30T22:00:00Z"
    },
    "refreshToken": "8a7b6c5d-4e3f-2a1b-0c9d-8e7f6a5b4c3d"
  },
  "statusCode": 201
}
```

**Kullanıcı Adı Çakışması Hatası (409 Conflict):**
```json
{
  "success": false,
  "message": "Bu kullanıcı adı zaten kullanılıyor",
  "statusCode": 409
}
```

**E-posta Çakışması Hatası (409 Conflict):**
```json
{
  "success": false,
  "message": "Bu e-posta adresi zaten kullanılıyor",
  "statusCode": 409
}
```

**Şifre Eşleşmeme Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "confirmPassword": ["Şifreler eşleşmiyor"]
  },
  "statusCode": 400
}
```

**Şifre Politikası Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "password": ["Şifre en az 8 karakter olmalıdır", 
                "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir"]
  },
  "statusCode": 400
}
```

**Geçersiz E-posta Formatı (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "email": ["Geçerli bir e-posta adresi giriniz"]
  },
  "statusCode": 400
}
```

**Birden Fazla Validasyon Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "username": ["Kullanıcı adı en az 3 karakter olmalıdır"],
    "email": ["Geçerli bir e-posta adresi giriniz"],
    "fullName": ["Ad soyad zorunludur"]
  },
  "statusCode": 400
}
```

### Kullanıcı Profili Güncelleme

**İstek (PUT /api/Users/profile):**
```json
{
  "fullName": "John Smith Doe",
  "email": "john.updated@example.com",
  "phoneNumber": "+905551234567"
}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "Profil başarıyla güncellendi",
  "data": {
    "id": 1,
    "username": "johndoe",
    "email": "john.updated@example.com",
    "fullName": "John Smith Doe",
    "phoneNumber": "+905551234567",
    "profilePictureUrl": "https://example.com/profiles/johndoe.jpg",
    "roles": ["User"]
  },
  "statusCode": 200
}
```

**E-posta Çakışması Hatası (409 Conflict):**
```json
{
  "success": false,
  "message": "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor",
  "statusCode": 409
}
```

**Geçersiz Telefon Numarası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "phoneNumber": ["Geçerli bir telefon numarası giriniz (örn: +905551234567)"]
  },
  "statusCode": 400
}
```

### Şifre Değiştirme

**İstek (POST /api/Auth/change-password):**
```json
{
  "currentPassword": "OldP@ssword123",
  "newPassword": "NewStrongP@ss456",
  "confirmPassword": "NewStrongP@ss456"
}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "Şifreniz başarıyla değiştirildi",
  "statusCode": 200
}
```

**Mevcut Şifre Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Mevcut şifre hatalı",
  "statusCode": 400
}
```

**Yeni Şifre Politikası Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "newPassword": ["Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir"]
  },
  "statusCode": 400
}
```

### Profil Resmi Yükleme

**İstek (POST /api/Users/profile-picture):**
Form verisi olarak resim dosyası

**Başarılı Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "Profil resmi güncellendi",
  "data": {
    "profilePictureUrl": "https://example.com/profiles/johndoe_20230430.jpg"
  },
  "statusCode": 200
}
```

**Geçersiz Dosya Formatı Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Desteklenmeyen dosya formatı. Lütfen JPG, PNG veya GIF formatında resim yükleyin",
  "statusCode": 400
}
```

**Dosya Boyutu Aşımı Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Dosya boyutu çok büyük. Maksimum 5MB boyutunda dosya yükleyebilirsiniz",
  "statusCode": 400
}
```

### İki Faktörlü Kimlik Doğrulama Ayarı

**İstek (POST /api/Auth/setup-2fa):**
```json
{
  "enabled": true,
  "type": "Email",
  "currentPassword": "StrongP@ss123"
}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "success": true,
  "message": "2FA ayarları güncellendi",
  "data": {
    "isEnabled": true,
    "type": "Email",
    "isRequired": false
  },
  "statusCode": 200
}
```

**Şifre Doğrulama Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Mevcut şifre hatalı",
  "statusCode": 400
}
```

**Geçersiz 2FA Tipi Hatası (400 Bad Request):**
```json
{
  "success": false,
  "message": "Desteklenmeyen iki faktörlü kimlik doğrulama tipi",
  "statusCode": 400
}
```

### Şifre Sıfırlama Talebi

**İstek (POST /api/Auth/forgot-password):**
```json
{
  "email": "john@example.com"
}
```

**Başarılı Yanıt (200 OK, her durumda):**
```json
{
  "success": true,
  "message": "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi",
  "statusCode": 200
}
```

Güvenlik nedeniyle, e-posta sistemde kayıtlı olmasa bile aynı yanıt döndürülür. Backend tarafında kayıtlı olmayan e-postalar için işlem yapılmaz, ancak yanıt her zaman başarılı görünür.

### Validasyon Hatası Yanıtı

**Başarısız Yanıt (400 Bad Request):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "username": ["Kullanıcı adı zorunludur"],
    "password": ["Şifre en az 8 karakter olmalıdır", "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir"]
  },
  "statusCode": 400
}
```

## Summary

Bu belgede, AuthApi projesinde kullanılan:
- Standart API yanıt formatı
- Validasyon mekanizması
- Exception handling yaklaşımı
- Rate limiting yapılandırması
- JWT kimlik doğrulama
- Loglama stratejisi

üzerine bilgiler verilmiştir. Bu yapılar, tüm API genelinde tutarlı davranış ve güvenilir sonuçlar sağlamak için kullanılmaktadır. 