# 07 - Hata Yönetimi

Bu bölüm, AuthApi projesindeki hata yönetimi stratejileri ve uygulamalarını açıklamaktadır.

## Hata Yönetimi Stratejisi

AuthApi, kullanıcılara tutarlı ve bilgilendirici hata mesajları sunmak için kapsamlı bir hata yönetimi stratejisi kullanır. Temel prensipler şunlardır:

1. **Tutarlı Hata Yanıtları**: Tüm API endpoint'leri için standart hata formatı
2. **Açıklayıcı Mesajlar**: Kullanıcı dostu hata mesajları
3. **Uygun HTTP Durum Kodları**: Her hata türü için doğru durum kodu
4. **Güvenlik Farkındalığı**: Hassas bilgilerin açığa çıkmaması
5. **Detaylı Loglama**: Hata ayıklama için kapsamlı loglar
6. **Graceful Degradation**: Hata durumunda bile minimum işlevsellik

## Standart Hata Yanıt Modeli

Tüm API hata yanıtları, tutarlı bir format kullanır:

```json
{
  "success": false,
  "message": "Kullanıcı dostu hata açıklaması",
  "statusCode": 400,
  "errors": {
    "propertyName": ["Bu alana özgü hata mesajı"],
    "anotherProperty": ["Başka bir alana özgü hata mesajı"]
  }
}
```

Bu model, `ApiResponse<T>` sınıfı kullanılarak uygulanır:

```csharp
/// <summary>
/// API yanıtları için standart model
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
    
    // Statik factory metotları
    public static ApiResponse<T> Success(T data, string message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message ?? "İşlem başarılı",
            StatusCode = statusCode,
            Data = data
        };
    }
    
    public static ApiResponse<T> Error(string message, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }
    
    public static ApiResponse<T> Error(Dictionary<string, List<string>> errors, string message = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message ?? "Bir veya daha fazla validasyon hatası oluştu",
            StatusCode = statusCode,
            Errors = errors
        };
    }
    
    // HTTP durum kodları için yardımcı metotlar
    public static ApiResponse<T> NotFound(string message = null)
    {
        return Error(message ?? "İstenen kaynak bulunamadı", 404);
    }
    
    public static ApiResponse<T> Unauthorized(string message = null)
    {
        return Error(message ?? "Bu işlem için yetkiniz yok", 401);
    }
    
    public static ApiResponse<T> Forbidden(string message = null)
    {
        return Error(message ?? "Bu işleme erişim reddedildi", 403);
    }
    
    public static ApiResponse<T> Conflict(string message = null)
    {
        return Error(message ?? "İstek, sunucu tarafında bir çakışma ile karşılaştı", 409);
    }
    
    public static ApiResponse<T> Created(T data, string message = null)
    {
        return Success(data, message ?? "Kaynak başarıyla oluşturuldu", 201);
    }
    
    public static ApiResponse<T> NoContent(string message = null)
    {
        return Success(default, message ?? "İşlem başarılı, içerik yok", 204);
    }
    
    public static ApiResponse<T> ServerError(string message = null)
    {
        return Error(message ?? "Sunucu hatası oluştu", 500);
    }
    
    public static ApiResponse<T> TooManyRequests(string message = null)
    {
        return Error(message ?? "Çok fazla istek yapıldı, lütfen daha sonra tekrar deneyin", 429);
    }
}
```

## Global Exception Handling

AuthApi, tüm işlenmemiş istisnaları yakalamak ve tutarlı yanıtlara dönüştürmek için global exception handling middleware kullanır:

```csharp
/// <summary>
/// Global hata yakalama middleware'i
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = CreateErrorResponse(exception);
        context.Response.StatusCode = response.StatusCode;
        
        await context.Response.WriteAsJsonAsync(response);
    }
    
    private ApiResponse<object> CreateErrorResponse(Exception exception)
    {
        // Exception tipine göre uygun yanıtı oluştur
        return exception switch
        {
            ValidationException validationEx => HandleValidationException(validationEx),
            UnauthorizedAccessException => ApiResponse<object>.Unauthorized(),
            NotFoundException notFoundEx => ApiResponse<object>.NotFound(notFoundEx.Message),
            ForbiddenException forbiddenEx => ApiResponse<object>.Forbidden(forbiddenEx.Message),
            ConflictException conflictEx => ApiResponse<object>.Conflict(conflictEx.Message),
            RateLimitExceededException rateLimitEx => ApiResponse<object>.TooManyRequests(rateLimitEx.Message),
            BadRequestException badRequestEx => ApiResponse<object>.Error(badRequestEx.Message, 400),
            _ => CreateDefaultErrorResponse(exception)
        };
    }
    
    private ApiResponse<object> HandleValidationException(ValidationException validationEx)
    {
        var errors = validationEx.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToList()
            );
            
        return ApiResponse<object>.Error(errors, "Validasyon hatası", 400);
    }
    
    private ApiResponse<object> CreateDefaultErrorResponse(Exception exception)
    {
        // Geliştirme ortamında daha detaylı hata bilgisi ver
        if (_environment.IsDevelopment())
        {
            return ApiResponse<object>.ServerError(exception.Message + Environment.NewLine + exception.StackTrace);
        }
        
        // Üretim ortamında genel bir hata mesajı ver
        return ApiResponse<object>.ServerError("Bir sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.");
    }
}
```

