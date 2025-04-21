# LogLibrary Kullanım Kılavuzu

## Mimari Genel Bakış

LogLibrary, çeşitli loglama hedefleriyle entegrasyon ve esneklik odaklı .NET uygulamaları için kapsamlı bir loglama çözümüdür. Kütüphane, açık sorumluluk ayrımı ile temiz bir mimari deseni takip eder. Bu kütüphane, merkezi bir loglama altyapısı kullanarak, uygulamanın farklı bölümlerinden gelen logları tutarlı bir şekilde kaydeder ve yönetir.

### Temel Bileşenler

1. **Arayüzler Katmanı**
   - `ILogService`: Loglama işlemleri için birincil arayüz. Temel loglama fonksiyonlarını tanımlar (Info, Error, Warning, Debug, Critical).
   - `ILogRepository`: Veri depolama işlemleri için repository arayüzü. Log verilerinin kaydedilmesi, sorgulanması ve yönetilmesi işlemlerini soyutlar.
   - Bu arayüzler, bağımlılık enjeksiyonu aracılığıyla uygulamaların loglama altyapısından bağımsız olmasını sağlar.

2. **Modeller Katmanı**
   - `LogEntry`: MongoDB entegrasyonu ile log verilerini temsil eden çekirdek model. Her log kaydı için gerekli tüm alanları içerir:
     - Zaman damgası
     - Log seviyesi (Info, Warning, Error, Debug, Critical)
     - Mesaj
     - Kaynak (hangi servis/metot)
     - Kullanıcı bilgileri (ID, isim, e-posta)
     - İşlem ID (tracing için)
     - Ek veri/obje
     - Uygulama adı
     - Ortam (Development, Production, vb.)
     - HTTP bilgileri (istek yolu, metot, durum kodu, süre)

3. **Servisler Katmanı**
   - `LogService`: Loglama mantığının temel uygulaması. Tüm log türleri için gerekli metotları içerir ve şu işlevleri yerine getirir:
     - Log kayıtlarını oluşturma ve repository'ye iletme
     - Hassas verilerin maskelenmesi
     - Asenkron loglama yönetimi
     - Standart .NET ILogger entegrasyonu
   - `GraylogService`: Graylog sunucusu ile entegrasyon. GELF formatında logları Graylog'a iletir ve şunları sağlar:
     - UDP protokolü üzerinden log gönderimi
     - Bağlantı hatalarına karşı dayanıklılık
     - Log objelerinin GELF formatına dönüştürülmesi

4. **Veri Katmanı**
   - `MongoLogRepository`: Repository deseninin MongoDB uygulaması. Log verilerinin MongoDB'de saklanmasını ve sorgulanmasını yönetir:
     - MongoDB koleksiyonuna bağlantı
     - CRUD işlemleri
     - İleri düzey sorgulama özellikleri (filtreleme, sayfalama)
     - Eski logların temizlenmesi
   - `MongoDbContext`: MongoDB bağlantıları için veritabanı bağlamı. Bağlantı yönetimini ve koleksiyon erişimini sağlar.

5. **Yapılandırma Katmanı**
   - `LogSettings`: Kütüphane için yapılandırma seçenekleri. Tüm loglama davranışlarını özelleştirmek için kullanılır:
     - MongoDB bağlantı ayarları
     - Uygulama ve ortam bilgisi
     - Log saklama süresi
     - Hassas veri maskeleme ayarları
     - Asenkron loglama seçenekleri
     - HTTP loglama yapılandırması
     - Graylog entegrasyon ayarları
   - JSON ayarlar dosyası desteği, kütüphanenin farklı ortamlarda (geliştirme, test, üretim) kolayca yapılandırılmasını sağlar.

## Çalışma Prensibi

LogLibrary, aşağıdaki temel adımlarla çalışır:

