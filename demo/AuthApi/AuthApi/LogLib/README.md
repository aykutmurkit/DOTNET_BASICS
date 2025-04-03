# LogLib

LogLib, gelişmiş loglama yetenekleri sunan, MongoDB tabanlı bir .NET kütüphanesidir. Endüstri standartlarına uygun log formatı, filtreleme özellikleri ve API entegrasyonu ile uygulamalarınız için kapsamlı bir log çözümü sağlar.

## Özellikler

- MongoDB entegrasyonu ile ölçeklenebilir log depolama
- Farklı log seviyeleri (Debug, Info, Warning, Error, Critical)
- Kullanıcı bilgisi, IP adresi gibi zengin içerikli loglar
- API istekleri ve yanıtları için özel loglama
- Gelişmiş filtreleme ve arama yetenekleri
- Endüstri standartlarına uygun log formatı (ELK Stack/Graylog uyumlu)
- Swagger ile belgelenmiş REST API

## Kurulum

### 1. NuGet Paketi Eklemek

```shell
dotnet add package LogLib
```

### 2. Projeye Doğrudan Referans Eklemek

```shell
dotnet add reference ../path/to/LogLib/LogLib.csproj
```

## Yapılandırma

### 1. appsettings.json Yapılandırması

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  },
  "LogSettings": {
    "ApplicationName": "YourAppName",
    "Environment": "Development",
    "DatabaseName": "ApplicationLogs",
    "ExpireAfterDays": 30,
    "ResetDatabaseOnStartup": false
  }
}
```

### 2. Servis Kayıtları (Program.cs veya Startup.cs)

**Kolay Yöntem (Önerilen):**

```csharp
using LogLib.Core.Extensions;

// ... diğer servis yapılandırmaları ...

// Otomatik olarak tüm LogLib servislerini ekler
services.AddLogLib(builder.Configuration);

// Alternatif olarak, bağlantı dizesini doğrudan sağlayabilirsiniz
// services.AddLogLib("mongodb://localhost:27017");
```

**Veya Manuel Yapılandırma:**

```csharp
// MongoDB client kaydı
services.AddSingleton<IMongoClient>(sp => 
{
    var connectionString = configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

// LogLib servislerini kaydet
services.AddSingleton<ILogRepository, MongoLogRepository>();
services.AddSingleton<ILogService, LogService>();

// HTTP context accessor kaydı
services.AddHttpContextAccessor();
```

## Temel Kullanım Örnekleri

### 1. Dependency Injection ile Servis Kullanımı

```csharp
public class ExampleController : ControllerBase
{
    private readonly ILogService _logService;

    public ExampleController(ILogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await _logService.LogInfoAsync("API endpoint çağrıldı");
        // İşlemler...
        return Ok("Success");
    }
}
```

### 2. Farklı Log Seviyeleri

```csharp
// Debug log
await _logService.LogDebugAsync("Debug mesajı");

// Info log
await _logService.LogInfoAsync("Bilgi mesajı");

// Warning log
await _logService.LogWarningAsync("Uyarı mesajı");

// Error log
try {
    // Hata oluşturabilecek kod
} 
catch (Exception ex) {
    await _logService.LogErrorAsync("Bir hata oluştu", ex);
}

// Critical log
await _logService.LogCriticalAsync("Kritik bir hata oluştu", exception);

// Security log
await _logService.LogSecurityAsync("Kullanıcı girişi başarısız", userId: "123", ipAddress: "192.168.1.1");
```

### 3. Kullanıcı Bilgileriyle Loglama

```csharp
await _logService.LogInfoAsync(
    "Kullanıcı profili güncellendi",
    userId: user.Id,
    username: user.Username,
    userEmail: user.Email
);
```

### 4. Ek Verilerle Loglama

```csharp
var additionalData = new Dictionary<string, object>
{
    { "OrderId", order.Id },
    { "OrderAmount", order.Amount },
    { "Products", order.Products }
};

await _logService.LogInfoAsync(
    "Sipariş tamamlandı",
    userId: user.Id,
    additionalData: additionalData
);
```

## Gelişmiş Kullanım Örnekleri

### 1. API İsteklerini Loglama

```csharp
// Middleware veya filtrelerde kullanım örneği
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    
    // İstek logla
    await _logService.LogApiRequestAsync(
        context.Request.Path,
        context.Request.Method,
        userId: context.User?.FindFirst("nameid")?.Value,
        username: context.User?.FindFirst("unique_name")?.Value,
        ipAddress: context.Connection.RemoteIpAddress.ToString(),
        requestBody: await GetRequestBodyAsync(context.Request)
    );
    
    await next();
    stopwatch.Stop();
    
    // Yanıt logla
    await _logService.LogApiResponseAsync(
        context.Request.Path,
        context.Request.Method,
        context.Response.StatusCode,
        stopwatch.ElapsedMilliseconds,
        userId: context.User?.FindFirst("nameid")?.Value,
        username: context.User?.FindFirst("unique_name")?.Value
    );
});
```

### 2. Logları Sorgulama

```csharp
// Son 24 saatin hata loglarını getir
var yesterday = DateTime.UtcNow.AddDays(-1);
var errorLogs = await _logService.GetLogsAsync(
    level: "Error",
    startDate: yesterday
);

