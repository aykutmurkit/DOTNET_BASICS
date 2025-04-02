# Loglama Sistemi

Bu bölüm, Deneme API'nin loglama sistemi hakkında bilgiler içerir.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Log Tipleri](#log-tipleri)
- [MongoDB Entegrasyonu](#mongodb-entegrasyonu)
- [Middleware Yapısı](#middleware-yapısı)
- [Yapılandırma](#yapılandırma)
- [Güvenlik Önlemleri](#güvenlik-önlemleri)
- [Extension Metotları](#extension-metotları)
- [Best Practices](#best-practices)

## Genel Bakış

Deneme API, kullanıcı aktivitelerini ve API işlemlerini kapsamlı bir şekilde izlemek için MongoDB tabanlı bir loglama sistemi kullanır. Bu sistem, gelen istekleri, yanıtları ve sistem olaylarını kaydeder ve analiz amacıyla saklar.

Loglama sistemi şu özellikleri sunar:

- İstek/yanıt loglaması
- API olay loglaması (bilgi, uyarı, hata)
- Hassas verilerin otomatik gizlenmesi
- Belirli endpoint'leri loglama dışında bırakma
- Loglar için TTL (Time-To-Live) ile otomatik silme

## Log Tipleri

### RequestResponseLog

İstek ve yanıt detaylarını içeren log tipidir.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | ObjectId | Benzersiz log ID'si |
| TraceId | string | İsteği tanımlamak için benzersiz iz ID'si |
| Path | string | İstek yapılan endpoint path'i |
| HttpMethod | string | HTTP metodu (GET, POST, vb.) |
| QueryString | string | URL sorgu parametreleri |
| RequestBody | string | İstek gövdesi (hassas veriler gizlenir) |
| ResponseBody | string | Yanıt gövdesi (hassas veriler gizlenir) |
| StatusCode | int | HTTP durum kodu |
| UserId | string | İsteği yapan kullanıcı ID'si |
| Username | string | İsteği yapan kullanıcı adı |
| UserIp | string | İsteği yapan kullanıcının IP adresi |
| RequestSize | long | İstek boyutu (byte) |
| ResponseSize | long | Yanıt boyutu (byte) |
| ExecutionTime | long | İsteğin işlenme süresi (ms) |
| Timestamp | DateTime | Log oluşturulma zamanı (UTC) |

### ApiLog

Genel API olaylarını içeren log tipidir.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | ObjectId | Benzersiz log ID'si |
| TraceId | string | İlgili iz ID'si |
| Level | string | Log seviyesi (Info, Warning, Error) |
| Message | string | Log mesajı |
| Exception | string | Hata detayı (varsa) |
| UserId | string | İlgili kullanıcı ID'si |
| Username | string | İlgili kullanıcı adı |
| Path | string | İlgili endpoint path'i |
| Timestamp | DateTime | Log oluşturulma zamanı (UTC) |

## MongoDB Entegrasyonu

Loglama sistemi, verileri saklamak için MongoDB kullanır. Sistem, aşağıdaki koleksiyonları oluşturur:

- **RequestResponseLogs**: İstek/yanıt logları
- **ApiLogs**: API olay logları

Her iki koleksiyon da, `Timestamp` alanı üzerinde TTL (Time-To-Live) indeksine sahiptir. Bu sayede loglar, belirli bir süre sonra otomatik olarak silinir.

## Middleware Yapısı

Loglama sistemi, `RequestResponseLoggingMiddleware` adlı özel bir middleware kullanarak HTTP istek ve yanıtlarını yakalar ve loglar. Bu middleware, ASP.NET Core pipeline'ında stratejik bir konumda çalışır.

### RequestResponseLoggingMiddleware

Bu middleware, gelen her HTTP isteğini ve giden yanıtı yakalayarak MongoDB'ye kaydeder. Middleware şu şekilde çalışır:

```csharp
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly string[] _excludedPaths;
    private readonly ILogRepository _logRepository;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IConfiguration configuration,
        ILogRepository logRepository)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        _excludedPaths = configuration.GetSection("LogSettings:ExcludedPaths").Get<string[]>() ?? Array.Empty<string>();
        _logRepository = logRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Bu endpoint için loglama atlanmalı mı kontrol et
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Yeni log nesnesi oluştur
        var log = new RequestResponseLog
        {
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path,
            HttpMethod = context.Request.Method,
            QueryString = context.Request.QueryString.ToString(),
            UserIp = context.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        // Kimlik bilgilerini ekle (eğer kullanıcı kimliği doğrulanmışsa)
        if (context.User.Identity?.IsAuthenticated == true)
        {
            log.UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            log.Username = context.User.FindFirstValue(ClaimTypes.Name);
        }

        // İstek gövdesini yakala
        var originalRequestBody = context.Request.Body;
        using var requestBodyStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Position = 0;

        // İstek gövdesini oku ve hassas verileri gizle
        using var requestReader = new StreamReader(requestBodyStream);
        var requestBody = await requestReader.ReadToEndAsync();
        log.RequestBody = SanitizeSensitiveData(requestBody);
        log.RequestSize = requestBody.Length;

        // İstek stream'ini sıfırla, böylece sonraki middleware'ler okuyabilir
        requestBodyStream.Position = 0;
        context.Request.Body = requestBodyStream;

        // Yanıt gövdesini yakalamak için orijinal response stream'ini değiştir
        var originalResponseBody = context.Response.Body;
        using var responseBodyStream = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBodyStream;

        // İsteği işleme süresi ölçümü için başlangıç zamanı al
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            // Request pipeline'ının geri kalanını çalıştır
            await _next(context);
        }
        catch (Exception ex)
        {
            // İşlem sırasında bir hata oluşursa, hata logunu kaydet
            _logger.LogError(ex, "Request pipeline execution error");
            log.Exception = ex.ToString();
            throw;
        }
        finally
        {
            // İşlem süresini hesapla
            var endTime = Stopwatch.GetTimestamp();
            log.ExecutionTime = Stopwatch.GetElapsedTime(startTime, endTime).Milliseconds;

            // Durum kodunu kaydet
            log.StatusCode = context.Response.StatusCode;

            // Yanıt gövdesini oku
            responseBodyStream.Position = 0;
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            log.ResponseBody = SanitizeSensitiveData(responseBody);
            log.ResponseSize = responseBody.Length;

            // Yanıtı istemciye döndür
            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            // Log kaydını MongoDB'ye kaydet
            try
            {
                await _logRepository.SaveRequestResponseLogAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving request/response log");
            }
        }
    }

    private bool ShouldSkipLogging(PathString path)
    {
        // Belirli endpoint'ler için loglama atlanmalı mı kontrol et
        return _excludedPaths.Any(excludedPath => 
            path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
    }

    private string SanitizeSensitiveData(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // JSON içeriğini parse et
        try
        {
            var jsonDoc = JsonDocument.Parse(content);
            using var outputStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = false });
            SanitizeJsonElement(jsonDoc.RootElement, writer);
            writer.Flush();
            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
        catch (JsonException)
        {
            // JSON olarak ayrıştırılamıyorsa, içeriği regex ile temizle
            var sanitized = content;
            sanitized = Regex.Replace(sanitized, "\"password\"\\s*:\\s*\"[^\"]*\"", "\"password\":\"***REDACTED***\"", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, "\"token\"\\s*:\\s*\"[^\"]*\"", "\"token\":\"***REDACTED***\"", RegexOptions.IgnoreCase);
            // diğer hassas alanlar için benzer regex'ler...
            return sanitized;
        }
    }

    private void SanitizeJsonElement(JsonElement element, Utf8JsonWriter writer)
    {
        // Hassas JSON alanlarını gizle
        // ... JSON işleme kodu ...
    }
}
```

### Middleware Kayıt Süreci

Middleware çalışma sırası:

1. Gelen istek alındığında, middleware `ShouldSkipLogging` metodu ile bu endpoint'in loglanıp loglanmayacağını kontrol eder.
2. `RecyclableMemoryStreamManager` kullanarak istek gövdesi için bir bellek akışı tahsis eder (bellek optimizasyonu için).
3. İstek gövdesini okur, hassas verileri gizler ve orijinal istek gövdesini diğer middleware'lerin okuyabilmesi için sıfırlar.
4. Yanıt gövdesini yakalamak için yanıt akışını değiştirir.
5. İstek işleme süresini ölçmek için bir sayaç başlatır.
6. Pipeline'ın geri kalanını çalıştırır.
7. Yanıt gövdesini okur, hassas verileri gizler ve yanıtı istemciye döndürür.
8. İstek, yanıt ve ilgili meta verileri bir `RequestResponseLog` nesnesine toplar.
9. Log nesnesini MongoDB'ye kaydeder.

## Yapılandırma

appsettings.json dosyasında loglama sistemi için aşağıdaki ayarlar kullanılabilir:

```json
"LogSettings": {
  "DatabaseName": "DenemeApiLogs",
  "ExpireAfterDays": 30,
  "ExcludedPaths": [
    "/swagger",
    "/health"
  ]
}
```

- **DatabaseName**: MongoDB veritabanı adı
- **ExpireAfterDays**: Logların silinmeden önce saklanacağı gün sayısı
- **ExcludedPaths**: Loglanmayacak endpoint'lerin listesi

Ayrıca, MongoDB bağlantı dizesi için:

```json
"ConnectionStrings": {
  "MongoDb": "mongodb://localhost:27017"
}
```

## Güvenlik Önlemleri

Loglama sistemi, güvenlik açısından aşağıdaki önlemleri içerir:

1. **Hassas Verilerin Gizlenmesi**: Şifre, token, refreshToken gibi hassas bilgiler otomatik olarak gizlenir ve "***REDACTED***" ile değiştirilir.

2. **Endpoint Filtreleme**: Belirli endpoint'ler (örn. /swagger, /health) loglama dışında bırakılabilir.

3. **TTL ile Otomatik Silme**: Loglar, belirli bir süre sonra otomatik olarak silinir.

## Extension Metotları

Loglama sistemini uygulamaya entegre etmek için bir dizi extension metodu kullanılır:

### ApplicationBuilderExtensions

```csharp
namespace Deneme.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
```

### LoggingExtensions

```csharp
namespace Deneme.Core.Extensions
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            // Log repository kayıtları
            services.AddSingleton<ILogRepository, MongoLogRepository>();
            
            // Log servisleri
            services.AddSingleton<IApiLogService, ApiLogService>();
            
            // RecyclableMemoryStreamManager kayıt (middleware için)
            services.AddSingleton<RecyclableMemoryStreamManager>();
            
            return services;
        }
    }
}
```

### Program.cs'de Kullanımı

```csharp
var builder = WebApplication.CreateBuilder(args);

// ... diğer servis kayıtları

// Loglama servislerini ekle
builder.Services.AddLoggingServices();

var app = builder.Build();

// ... diğer middleware kayıtları

// İstek/yanıt loglama middleware'ini ekle
app.UseRequestResponseLogging();

app.MapControllers();

app.Run();
```

## Best Practices

### Loglama Sistemi Kullanımı

1. **Doğru Log Seviyesi**: Log mesajları için doğru seviye kullanılmalıdır:
   - **Info**: Normal işlem bilgileri
   - **Warning**: Potansiyel sorunlar
   - **Error**: Gerçek hatalar ve istisnalar

2. **Anlamlı Mesajlar**: Log mesajları anlamlı ve açıklayıcı olmalıdır.

3. **Exception Detayları**: Hata loglarında, exception bilgileri de kaydedilmelidir.

### Log Erişimi ve Analizi

1. **Düzenli İnceleme**: MongoDB Collection'ları doğrudan inceleyerek sistem hataları için error loglar düzenli olarak kontrol edilmelidir.

2. **Şüpheli Aktivite**: Başarısız kimlik doğrulama denemeleri gibi şüpheli aktiviteler için loglar incelenmelidir.

3. **Performans Analizi**: İstek/yanıt loglarındaki ExecutionTime değeri, performans sorunlarını tespit etmek için kullanılabilir.

---

Loglama sistemi, Deneme API'nin güvenliğini, hata ayıklamasını ve izlenmesini kolaylaştırmak için tasarlanmıştır. Sistem, geliştiricilere ve yöneticilere, API kullanımı hakkında değerli bilgiler sağlar. 