1. **Servis Kaydı**: Uygulama başlangıcında, `AddLogLibrary()` extension metodu ile tüm bağımlılıklar kaydedilir.
2. **Yapılandırma Yükleme**: Settings.json dosyasından veya program kodundan yapılandırma ayarları yüklenir.
3. **Log Oluşturma**: Uygulama içinde ILogService üzerinden log metotları çağrılır.
4. **Veri İşleme**: LogService, gelen verileri işler, hassas bilgileri maskeler ve LogEntry nesnesine dönüştürür.
5. **Veri Saklama**: LogEntry, MongoDB veritabanına kaydedilir ve/veya Graylog'a gönderilir.
6. **Sorgu ve Yönetim**: Log verileri sorgulanabilir, filtrelenebilir ve belirli bir saklama politikasına göre temizlenebilir.

## Kullanılan Tasarım Desenleri

1. **Repository Deseni**: `ILogRepository` aracılığıyla veri erişimini soyutlar. Bu, MongoDB yerine başka bir veritabanı kullanılmak istendiğinde sadece repository implementasyonunun değiştirilmesini sağlar.

2. **Bağımlılık Enjeksiyonu**: Tüm servisler uzantı metotları aracılığıyla kaydedilir. Bu, uygulamanın farklı bileşenlerinin birbirine gevşek bağlı olmasını sağlar ve test edilebilirliği artırır.

3. **Options Deseni**: `IOptions<LogSettings>` aracılığıyla yapılandırma. Bu, uygulama çalışırken yapılandırma değişikliklerinin yüklenmesini sağlar ve kodun yapılandırma detaylarından izole edilmesine yardımcı olur.

4. **Decorator Deseni**: Maskeleme gibi ek işlevlerle loglamayı zenginleştirir. Log verilerine ek işlevsellik eklemek için temel işlevlerin üzerine ek katmanlar ekler.

5. **Async/Await Deseni**: Tüm işlemler asenkron yürütmeyi destekler. Bu, loglama işlemlerinin ana uygulama akışını bloke etmemesini sağlar.

6. **Extension Metotları**: `AddLogLibrary()` aracılığıyla basitleştirilmiş servis kaydı. Kullanıcıların minimum kodla kütüphaneyi entegre etmesini sağlar.

7. **Factory Deseni**: LogEntry nesnelerinin oluşturulması için kullanılır, karmaşık nesne yaratma mantığını kapsüller.

8. **Singleton Deseni**: MongoDbContext gibi bileşenler için kullanılır, tek bir bağlantı havuzunun kullanılmasını sağlar.

## LogLibrary'nin Projenizde Kullanımı

### 1. Kurulum ve Bağımlılıklar

Projenize LogLibrary'yi eklemek için:

```xml
<!-- .csproj dosyanıza ekleyin -->
<ItemGroup>
  <PackageReference Include="LogLibrary" Version="1.0.0" />
</ItemGroup>
```

Gerekli bağımlılıklar:
- MongoDB.Driver (>= 2.19.0)
- Microsoft.Extensions.Options (>= 7.0.0)
- Microsoft.Extensions.DependencyInjection (>= 7.0.0)
- Microsoft.AspNetCore.Http.Abstractions (>= 2.2.0)

### 2. Kayıt

```csharp
// Program.cs veya Startup.cs içinde
public void ConfigureServices(IServiceCollection services)
{
    // Diğer servis kayıtları
    
    // LogLibrary'yi yapılandırma ile ekle
    services.AddLogLibrary(Configuration);
    
    // VEYA: Özel ayarlarla
    services.AddLogLibrary(options => {
        options.ConnectionString = "mongodb://localhost:27017";
        options.DatabaseName = "MyCustomLogs";
        options.ApplicationName = "MySpecialApp";
        options.EnableGraylog = true;
        options.GraylogHost = "graylog.company.com";
        options.GraylogPort = 12201;
    });
}
```

### 3. Temel Loglama

