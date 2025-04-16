# 04 - Loglama Sistemi

AuthApi'nin en önemli özelliklerinden biri, kapsamlı ve yapılandırılabilir loglama sistemidir. Bu bölüm, projenin loglama altyapısını detaylı olarak açıklamaktadır.

## Loglama Mimarisi

AuthApi, farklı loglama ihtiyaçlarını karşılamak için çok katmanlı bir loglama mimarisi kullanır:

1. **MongoDB Loglama**: Yapılandırılmış loglar için ana depolama alanı
2. **Graylog Entegrasyonu**: Merkezi log yönetimi ve analizi için
3. **Serilog**: Dosya ve konsol loglama için

### Loglama Katmanları

![Loglama Mimarisi](https://i.imgur.com/B0Naxjb.png)

## Loglama Yapılandırması

Loglama ayarları `appsettings.json` dosyasında tanımlanır:

```json
"LogSettings": {
  "EnableGraylog": true,
  "GraylogHost": "localhost",
  "GraylogPort": 12201,
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "BilgiLED",
  "CollectionName": "authLogs",
  "ApplicationName": "AuthApi",
  "Environment": "Development",
  "EnableAsyncLogging": true,
  "EnableHttpLogging": true,
  "MaskSensitiveData": true
}
```

## Log Türleri

AuthApi iki ana log türü kullanır:

1. **ApiLog**: Sistem olayları, işlemler ve hatalar için genel loglama
2. **RequestResponseLog**: HTTP istekleri ve yanıtları için ayrıntılı loglama

### ApiLog Örneği

```json
{
  "id": "6079f3e5b9d2a41234c98762",
  "timestamp": "2025-04-16T14:25:17.123Z",
  "level": "Information",
  "message": "Kullanıcı başarıyla kaydedildi",
  "source": "AuthController.Register",
  "userId": "42",
  "userName": "johndoe",
  "userEmail": "john.doe@example.com",
  "data": {
    "username": "johndoe",
    "email": "john.doe@example.com"
  },
  "applicationName": "AuthApi",
  "environment": "Development",
  "correlationId": "7b5e8d3f-1c2a-4b6d-9f8e-5d4c3b2a1098"
}
```

### RequestResponseLog Örneği

```json
{
  "id": "6079f3e5b9d2a41234c98763",
  "timestamp": "2025-04-16T14:25:18.456Z",
  "requestMethod": "POST",
  "requestPath": "/api/Auth/login",
  "requestHeaders": {
    "Content-Type": "application/json",
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ..."
  },
  "requestBody": {
    "username": "johndoe",
    "password": "********"
  },
  "responseStatusCode": 200,
  "responseHeaders": {
    "Content-Type": "application/json; charset=utf-8"
  },
  "responseBody": {
    "success": true,
    "message": "Giriş başarılı",
    "statusCode": 200,
    "data": {
      "user": {
        "id": 42,
        "username": "johndoe",
        "email": "john.doe@example.com",
        "roles": ["User"]
      },
      "accessToken": {
        "token": "eyJhbGciOiJIUzI1...",
        "expiresAt": "2025-04-17T02:25:18.456Z"
      },
      "refreshToken": {
        "token": "********",
        "expiresAt": "2025-04-23T14:25:18.456Z"
      }
    }
  },
  "duration": 123,
  "clientIp": "192.168.1.100",
  "applicationName": "AuthApi",
  "environment": "Development",
  "correlationId": "7b5e8d3f-1c2a-4b6d-9f8e-5d4c3b2a1098"
}
```

## LogService Uygulaması

Loglama işlemleri, `LogService` sınıfı tarafından yönetilir:

```csharp
public class LogService : ILogService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<LogService> _logger;
    private readonly LogSettings _logSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    private readonly IMongoCollection<ApiLog> _apiLogCollection;
    private readonly IMongoCollection<RequestResponseLog> _requestLogCollection;
    
    public LogService(
        IOptions<LogSettings> logSettings,
        IMongoClient mongoClient,
        ILogger<LogService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logSettings = logSettings.Value;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        
        _database = mongoClient.GetDatabase(_logSettings.DatabaseName);
        _apiLogCollection = _database.GetCollection<ApiLog>(_logSettings.ApiLogsCollectionName);
        _requestLogCollection = _database.GetCollection<RequestResponseLog>(_logSettings.RequestLogsCollectionName);
        
        // MongoDB TTL indeksi oluşturma (otomatik log silme için)
        var indexKeysDefinition = Builders<ApiLog>.IndexKeys.Ascending(x => x.Timestamp);
        var indexOptions = new CreateIndexOptions { 
            ExpireAfter = TimeSpan.FromDays(_logSettings.LogRetentionDays) 
        };
        _apiLogCollection.Indexes.CreateOne(new CreateIndexModel<ApiLog>(indexKeysDefinition, indexOptions));
        
        // Request logları için de TTL indeksi oluştur
        var requestIndexKeysDefinition = Builders<RequestResponseLog>.IndexKeys.Ascending(x => x.Timestamp);
        _requestLogCollection.Indexes.CreateOne(new CreateIndexModel<RequestResponseLog>(requestIndexKeysDefinition, indexOptions));
    }
    
    // Bilgi seviyesi log kaydı
    public async Task LogInfoAsync(
        string message, 
        string source = null, 
        object data = null, 
        string userId = null, 
        string userName = null, 
        string userEmail = null)
    {
        await LogAsync(LogLevel.Information, message, source, data, null, userId, userName, userEmail);
    }
    
    // Uyarı seviyesi log kaydı
    public async Task LogWarningAsync(
        string message, 
        string source = null, 
        object data = null, 
        Exception exception = null, 
        string userId = null, 
        string userName = null, 
        string userEmail = null)
    {
        await LogAsync(LogLevel.Warning, message, source, data, exception, userId, userName, userEmail);
    }
    
    // Hata seviyesi log kaydı
    public async Task LogErrorAsync(
        string message, 
        string source = null, 
        object data = null, 
        Exception exception = null, 
        string userId = null, 
        string userName = null, 
        string userEmail = null)
    {
        await LogAsync(LogLevel.Error, message, source, data, exception, userId, userName, userEmail);
    }
    
    // Genel log işlemi
    private async Task LogAsync(
        LogLevel level, 
        string message, 
        string source = null, 
        object data = null, 
        Exception exception = null, 
        string userId = null, 
        string userName = null, 
        string userEmail = null)
    {
        // API Log nesnesi oluştur
        var apiLog = new ApiLog
        {
            Timestamp = DateTime.UtcNow,
            Level = level.ToString(),
            Message = message,
            Source = source,
            Data = data != null ? MaskSensitiveData(data) : null,
            Exception = exception != null ? new ExceptionInfo
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message
            } : null,
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail,
            CorrelationId = GetCorrelationId(),
            ApplicationName = _logSettings.ApplicationName,
            Environment = _logSettings.Environment
        };
        
        // .NET logger API'sine de ilet
        switch (level)
        {
            case LogLevel.Information:
                _logger.LogInformation("{Message} {Data}", message, data);
                break;
            case LogLevel.Warning:
                _logger.LogWarning(exception, "{Message} {Data}", message, data);
                break;
            case LogLevel.Error:
                _logger.LogError(exception, "{Message} {Data}", message, data);
                break;
        }
        
        // Asenkron loglama etkinse
        if (_logSettings.EnableAsyncLogging)
        {
            // Fire & forget şeklinde log gönder
            _ = Task.Run(async () => 
            {
                try
                {
                    await _apiLogCollection.InsertOneAsync(apiLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MongoDB'ye log kaydedilirken hata oluştu");
                }
            });
        }
        else
        {
            // Senkron olarak log gönder
            try
            {
                await _apiLogCollection.InsertOneAsync(apiLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongoDB'ye log kaydedilirken hata oluştu");
            }
        }
    }
    
    // HTTP istek/yanıt logları
    public async Task LogHttpAsync(HttpContext context, double durationMs)
    {
        if (!_logSettings.EnableHttpLogging)
            return;
        
        // İstek verilerini al
        var request = context.Request;
        var response = context.Response;
        
        // İstek gövdesini oku (önceden middleware tarafından eklenir)
        var requestBody = string.Empty;
        if (context.Items.TryGetValue("RequestBody", out var requestBodyObj))
        {
            requestBody = requestBodyObj as string;
        }
        
        // Yanıt gövdesini oku (önceden middleware tarafından eklenir)
        var responseBody = string.Empty;
        if (context.Items.TryGetValue("ResponseBody", out var responseBodyObj))
        {
            responseBody = responseBodyObj as string;
        }
        
        // RequestResponse log nesnesi oluştur
        var requestLog = new RequestResponseLog
        {
            Timestamp = DateTime.UtcNow,
            RequestMethod = request.Method,
            RequestPath = request.Path,
            RequestHeaders = GetHeaders(request.Headers),
            RequestBody = !string.IsNullOrEmpty(requestBody) ? MaskSensitiveData(requestBody) : null,
            ResponseStatusCode = response.StatusCode,
            ResponseHeaders = GetHeaders(response.Headers),
            ResponseBody = !string.IsNullOrEmpty(responseBody) ? MaskSensitiveData(responseBody) : null,
            Duration = durationMs,
            ClientIp = GetClientIp(context),
            CorrelationId = GetCorrelationId(),
            ApplicationName = _logSettings.ApplicationName,
            Environment = _logSettings.Environment
        };
        
        // Asenkron loglama etkinse
        if (_logSettings.EnableAsyncLogging)
        {
            // Fire & forget şeklinde log gönder
            _ = Task.Run(async () => 
            {
                try
                {
                    await _requestLogCollection.InsertOneAsync(requestLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MongoDB'ye HTTP log kaydedilirken hata oluştu");
                }
            });
        }
        else
        {
            // Senkron olarak log gönder
            try
            {
                await _requestLogCollection.InsertOneAsync(requestLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongoDB'ye HTTP log kaydedilirken hata oluştu");
            }
        }
    }
    
    // Hassas verileri maskele
    private object MaskSensitiveData(object data)
    {
        if (!_logSettings.MaskSensitiveData || data == null)
            return data;
        
        if (data is string stringData)
        {
            // JSON içerik ise
            if (stringData.StartsWith("{") || stringData.StartsWith("["))
            {
                try
                {
                    var json = JsonDocument.Parse(stringData);
                    var maskedJson = MaskSensitiveJsonProperties(json.RootElement);
                    return JsonSerializer.Serialize(maskedJson);
                }
                catch
                {
                    // JSON parse hatası, orijinal string'i döndür
                    return stringData;
                }
            }
            return stringData;
        }
        
        // Diğer nesneler için objeyi JSON'a çevir ve maskele
        var jsonString = JsonSerializer.Serialize(data);
        try
        {
            var json = JsonDocument.Parse(jsonString);
            var maskedJson = MaskSensitiveJsonProperties(json.RootElement);
            return maskedJson;
        }
        catch
        {
            // JSON parse hatası, orijinal veriyi döndür
            return data;
        }
    }
    
    // Hassas JSON özelliklerini maskele
    private object MaskSensitiveJsonProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var result = new Dictionary<string, object>();
            
            foreach (var property in element.EnumerateObject())
            {
                var propertyName = property.Name.ToLowerInvariant();
                
                // Hassas olabilecek alanları kontrol et
                if (propertyName.Contains("password") || 
                    propertyName.Contains("secret") || 
                    propertyName.Contains("token") || 
                    propertyName.Contains("key") ||
                    propertyName.Contains("auth") ||
                    propertyName.Contains("credential"))
                {
                    result[property.Name] = "********";
                }
                else if (property.Value.ValueKind == JsonValueKind.Object || 
                         property.Value.ValueKind == JsonValueKind.Array)
                {
                    result[property.Name] = MaskSensitiveJsonProperties(property.Value);
                }
                else
                {
                    result[property.Name] = GetJsonElementValue(property.Value);
                }
            }
            
            return result;
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            var result = new List<object>();
            
            foreach (var item in element.EnumerateArray())
            {
                result.Add(MaskSensitiveJsonProperties(item));
            }
            
            return result;
        }
        else
        {
            return GetJsonElementValue(element);
        }
    }
    
    // Korelasyon ID'sini al
    private string GetCorrelationId()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            // Eğer istek header'ında varsa, o ID'yi kullan
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                return correlationId.ToString();
            }
            
            // HttpContext trace identifier'ı kullan
            return _httpContextAccessor.HttpContext.TraceIdentifier;
        }
        
        // HTTP bağlamı yoksa yeni bir GUID oluştur
        return Guid.NewGuid().ToString();
    }
}
```

## HTTP İstek/Yanıt Loglama Middleware

HTTP isteklerini ve yanıtlarını loglamak için özel bir middleware kullanılır:

```csharp
public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogService _logService;
    private readonly LogSettings _logSettings;
    
    public HttpLoggingMiddleware(
        RequestDelegate next, 
        ILogService logService,
        IOptions<LogSettings> logSettings)
    {
        _next = next;
        _logService = logService;
        _logSettings = logSettings.Value;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Loglama devre dışı ise veya loglama dışı tutulan bir path ise
        if (!_logSettings.EnableHttpLogging || IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }
        
        // İstek gövdesini oku
        var originalRequestBody = context.Request.Body;
        using var requestBodyStream = new MemoryStream();
        
        if (context.Request.ContentLength > 0)
        {
            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Position = 0;
            
            using var streamReader = new StreamReader(requestBodyStream);
            var requestBody = await streamReader.ReadToEndAsync();
            context.Items["RequestBody"] = requestBody;
            
            requestBodyStream.Position = 0;
            context.Request.Body = requestBodyStream;
        }
        
        // Yanıt gövdesini yakalamak için orijinal response body'yi değiştir
        var originalResponseBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        
        // Zamanı ölç
        var startTime = DateTime.UtcNow;
        
        try
        {
            await _next(context);
            
            // Yanıt gövdesini oku
            responseBodyStream.Position = 0;
            using var streamReader = new StreamReader(responseBodyStream);
            var responseBody = await streamReader.ReadToEndAsync();
            context.Items["ResponseBody"] = responseBody;
            
            // Yanıtı orijinal stream'e kopyala
            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalResponseBody);
            
            // Log oluştur
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            await _logService.LogHttpAsync(context, duration);
        }
        finally
        {
            // Orijinal stream'leri geri yükle
            context.Request.Body = originalRequestBody;
            context.Response.Body = originalResponseBody;
        }
    }
    
    // Belirli path'leri loglama dışında tutma
    private bool IsExcludedPath(string path)
    {
        if (_logSettings.ExcludedPaths == null || _logSettings.ExcludedPaths.Count == 0)
            return false;
            
        return _logSettings.ExcludedPaths.Any(excludedPath => 
            path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));
    }
}
```

## Loglama Entegrasyonu

Loglama sistemi, Program.cs dosyasında yapılandırılır:

```csharp
// Program.cs'den loglama konfigürasyonu
builder.Services.AddLogLibrary(builder.Configuration);

// LogLibrary servislerini kaydet
public static IServiceCollection AddLogLibrary(this IServiceCollection services, IConfiguration configuration)
{
    // Log ayarlarını yapılandır
    services.Configure<LogSettings>(configuration.GetSection("LogSettings"));
    var logSettings = configuration.GetSection("LogSettings").Get<LogSettings>();
    
    // MongoDB bağlantısı ekle
    services.AddSingleton<IMongoClient>(sp => 
        new MongoClient(logSettings.ConnectionString));
    
    // Log servisini ekle
    services.AddSingleton<ILogService, LogService>();
    
    // Graylog entegrasyonu (etkinse)
    if (logSettings.EnableGraylog)
    {
        // Serilog yapılandırması ile Graylog sink'i ekle
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", logSettings.ApplicationName)
            .Enrich.WithProperty("Environment", logSettings.Environment)
            .WriteTo.Console()
            .WriteTo.Graylog(new GraylogSinkOptions
            {
                HostnameOrAddress = logSettings.GraylogHost,
                Port = logSettings.GraylogPort,
                TransportType = TransportType.Udp
            })
            .CreateLogger();
            
        // Serilog provider'ını ekle
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });
    }
    
    // HTTP loglama middleware'i için HttpContextAccessor ekle
    services.AddHttpContextAccessor();
    
    return services;
}
```

## Log Sorguları

MongoDB'de saklanan loglar, çeşitli sorgu kriterleri ile sorgulanabilir:

```csharp
// Belirli bir kullanıcının işlemlerini sorgulama
var userLogs = await _apiLogCollection
    .Find(log => log.UserId == "42")
    .SortByDescending(log => log.Timestamp)
    .Limit(100)
    .ToListAsync();

// Belirli bir zaman aralığındaki hataları sorgulama
var startDate = DateTime.UtcNow.AddDays(-1);
var endDate = DateTime.UtcNow;

var errorLogs = await _apiLogCollection
    .Find(log => 
        log.Level == "Error" && 
        log.Timestamp >= startDate && 
        log.Timestamp <= endDate)
    .SortByDescending(log => log.Timestamp)
    .ToListAsync();

// Belirli bir endpoint'e yapılan istekleri sorgulama
var loginAttempts = await _requestLogCollection
    .Find(log => 
        log.RequestPath == "/api/Auth/login" && 
        log.RequestMethod == "POST")
    .SortByDescending(log => log.Timestamp)
    .Limit(50)
    .ToListAsync();
```

## Log Döngüsü ve Yaşam Süresi

Loglar, belirtilen süre sonunda otomatik olarak silinir:

```json
"LogSettings": {
  "LogRetentionDays": 90
}
```

