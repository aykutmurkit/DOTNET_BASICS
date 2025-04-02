# N-Tier Mimari Dokümantasyonu

Bu doküman, DeviceApi'nin N-Tier (Çok Katmanlı) mimari yapısını açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Projenin Katmanlı Yapısı](#projenin-katmanlı-yapısı)
- [Katmanlar Arası İletişim](#katmanlar-arası-i̇letişim)
- [API Katmanı](#api-katmanı)
- [Business Katmanı](#business-katmanı)
- [DataAccess Katmanı](#dataaccess-katmanı)
- [Entity Katmanı](#entity-katmanı)
- [Core Katmanı](#core-katmanı)
- [Uygulama Başlatma Süreci](#uygulama-başlatma-süreci)
- [Bağımlılık Enjeksiyonu](#bağımlılık-enjeksiyonu)
- [Middleware Yapısı](#middleware-yapısı)
- [Extension Metotları](#extension-metotları)

## Genel Bakış

DeviceApi, modern bir yazılım geliştirme yaklaşımı olan N-Tier (Çok Katmanlı) mimari üzerine inşa edilmiştir. Bu mimari, uygulamayı mantıksal ve fiziksel olarak birbirinden ayrılmış katmanlara bölerek, daha modüler, test edilebilir, bakımı daha kolay ve ölçeklenebilir hale getirir.

N-Tier mimari, her katmanın belirli bir sorumluluğu olduğu ve sadece kendinden bir alt katmanla iletişim kurabildiği bir yapıdır. Bu sayede, herhangi bir katmanda yapılan değişiklikler diğer katmanları minimum etkileyecektir.

## Projenin Katmanlı Yapısı

DeviceApi projesi, klasör bazlı N-Tier mimarisi kullanılarak aşağıdaki katmanlardan oluşmaktadır:

```
DeviceApi/
│
├── API/                      # Presentation Layer
│   ├── Controllers/          # API endpoint'lerini içeren controller'lar
│   ├── Middleware/           # HTTP Pipeline'ında kullanılan middleware'ler
│   ├── Models/               # API'ye özgü model sınıfları
│   └── Extensions/           # API katmanına özgü extension metotları
│
├── Business/                 # Business Layer
│   ├── Services/             # İş mantığını içeren servisler
│   │   ├── Concrete/         # Servis implementasyonları
│   │   └── Interfaces/       # Servis arayüzleri
│   └── Extensions/           # Business katmanına özgü extension metotları
│
├── DataAccess/               # Data Access Layer
│   ├── Context/              # EF Core DbContext sınıfları
│   ├── Repositories/         # Repository pattern implementasyonları
│   │   ├── Concrete/         # Somut repository sınıfları
│   │   └── Interfaces/       # Repository arayüzleri
│   ├── Configurations/       # Entity Configurations
│   ├── Seeding/              # Seed sınıfları ve veritabanı ilk veriler
│   └── Extensions/           # DataAccess katmanına özgü extension metotları
│
├── Entities/                 # Entity Layer
│   ├── Concrete/             # Veritabanı entity'leri
│   │   ├── Device.cs         # Cihaz varlığı
│   │   ├── Platform.cs       # Platform varlığı  
│   │   └── Station.cs        # İstasyon varlığı
│   └── DTOs/                 # Veri Transfer Objeleri
│
├── Core/                     # Core Layer
│   ├── Security/             # Güvenlik ile ilgili sınıflar
│   ├── Utilities/            # Yardımcı sınıflar ve metotlar
│   ├── Extensions/           # Genel extension metotları
│   └── Logging/              # Loglama altyapısı
│
├── Program.cs                # Uygulama giriş noktası ve konfigürasyon
└── appsettings.json          # Uygulama ayarları
```

## Namespace Yapısı

Projenin namespace yapısı, klasör yapısına paralel olarak düzenlenmiştir:

```
DeviceApi.API.*                  # API katmanı namespace'leri
DeviceApi.Business.*             # Business katmanı namespace'leri
DeviceApi.DataAccess.*           # DataAccess katmanı namespace'leri
DeviceApi.Entities.*             # Entity katmanı namespace'leri
DeviceApi.Core.*                 # Core katmanı namespace'leri
```

## Katmanlar Arası İletişim

N-Tier mimarideki temel prensip, katmanlar arasındaki iletişimin tek yönlü olmasıdır. Her katman, kendinden bir alt katmanla iletişim kurabilir, ancak üst katmanlara doğrudan erişemez.

```
API → Business → DataAccess → Entity
  ↓       ↓          ↓         ↓
  └───────┴──────────┴─────────┘
               ↓
             Core
```

Bu yapı sayesinde:

1. **Bağımlılık Yönü**: Üst katmanlar, alt katmanlara bağımlıdır, ancak alt katmanlar üst katmanlara bağımlı değildir.
2. **Değişiklik Yalıtımı**: Bir katmandaki değişiklik, sadece o katmana ve ona bağımlı olan katmanlara etki eder.
3. **Modülerlik**: Her katman bağımsız olarak geliştirilebilir ve test edilebilir.

## API Katmanı

API katmanı, uygulamanın en dış katmanıdır ve kullanıcılarla doğrudan etkileşimde bulunur.

### Temel Bileşenler

- **Controllers**: HTTP isteklerini kabul eden ve yanıtları döndüren endpoint'ler.
- **Middleware**: HTTP istek/yanıt pipeline'ında çalışan ara yazılımlar.
- **Models**: API istekleri ve yanıtları için kullanılan veri modelleri.
- **Extensions**: API katmanına özgü extension metotları.

### Örnek Controller

```csharp
[Route("api/[controller]")]
[ApiController]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllDevices()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(ApiResponse<List<DeviceDto>>.Success(devices, "Cihazlar başarıyla getirildi"));
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetDeviceById(int id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        return Ok(ApiResponse<DeviceDto>.Success(device, "Cihaz başarıyla getirildi"));
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDevice(CreateDeviceRequest request)
    {
        var device = await _deviceService.CreateDeviceAsync(request);
        return Created($"/api/devices/{device.Id}", ApiResponse<DeviceDto>.Success(device, "Cihaz başarıyla oluşturuldu", 201));
    }
    
    // ... Diğer action metotları
}
```

### Middleware Örneği

```csharp
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly string[] _excludedPaths;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        _excludedPaths = configuration.GetSection("LogSettings:ExcludedPaths").Get<string[]>() ?? Array.Empty<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Loglama işlemleri burada gerçekleştirilir
        // ...
    }
}
```

## Business Katmanı

Business katmanı, uygulamanın iş mantığını içerir ve veritabanı işlemleri ile sunum katmanı arasında bir köprü görevi görür.

### Temel Bileşenler

- **Services/Interfaces**: İş mantığı için servis arayüzleri.
- **Services/Concrete**: Servis arayüzlerinin implementasyonları.
- **Extensions**: Business katmanı için extension metotları.

### Örnek Servis Arayüzü

```csharp
public interface IDeviceService
{
    Task<List<DeviceDto>> GetAllDevicesAsync();
    Task<DeviceDto> GetDeviceByIdAsync(int id);
    Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request);
    Task<DeviceDto> UpdateDeviceAsync(int id, UpdateDeviceRequest request);
    Task DeleteDeviceAsync(int id);
    // ... Diğer metotlar
}
```

### Örnek Servis İmplementasyonu

```csharp
public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IPlatformRepository _platformRepository;

    public DeviceService(IDeviceRepository deviceRepository, IPlatformRepository platformRepository)
    {
        _deviceRepository = deviceRepository;
        _platformRepository = platformRepository;
    }

    public async Task<List<DeviceDto>> GetAllDevicesAsync()
    {
        var devices = await _deviceRepository.GetAllAsync(include: d => d.Platform);
        return devices.Select(MapToDeviceDto).ToList();
    }
    
    public async Task<DeviceDto> GetDeviceByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id, include: d => d.Platform);
        if (device == null)
        {
            throw new NotFoundException($"ID {id} ile cihaz bulunamadı");
        }
        
        return MapToDeviceDto(device);
    }
    
    public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
    {
        // Platform varlığının kontrolü
        var platform = await _platformRepository.GetByIdAsync(request.PlatformId);
        if (platform == null)
        {
            throw new NotFoundException($"ID {request.PlatformId} ile platform bulunamadı");
        }
        
        // IP ve port çiftinin benzersiz olup olmadığını kontrol et
        if (await _deviceRepository.AnyAsync(d => d.Ip == request.Ip && d.Port == request.Port))
        {
            throw new ConflictException($"{request.Ip}:{request.Port} IP-port kombinasyonu zaten kullanımda");
        }
        
        // Yeni cihaz oluştur
        var device = new Device
        {
            Name = request.Name,
            Ip = request.Ip,
            Port = request.Port,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PlatformId = request.PlatformId
        };
        
        await _deviceRepository.AddAsync(device);
        
        // Eklenen cihazı ID ve platform bilgisiyle birlikte getir
        var createdDevice = await _deviceRepository.GetByIdAsync(device.Id, include: d => d.Platform);
        
        return MapToDeviceDto(createdDevice);
    }
    
    // ... Diğer metotlar
    
    private DeviceDto MapToDeviceDto(Device device)
    {
        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Ip = device.Ip,
            Port = device.Port,
            Latitude = device.Latitude,
            Longitude = device.Longitude,
            Platform = device.Platform != null ? new PlatformDto
            {
                Id = device.Platform.Id,
                Name = device.Platform.Name
            } : null
        };
    }
}
```

### Business Layer Extension Örneği

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Service registrations
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}
```

## DataAccess Katmanı

DataAccess katmanı, veritabanı işlemlerini gerçekleştirir ve veritabanı ile uygulama arasındaki etkileşimi yönetir.

### Temel Bileşenler

- **Context**: Entity Framework Core DbContext sınıfları.
- **Repositories/Interfaces**: Repository arayüzleri.
- **Repositories/Concrete**: Repository implementasyonları.
- **Configurations**: Entity yapılandırmaları.
- **Seeding**: Veritabanı seed işlemleri.

### Örnek Repository Arayüzü

```csharp
public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync(Expression<Func<Device, object>> include = null);
    Task<Device> GetByIdAsync(int id, Expression<Func<Device, object>> include = null);
    Task<bool> AnyAsync(Expression<Func<Device, bool>> predicate);
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(Device device);
    // ... Diğer metotlar
}
```

### Örnek Repository İmplementasyonu

```csharp
public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _context;

    public DeviceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetAllAsync(Expression<Func<Device, object>> include = null)
    {
        IQueryable<Device> query = _context.Devices;
        
        if (include != null)
        {
            query = query.Include(include);
        }
        
        return await query.AsNoTracking().ToListAsync();
    }
    
    public async Task<Device> GetByIdAsync(int id, Expression<Func<Device, object>> include = null)
    {
        IQueryable<Device> query = _context.Devices;
        
        if (include != null)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(d => d.Id == id);
    }
    
    public async Task<bool> AnyAsync(Expression<Func<Device, bool>> predicate)
    {
        return await _context.Devices.AnyAsync(predicate);
    }
    
    public async Task AddAsync(Device device)
    {
        await _context.Devices.AddAsync(device);
        await _context.SaveChangesAsync();
    }
    
    // ... Diğer metotlar
}
```

### DataAccess Layer Extension Örneği

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Repository registrations
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IStationRepository, StationRepository>();
        
        return services;
    }
}
```

## Entity Katmanı

Entity katmanı, veritabanında bulunan tabloları temsil eden sınıfları içerir. Bu katman, diğer katmanlardan daha az değişen ve en kararlı katmandır.

### Temel Bileşenler

- **Concrete**: Veritabanı tablolarını temsil eden entity sınıfları.
- **DTOs**: Katmanlar arası veri taşıma görevini üstlenen Data Transfer Object sınıfları.

### Entity Sınıfları

DeviceApi'de şu temel entity sınıfları bulunmaktadır:

1. **Device (Cihaz)**: Sistemdeki cihazları temsil eder.
```csharp
public class Device
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Ip { get; set; }
    
    [Required]
    public int Port { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    [Required]
    public int PlatformId { get; set; }
    
    [ForeignKey("PlatformId")]
    public Platform Platform { get; set; }
}
```

2. **Platform**: Cihazların bağlı olduğu platformları temsil eder.
```csharp
public class Platform
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    public ICollection<Device> Devices { get; set; }
}
```

3. **Station (İstasyon)**: İstasyonları temsil eden entity.
```csharp
public class Station
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}
```

### DTO Örnekleri

DTO sınıfları, entity sınıflarının API katmanı ile Business katmanı arasında taşınmasını sağlayan veri modellerini içerir.

```csharp
// Device ilgili DTO'lar
public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public PlatformDto Platform { get; set; }
}

public class CreateDeviceRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Geçerli bir IP adresi giriniz")]
    public string Ip { get; set; }
    
    [Required]
    [Range(1, 65535)]
    public int Port { get; set; }
    
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Range(-180, 180)]
    public double Longitude { get; set; }
    
    [Required]
    public int PlatformId { get; set; }
}

public class UpdateDeviceRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Geçerli bir IP adresi giriniz")]
    public string Ip { get; set; }
    
    [Required]
    [Range(1, 65535)]
    public int Port { get; set; }
    
    [Range(-90, 90)]
    public double Latitude { get; set; }
    
    [Range(-180, 180)]
    public double Longitude { get; set; }
    
    [Required]
    public int PlatformId { get; set; }
}
```

## Core Katmanı

Core katmanı, tüm katmanlar tarafından kullanılabilecek ortak işlevselliği sağlar.

### Temel Bileşenler

- **Security**: Güvenlik işlemleri için sınıflar (JWT, şifreleme vb.).
- **Utilities**: Yardımcı sınıflar ve metotlar.
- **Extensions**: Extension metotları.
- **Logging**: Loglama altyapısı.

### Örnek Utility Sınıfı

```csharp
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public IDictionary<string, List<string>> Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Error(IDictionary<string, List<string>> errors, string message, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Errors = errors,
            Message = message
        };
    }
}
```

## Uygulama Başlatma Süreci

Uygulamanın başlatılması `Program.cs` dosyasında gerçekleşir. Bu dosya, tüm katmanların servislerini ve yapılandırmalarını kaydetmek için extension metotlarını kullanır.

```csharp
var builder = WebApplication.CreateBuilder(args);

// API layer servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Business layer servisleri
builder.Services.AddBusinessServices();

// DataAccess layer servisleri
builder.Services.AddDataAccessServices(builder.Configuration);

// Core layer servisleri
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimitingServices(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestResponseLogging();

app.MapControllers().RequireRateLimiting("ip");

app.Run();
```

## Bağımlılık Enjeksiyonu

N-Tier mimaride, katmanlar arası bağımlılıklar Dependency Injection (DI) pattern ile yönetilir. Her katmanın kendi extension metodu, kendi servislerini DI container'a kaydeder.

### Bağımlılık Enjeksiyonu Yaşam Döngüsü Türleri

DeviceApi, aşağıdaki DI yaşam döngüsü türlerini kullanır:

1. **Singleton**: Uygulama yaşam döngüsü boyunca tek bir örnek oluşturulur
   - Örnek: `IMongoClient`, `ILogRepository`

2. **Scoped**: HTTP isteği başına bir örnek oluşturulur
   - Örnek: `IDeviceService`, `IPlatformService`, `IDeviceRepository`, `AppDbContext`

3. **Transient**: Her ihtiyaç duyulduğunda yeni bir örnek oluşturulur
   - Örnek: Durum tutmayan utility sınıfları

## Middleware Yapısı

DeviceApi, HTTP istek/yanıt pipeline'ını yapılandırmak için çeşitli middleware'ler kullanır. Bu middleware'ler, `Program.cs` dosyasında aşağıdaki sırada yapılandırılır:

1. Exception Handling
2. HTTPS Redirection
3. Rate Limiting
4. Static Files
5. Cookie Policy
6. Authentication
7. Authorization
8. Request/Response Logging

### RequestResponseLoggingMiddleware

Bu middleware, HTTP isteklerini ve yanıtlarını loglamak için kullanılır:

```csharp
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    private readonly string[] _excludedPaths;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        _excludedPaths = configuration.GetSection("LogSettings:ExcludedPaths").Get<string[]>() ?? Array.Empty<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Belirli endpoint'ler loglama dışında bırakılabilir
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var log = new RequestResponseLog
        {
            Path = context.Request.Path,
            HttpMethod = context.Request.Method,
            // ... diğer log özellikleri
        };

        // İstek gövdesini yakala
        // ... istek loglama kodu

        // Yanıt gövdesini yakala
        // ... yanıt loglama kodu

        // Logu kaydet
        // ... log kaydetme kodu
    }
}
```

## Extension Metotları

N-Tier mimaride, extension metotları yaygın olarak kullanılır. Bu metotlar, katmanlar arasındaki modülerliği ve temiz kod yapısını destekler.

### API Extension Metotları

`Deneme.API.Extensions` namespace'inde, API katmanına özgü extension metotları bulunur:

```csharp
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
```

### Logging Extension Metotları

`Deneme.Extensions` namespace'inde, loglama ile ilgili extension metotları bulunur:

```csharp
public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        // Log repositories
        services.AddSingleton<ILogRepository, MongoLogRepository>();
        
        // Log services
        services.AddSingleton<IApiLogService, ApiLogService>();
        
        // HTTP context accessor for accessing request context in service layer
        services.AddHttpContextAccessor();
        
        return services;
    }
    
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        
        return app;
    }
}
```

### Core Extension Metotları

`Deneme.Core.Extensions` namespace'inde, çekirdek fonksiyonlar için extension metotları bulunur:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT doğrulama yapılandırması
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // JWT options yapılandırma
        });
        
        return services;
    }
    
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Rate limiting yapılandırma
        return services;
    }
}
```

---

Bu dokümantasyon, N-Tier mimarisi hakkında genel bir bakış sağlar ve DeviceApi'nin mimari yapısını, katmanlarını ve bileşenlerini açıklar. Geliştiriciler, bu yapıyı anlayarak uygulamayı daha kolay genişletebilir ve bakımını yapabilir. 