```csharp
// Constructor enjeksiyonu
private readonly ILogService _logService;

public YourClass(ILogService logService)
{
    _logService = logService;
}

// Log metotları ve kullanım örnekleri
public async Task UserLogin(string username, string password)
{
    try 
    {
        // İş mantığı...
        var user = await _userService.AuthenticateAsync(username, password);
        
        if (user != null)
        {
            // Başarılı giriş - INFO log
            await _logService.LogInfoAsync(
                message: $"Kullanıcı başarıyla giriş yaptı: {username}", 
                source: "AuthenticationService.UserLogin",
                data: new { Username = username, LoginTime = DateTime.UtcNow, IpAddress = GetClientIp() },
                userId: user.Id,
                userName: user.FullName,
                userEmail: user.Email
            );
            
            return user;
        }
        else
        {
            // Başarısız giriş - WARNING log
            await _logService.LogWarningAsync(
                message: $"Başarısız giriş denemesi: {username}",
                source: "AuthenticationService.UserLogin",
                data: new { Username = username, AttemptTime = DateTime.UtcNow, IpAddress = GetClientIp() }
            );
            
            return null;
        }
    }
    catch (Exception ex)
    {
        // Hata durumu - ERROR log
        await _logService.LogErrorAsync(
            message: $"Giriş işlemi sırasında hata: {ex.Message}",
            source: "AuthenticationService.UserLogin",
            exception: ex,
            userId: null,
            userName: username
        );
        
        throw;
    }
}

// Farklı log seviyelerinin uygun kullanımı:
// Hassas işlemleri DEBUG seviyesinde loglama
await _logService.LogDebugAsync(
    message: "Token oluşturma işlemi başlatıldı",
    source: "TokenService.GenerateToken",
    data: new { UserId = userId, TokenType = "AccessToken", ExpiresIn = 3600 }
);

// Sistem yüksek yük altındayken CRITICAL log
await _logService.LogCriticalAsync(
    message: "Veritabanı bağlantı havuzu tükendi!",
    source: "DatabaseMonitor",
    exception: poolException
);
```

### 4. HTTP Loglaması

```csharp
// HTTP Middleware içinde kullanım:
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogService _logService;
    
    public LoggingMiddleware(RequestDelegate next, ILogService logService)
    {
        _next = next;
        _logService = logService;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var originalBodyStream = context.Response.Body;
        
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;
        
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var userId = context.User?.FindFirst("sub")?.Value;
        var userName = context.User?.Identity?.Name;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        try
        {
            // İsteği işle
            await _next(context);
            
            // Yanıtı kaydet
            watch.Stop();
            var statusCode = context.Response.StatusCode;
            var durationMs = watch.ElapsedMilliseconds;
            
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            
            // Log oluştur
            await _logService.LogHttpAsync(
                path: path,
                method: method,
                statusCode: statusCode,
                durationMs: durationMs,
                traceId: traceId,
                userId: userId,
                userName: userName,
                ipAddress: ipAddress,
                requestData: JsonConvert.DeserializeObject(requestBody),
                responseData: JsonConvert.DeserializeObject(responseBody)
            );
        }
        catch (Exception ex)
        {
            // Hata durumunda log oluştur
            watch.Stop();
            await _logService.LogErrorAsync(
                message: $"HTTP isteği işlenirken hata: {ex.Message}",
                source: "LoggingMiddleware",
                exception: ex,
                userId: userId
            );
            
            // Hata yanıtı oluştur ve log
            context.Response.StatusCode = 500;
            
            var errorResponse = new { error = "Internal Server Error" };
            var errorResponseJson = JsonConvert.SerializeObject(errorResponse);
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await new StreamWriter(responseBodyStream).WriteAsync(errorResponseJson);
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            
            await _logService.LogHttpAsync(
                path: path,
                method: method,
                statusCode: 500,
                durationMs: watch.ElapsedMilliseconds,
                traceId: traceId,
                userId: userId,
                userName: userName,
                ipAddress: ipAddress,
                requestData: JsonConvert.DeserializeObject(requestBody),
                responseData: errorResponse
            );
        }
        finally
        {
            // Orijinal response body'yi geri yükle
            await responseBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
    
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        // İstek gövdesini oku (detaylar atlandı)
        // ...
    }
}

// Middleware'i kaydetme:
app.UseMiddleware<LoggingMiddleware>();
```