// Belirli bir kullanıcının loglarını getir
var userLogs = await _logService.GetLogsAsync(
    userId: "123",
    take: 50
);

// Arama terimine göre filtreleme
var searchResults = await _logService.GetLogsAsync(
    searchTerm: "sipariş hatası"
);
```

## Diğer Projelere Entegrasyon

### ASP.NET Core Web API Entegrasyonu

1. Proje Referansı Eklemek:

```shell
dotnet add reference ../path/to/LogLib/LogLib.csproj
```

2. Program.cs Yapılandırması:

```csharp
// LogLib için gerekli using ifadeleri
using LogLib.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// LogLib servislerini tek adımda ekleyin
builder.Services.AddLogLib(builder.Configuration);

// Diğer servis kayıtları
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware yapılandırması
// ...

app.Run();
```

3. Controller'larda Kullanım:

```csharp
using LogLib.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly IUserService _userService;

    public UsersController(ILogService logService, IUserService userService)
    {
        _logService = logService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try 
        {
            await _logService.LogInfoAsync("Kullanıcı listesi istendi");
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            await _logService.LogErrorAsync("Kullanıcılar getirilirken hata oluştu", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
```

### Blazor Uygulamasına Entegrasyon

```csharp
// Program.cs
using LogLib.Core.Extensions;

// Tek satırda LogLib servislerini ekleyin
builder.Services.AddLogLib(builder.Configuration);

// BlazorComponent.razor.cs
public partial class BlazorComponent
{
    [Inject]
    private ILogService LogService { get; set; }
    
    private async Task ButtonClickHandler()
    {
        await LogService.LogInfoAsync("Buton tıklandı");
        // İşlemler...
    }
}
```

### Console Uygulamasına Entegrasyon

```csharp
using LogLib.Core.Extensions;
using LogLib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Yapılandırma
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            
        // Servis koleksiyonu
        var services = new ServiceCollection();
        
        // LogLib servislerini tek adımda ekleyin
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogLib(configuration);
        
        // Kendi servislerinizi ekleyin
        // services.AddTransient<IYourService, YourService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var logService = serviceProvider.GetRequiredService<ILogService>();
        
        // Log oluştur
        await logService.LogInfoAsync("Konsol uygulaması başlatıldı");
        
        // Uygulama kodları...
        
        await logService.LogInfoAsync("Konsol uygulaması kapatıldı");
    }
}
```

## API Referansı

### ILogService Metotları

```csharp
// Debug log oluşturma
Task LogDebugAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Bilgi logu oluşturma
Task LogInfoAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Uyarı logu oluşturma
Task LogWarningAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Hata logu oluşturma
Task LogErrorAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Kritik hata logu oluşturma
Task LogCriticalAsync(string message, Exception? exception = null, string? userId = null, string? username = null, string? userEmail = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Güvenlik logu oluşturma
Task LogSecurityAsync(string message, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// API istek logu oluşturma
Task LogApiRequestAsync(string path, string method, string? userId = null, string? username = null, string? userEmail = null, string? ipAddress = null, string? requestBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// API yanıt logu oluşturma
Task LogApiResponseAsync(string path, string method, int statusCode, long durationMs, string? userId = null, string? username = null, string? userEmail = null, string? responseBody = null, string? correlationId = null, Dictionary<string, object>? additionalData = null);

// Logları sorgulama
Task<IEnumerable<LogEntry>> GetLogsAsync(
    string? level = null,
    string? application = null,
    string? userId = null,
    string? username = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? searchTerm = null,
    int skip = 0,
    int take = 100);
```

## Servis Uzantı Metotları (ServiceCollectionExtensions)

```csharp
// IConfiguration ile kullanım
services.AddLogLib(configuration);

// Doğrudan bağlantı dizesi ile kullanım
services.AddLogLib("mongodb://localhost:27017");
```

## Örnek appsettings.json

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  },
  "LogSettings": {
    "ApplicationName": "YourAppName",
    "Environment": "Development",
    "DatabaseName": "ApplicationLogs",
    "ExpireAfterDays": 30,
    "ResetDatabaseOnStartup": false
  }
}
```

## Lisans

MIT 