## Özel İstisna Sınıfları

AuthApi, farklı hata senaryoları için özel istisna sınıfları kullanır:

```csharp
/// <summary>
/// Bir kaynak bulunamadığında fırlatılır
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key) 
        : base($"{entityName} kaydı bulunamadı: {key}") { }
}

/// <summary>
/// Kullanıcının bir kaynağa erişim yetkisi olmadığında fırlatılır
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
    public ForbiddenException() 
        : base("Bu işlemi gerçekleştirmek için gerekli yetkiniz bulunmamaktadır.") { }
}

/// <summary>
/// İstek sırasında çakışma olduğunda fırlatılır (ör. unique constraint)
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// İstek geçersiz olduğunda fırlatılır
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

/// <summary>
/// Rate limit aşıldığında fırlatılır
/// </summary>
public class RateLimitExceededException : Exception
{
    public RateLimitExceededException(string message) : base(message) { }
    public RateLimitExceededException() 
        : base("Çok fazla istek yapıldı. Lütfen daha sonra tekrar deneyin.") { }
}
```

## Controller Seviyesinde Hata Yönetimi

Controller metotlarında, yaygın hata senaryoları için try-catch blokları kullanılır:

```csharp
// Kullanıcı girişi endpoint örneği
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
            
        if (ex.Message.Contains("Kullanıcı adı veya şifre"))
        {
            return StatusCode(401, ApiResponse<AuthResponse>.Unauthorized(ex.Message));
        }
        return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
    }
}
```

## Validasyon Yönetimi

AuthApi, giriş verilerini doğrulamak için FluentValidation kullanır:

```csharp
/// <summary>
/// Giriş isteği validasyonu
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
            .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır");
    }
}
```

Bu validator'ları kaydetmek ve API davranışını yapılandırmak için:

```csharp
// Program.cs'den validator kaydı
// Validatorları otomatik kaydet
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// API validation behavior'u özelleştir
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Özel model validasyon hatası yanıtı
    options.InvalidModelStateResponseFactory = context =>
    {
        // Hata sözlüğü oluşturma
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

        // Standart hata yanıtı oluşturma
        var response = ApiResponse<object>.Error(
            errors, 
            "Lütfen form alanlarını kontrol ediniz", 
            StatusCodes.Status400BadRequest
        );

        return new BadRequestObjectResult(response);
    };
});
```

## ValidationFilter

Model validasyonu sonuçlarını yakalamak için özel bir filtre:

```csharp
/// <summary>
/// Model validasyon hatalarını standart API yanıtlarına dönüştüren filtre
/// </summary>
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

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // İşlem gerekmez
    }
}
```

## Hata Loglama

Tüm hatalar kapsamlı bir şekilde loglanır:

```csharp
try
{
    // İşlem kodu...
}
catch (Exception ex)
{
    await _logService.LogErrorAsync(
        message: "İşlem başarısız", 
        source: "ServiceName.MethodName",
        exception: ex,
        userId: userId,
        userName: userName,
        data: new { RelevantData = value }
    );
    
    throw; // Hatayı yeniden fırlat
}
```

## Güvenlik Önlemleri

Hata yanıtlarında güvenlik konuları göz önünde bulundurulur:

1. **Hassas Veri Gizleme**: Hata mesajlarında hassas veriler görünmez (örn. şifreler)
2. **Üretim Ortamında Gizleme**: Üretim ortamında stack trace gibi teknik detaylar gizlenir
3. **Güvenlik Riski Azaltma**: Hata mesajları, hackerların faydalı bilgiler edinmesini önleyecek şekilde tasarlanmıştır
4. **Kullanıcı Dostu Bilgilendirme**: Kullanıcıya sadece bilmesi gereken bilgiler sunulur

## Örnek Hata Senaryoları ve Yanıtları

### 1. Validasyon Hatası

**İstek:**
```json
{
  "username": "a",
  "password": "123"
}
```

**Yanıt:**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "statusCode": 400,
  "errors": {
    "username": ["Kullanıcı adı 3-50 karakter arasında olmalıdır"],
    "password": ["Şifre en az 6 karakter olmalıdır"]
  }
}
```

### 2. Kaynak Bulunamadı

**İstek:**
```
GET /api/Users/9999
```

**Yanıt:**
```json
{
  "success": false,
  "message": "ID: 9999 olan kullanıcı bulunamadı",
  "statusCode": 404,
  "errors": null
}
```

### 3. Yetkilendirme Hatası

**İstek:**
```
GET /api/Users
```

**Yanıt:**
```json
{
  "success": false,
  "message": "Bu işlem için yetkiniz yok",
  "statusCode": 401,
  "errors": null
}
```

### 4. Çakışma Hatası

**İstek:**
```json
{
  "username": "admin",
  "email": "new.user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Yanıt:**
```json
{
  "success": false,
  "message": "Bu kullanıcı adı zaten kullanılıyor",
  "statusCode": 409,
  "errors": null
}
```

### 5. Rate Limit Aşıldı

**Birçok istek sonrası yanıt:**
```json
{
  "success": false,
  "message": "Çok fazla istek yapıldı. Lütfen daha sonra tekrar deneyin.",
  "statusCode": 429,
  "errors": null
}
```