### 5. Log Alımı ve Yönetimi

```csharp
// Log Yönetim Servisi örneği
public class LogManagementService
{
    private readonly ILogService _logService;
    
    public LogManagementService(ILogService logService)
    {
        _logService = logService;
    }
    
    // Sayfalama ve filtreleme ile logları alma
    public async Task<(IEnumerable<LogEntry> Logs, long TotalCount)> GetFilteredLogsAsync(
        int page = 1,
        int pageSize = 50,
        string level = null,
        string searchKeyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        // Kullanıcının istediği filtrelerle logları getir
        return await _logService.GetLogsAsync(
            pageNumber: page,
            pageSize: pageSize,
            level: level,
            searchTerm: searchKeyword,
            startDate: startDate,
            endDate: endDate
        );
    }
    
    // Günlük log temizleme işlemi (Scheduled Task/Background Service içinde kullanılabilir)
    public async Task CleanupOldLogsAsync()
    {
        // 30 günden eski logları temizle
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        var deletedCount = await _logService.CleanupLogsAsync(cutoffDate);
        
        // Temizleme işlemini de logla
        await _logService.LogInfoAsync(
            message: $"{deletedCount} adet eski log kaydı temizlendi",
            source: "LogManagementService.CleanupOldLogs",
            data: new { CutoffDate = cutoffDate, DeletedCount = deletedCount }
        );
    }
}
```

## Yapılandırma Seçenekleri

Kütüphane `appsettings.json` aracılığıyla yapılandırılabilir. Aşağıda tüm yapılandırma seçenekleri ve açıklamaları bulunmaktadır:

```json
{
  "LogSettings": {
    // MongoDB bağlantı bilgileri
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MyAppLogs",
    "CollectionName": "Logs",
    
    // Uygulama bilgileri
    "ApplicationName": "MyApplication",
    "Environment": "Production", // Development, Staging, Production
    
    // Log saklama politikası (gün olarak)
    "RetentionDays": 30,
    
    // Hassas veri maskeleme ayarı
    "MaskSensitiveData": true,
    
    // Performans ayarları
    "EnableAsyncLogging": true, // Ana thread'i bloklamadan arkaplanda loglama
    
    // HTTP loglama ayarları
    "EnableHttpLogging": true,
    
    // Graylog entegrasyon ayarları
    "EnableGraylog": false,
    "GraylogHost": "graylog.example.com",
    "GraylogPort": 12201,
    
    // MongoDB bağlantı havuzu ayarları
    "MaxConnectionPoolSize": 100,
    "ConnectionTimeoutMs": 30000,
    
    // Loglama limitleri
    "MaxLogSize": 1048576, // 1 MB
    "TruncateLargeMessages": true
  }
}
```

## Gelişmiş Özellikler

### 1. Hassas Veri Maskeleme

LogLibrary, otomatik olarak hassas verileri maskeler. Bu özellik, güvenlik açısından kritik öneme sahiptir:

```csharp
// LogService.cs içinde:
private object MaskSensitiveData(object data)
{
    if (data == null || !_logSettings.MaskSensitiveData)
        return data;

    try
    {
        // Objeyi JSON'a dönüştür
        var json = JsonConvert.SerializeObject(data);
        
        // Hassas verileri maskele
        var maskedJson = MaskSensitiveDataInJson(json);
        
        // Tekrar objeye dönüştür
        return JsonConvert.DeserializeObject(maskedJson);
    }
    catch
    {
        // Hata durumunda orijinal veriyi döndür
        return data;
    }
}

// Hassas alanlar için maskeleme desenleri
private static readonly Dictionary<string, string> SensitivePatterns = new Dictionary<string, string>
{
    { "password", "\"password\":\"[MASKED]\"" },
    { "Password", "\"Password\":\"[MASKED]\"" },
    { "token", "\"token\":\"[MASKED]\"" },
    { "Token", "\"Token\":\"[MASKED]\"" },
    { "secret", "\"secret\":\"[MASKED]\"" },
    { "Secret", "\"Secret\":\"[MASKED]\"" },
    { "creditcard", "\"creditcard\":\"[MASKED]\"" },
    { "CreditCard", "\"CreditCard\":\"[MASKED]\"" },
    { "ssn", "\"ssn\":\"[MASKED]\"" },
    { "SSN", "\"SSN\":\"[MASKED]\"" }
};
```

### 2. Asenkron Loglama

Performans için, loglama işlemleri arka planda asenkron olarak gerçekleştirilebilir:

```csharp
// LogService.cs içinde:
if (_logSettings.EnableAsyncLogging)
{
    // Ana thread'i bloklamadan arka planda çalıştır
    _ = Task.Run(async () => 
    {
        try
        {
            await _logRepository.SaveLogAsync(logEntry);
            
            if (_logSettings.EnableGraylog)
            {
                await _graylogService.SendLogAsync(logEntry);
            }
        }
        catch (Exception ex)
        {
            // Başarısız loglama denemelerini konsola yaz
            Console.WriteLine($"Async loglama hatası: {ex.Message}");
        }
    });
}
else
{
    // Senkron loglama
    await _logRepository.SaveLogAsync(logEntry);
    
    if (_logSettings.EnableGraylog)
    {
        await _graylogService.SendLogAsync(logEntry);
    }
}
```

### 3. Graylog Entegrasyonu

Logların merkezi bir Graylog sunucusuna iletilmesi:

```csharp
// GraylogService.cs içinde:
public async Task SendLogAsync(LogEntry logEntry)
{
    if (!_logSettings.EnableGraylog || string.IsNullOrEmpty(_logSettings.GraylogHost))
        return;

    try
    {
        // LogEntry'yi GELF formatına dönüştür
        var gelfMessage = ConvertToGelf(logEntry);
        
        // UDP üzerinden gönder
        using var udpClient = new UdpClient();
        var gelfJson = JsonConvert.SerializeObject(gelfMessage);
        var bytes = Encoding.UTF8.GetBytes(gelfJson);
        
        await udpClient.SendAsync(
            bytes,
            bytes.Length,
            _logSettings.GraylogHost,
            _logSettings.GraylogPort
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Graylog'a log gönderimi başarısız: {ex.Message}");
    }
}

private object ConvertToGelf(LogEntry logEntry)
{
    // GELF format dönüşümü (detaylar atlandı)
    // ...
}
```

### 4. Log Saklama ve Temizleme

Otomatik log temizleme ve saklama politikası yönetimi:

```csharp
// Scheduled task/Hosted Service örneği:
public class LogCleanupService : BackgroundService
{
    private readonly ILogService _logService;
    private readonly IOptions<LogSettings> _logSettings;
    
    public LogCleanupService(
        ILogService logService,
        IOptions<LogSettings> logSettings)
    {
        _logService = logService;
        _logSettings = logSettings;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // RetentionDays ayarına göre eski logları temizle
                var retentionDays = _logSettings.Value.RetentionDays;
                var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
                
                var deletedCount = await _logService.CleanupLogsAsync(cutoffDate);
                
                // Temizleme sonucunu logla
                await _logService.LogInfoAsync(
                    message: $"Otomatik log temizleme: {deletedCount} kayıt silindi",
                    source: "LogCleanupService"
                );
            }
            catch (Exception ex)
            {
                // Hata durumunu logla
                await _logService.LogErrorAsync(
                    message: "Log temizleme sırasında hata",
                    source: "LogCleanupService",
                    exception: ex
                );
            }
            
            // Her gün çalıştır
            var delay = TimeSpan.FromDays(1);
            await Task.Delay(delay, stoppingToken);
        }
    }
}
```

### 5. Yapılandırılmış Loglama

Karmaşık objeler ve structured logging desteği:

```csharp
// Örnek yapılandırılmış log:
var orderData = new
{
    OrderId = order.Id,
    CustomerId = order.CustomerId,
    TotalAmount = order.TotalAmount,
    PaymentMethod = order.PaymentMethod,
    Items = order.Items.Select(i => new
    {
        ProductId = i.ProductId,
        Quantity = i.Quantity,
        UnitPrice = i.UnitPrice
    }).ToList(),
    BillingAddress = new
    {
        Street = order.BillingAddress.Street,
        City = order.BillingAddress.City,
        Country = order.BillingAddress.Country,
        PostalCode = order.BillingAddress.PostalCode
    }
};

await _logService.LogInfoAsync(
    message: $"Sipariş #{order.Id} başarıyla oluşturuldu",
    source: "OrderService.CreateOrder",
    data: orderData
);
```

## En İyi Uygulamalar

1. **Her zaman durum için uygun log seviyesini kullanın:**
   - `Info`: Normal operasyonel bilgiler (kullanıcı girişi, sipariş oluşturma)
   - `Warning`: Potansiyel sorunlar (başarısız giriş denemeleri, geçici kesintiler)
   - `Error`: İşlem tamamlanamadığında oluşan hatalar (veritabanı hataları, API çağrı hataları)
   - `Debug`: Geliştirme ve sorun giderme için ayrıntılı bilgiler (metot çağrıları, parametre değerleri)
   - `Critical`: Sistemin çalışmasını tehdit eden ciddi hatalar (bağlantı havuzu tükenmesi, disk dolması)

2. **Her logla birlikte açık bir kaynak tanımlayıcı ekleyin:**
   - Format: `"{Sınıf}.{Metot}"` (örn. `"UserService.Authenticate"`)
   - Bu şekilde logların hangi kod parçasından geldiğini kolayca belirleyebilirsiniz

3. **Stringleri birleştirmek yerine mümkün olduğunda yapılandırılmış veri sağlayın:**
   - Kötü: `"Kullanıcı John Doe 5 ürünü 100TL tutarında satın aldı"`
   - İyi: Mesaj `"Sipariş tamamlandı"` + Veri: `{ UserId: 123, UserName: "John Doe", ItemCount: 5, TotalAmount: 100 }`

4. **API projeleri için HTTP loglama middleware'i kullanın:**
   - Tüm HTTP isteklerini ve yanıtlarını otomatik olarak loglamak için middleware entegrasyonu yapın
   - Performans ve hata izleme için HTTP yanıt sürelerini ölçün

5. **Veri gereksinimlerinize göre log saklama süresini yapılandırın:**
   - Üretim ortamında disk alanını korumak için eski logları düzenli olarak temizleyin
   - Kritik loglar için daha uzun saklama süreleri düşünün (örn. finansal işlemler)

6. **Desenleri belirlemek için düzenli log analizleri yapın:**
   - Tekrarlayan hataları tespit etmek için düzenli aralıklarla logları analiz edin
   - Performans darboğazlarını belirlemek için HTTP yanıt sürelerini gözden geçirin

7. **Hassas verileri loglamaktan kaçının:**
   - Şifreler, tokenlar, kredi kartı bilgileri gibi hassas verileri asla doğrudan loglamayın
   - MaskSensitiveData özelliğinin her zaman etkin olduğundan emin olun

8. **Günlük log hacmini izleyin:**
   - Çok fazla loglama, disk alanı ve performans sorunlarına yol açabilir
   - Debug loglarını üretim ortamında etkinleştirirken dikkatli olun

## Entegrasyon Örnekleri

### ASP.NET Core Middleware

```csharp
// Startup.cs içinde:
public void Configure(IApplicationBuilder app)
{
    // Diğer middleware'ler
    
    // Loglama middleware'ini ekle
    app.UseMiddleware<LoggingMiddleware>();
    
    // Diğer middleware'ler...
}
```

### Worker Servisi Entegrasyonu

```csharp
public class OrderProcessingWorker : BackgroundService
{
    private readonly IOrderProcessor _orderProcessor;
    private readonly ILogService _logService;
    
    public OrderProcessingWorker(
        IOrderProcessor orderProcessor,
        ILogService logService)
    {
        _orderProcessor = orderProcessor;
        _logService = logService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _logService.LogInfoAsync(
            message: "Order Processing Worker başlatıldı",
            source: "OrderProcessingWorker"
        );
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try 
            {
                // Bekleyen siparişleri işle
                var orders = await _orderProcessor.GetPendingOrdersAsync();
                
                await _logService.LogInfoAsync(
                    message: $"{orders.Count} adet bekleyen sipariş bulundu",
                    source: "OrderProcessingWorker.ExecuteAsync",
                    data: new { OrderCount = orders.Count }
                );
                
                foreach (var order in orders)
                {
                    try
                    {
                        await _orderProcessor.ProcessOrderAsync(order);
                        
                        await _logService.LogInfoAsync(
                            message: $"Sipariş başarıyla işlendi: #{order.Id}",
                            source: "OrderProcessingWorker.ExecuteAsync",
                            data: new { OrderId = order.Id, CustomerId = order.CustomerId }
                        );
                    }
                    catch (Exception orderEx)
                    {
                        await _logService.LogErrorAsync(
                            message: $"Sipariş işlenirken hata: #{order.Id}",
                            source: "OrderProcessingWorker.ExecuteAsync",
                            exception: orderEx,
                            data: new { OrderId = order.Id, CustomerId = order.CustomerId }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "Worker çalışması başarısız",
                    source: "OrderProcessingWorker.ExecuteAsync",
                    exception: ex
                );
            }
            
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
        
        await _logService.LogInfoAsync(
            message: "Order Processing Worker durduruldu",
            source: "OrderProcessingWorker"
        );
    }
}
```

### Web API Controller Entegrasyonu

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogService _logService;
    
    public ProductsController(
        IProductService productService,
        ILogService logService)
    {
        _productService = productService;
        _logService = logService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQuery query)
    {
        await _logService.LogDebugAsync(
            message: "Ürün listesi istendi",
            source: "ProductsController.GetProducts",
            data: query
        );
        
        try
        {
            var products = await _productService.GetProductsAsync(
                query.Category,
                query.MinPrice,
                query.MaxPrice,
                query.Page,
                query.PageSize);
                
            await _logService.LogInfoAsync(
                message: $"{products.Count} adet ürün listelendi",
                source: "ProductsController.GetProducts",
                data: new { 
                    QueryParams = query,
                    ResultCount = products.Count
                }
            );
            
            return Ok(products);
        }
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(
                message: "Ürün listesi alınırken hata oluştu",
                source: "ProductsController.GetProducts",
                exception: ex,
                data: query
            );
            
            return StatusCode(500, new { error = "Ürünler alınamadı" });
        }
    }
}
```

## Sorun Giderme ve İzleme

1. **MongoDB Bağlantı Sorunları**
   - MongoDB bağlantı dizesini ve erişim izinlerini kontrol edin
   - Ağ erişiminde güvenlik duvarı kısıtlamaları olup olmadığını kontrol edin
   - MongoDB sunucusunun çalışır durumda olduğunu doğrulayın

2. **Log Hacmi Yönetimi**
   - DEBUG seviyesindeki logları üretim ortamında devre dışı bırakarak log hacmini azaltın
   - RetentionDays ayarını ihtiyaçlarınıza göre optimize edin
   - Düzenli temizleme görevleri planlayın

3. **Performans İzleme**
   - EnableAsyncLogging ayarını `true` yaparak ana uygulama performansını koruyun
   - Loglama işlemlerinin uygulamanızı yavaşlatıp yavaşlatmadığını izleyin
   - Çok büyük nesnelerin loglanmasından kaçının

4. **Graylog Entegrasyon Sorunları**
   - UDP port erişimini kontrol edin (genellikle 12201)
   - Güvenlik duvarı kurallarını kontrol edin
   - GELF girişinin Graylog'da doğru yapılandırıldığından emin olun

5. **Hassas Veri Sızıntıları**
   - MaskSensitiveData ayarının aktif olduğundan emin olun
   - SensitivePatterns listesinin ihtiyaçlarınıza göre genişletildiğinden emin olun
   - Düzenli olarak log verilerini hassas bilgi içerip içermediğine dair gözden geçirin 