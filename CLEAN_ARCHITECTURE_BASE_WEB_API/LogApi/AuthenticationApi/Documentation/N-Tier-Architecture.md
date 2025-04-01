# N-Tier Mimari Dokümantasyonu

Bu doküman, Deneme API'nin N-Tier (Çok Katmanlı) mimari yapısını açıklar.

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

Deneme API, modern bir yazılım geliştirme yaklaşımı olan N-Tier (Çok Katmanlı) mimari üzerine inşa edilmiştir. Bu mimari, uygulamayı mantıksal ve fiziksel olarak birbirinden ayrılmış katmanlara bölerek, daha modüler, test edilebilir, bakımı daha kolay ve ölçeklenebilir hale getirir.

N-Tier mimari, her katmanın belirli bir sorumluluğu olduğu ve sadece kendinden bir alt katmanla iletişim kurabildiği bir yapıdır. Bu sayede, herhangi bir katmanda yapılan değişiklikler diğer katmanları minimum etkileyecektir.

## Projenin Katmanlı Yapısı

Deneme API projesi, klasör bazlı N-Tier mimarisi kullanılarak aşağıdaki katmanlardan oluşmaktadır:

```
deneme/
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
Deneme.API.*                  # API katmanı namespace'leri
Deneme.Business.*             # Business katmanı namespace'leri
Deneme.DataAccess.*           # DataAccess katmanı namespace'leri
Deneme.Entities.*             # Entity katmanı namespace'leri
Deneme.Core.*                 # Core katmanı namespace'leri
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
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<List<UserDto>>.Success(users, "Kullanıcılar başarıyla getirildi"));
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
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    // ... Diğer metotlar
}
```

### Örnek Servis İmplementasyonu

```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToUserDto).ToList();
    }
    
    // ... Diğer metotlar
}
```

### Business Layer Extension Örneği

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Service registrations
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        
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
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task AddUserAsync(User user);
    // ... Diğer metotlar
}
```

### Örnek Repository İmplementasyonu

```csharp
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .AsNoTracking()
            .ToListAsync();
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
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}
```

## Entity Katmanı

Entity katmanı, veritabanı varlıklarını ve veri transfer objelerini (DTO) içerir.

### Temel Bileşenler

- **Concrete**: Entity Framework entity sınıfları.
- **DTOs**: Veri transfer objeleri.

### Örnek Entity

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    // Navigasyon özellikleri
    public int UserRoleId { get; set; }
    public UserRole UserRole { get; set; }
}
```

### Örnek DTO

```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public RoleDto Role { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public TwoFactorDto TwoFactor { get; set; }
    public ProfilePictureDto ProfilePicture { get; set; }
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

### Core Layer Extension Örneği

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB için client
        services.AddSingleton<IMongoClient>(_ => 
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        });
        
        // HttpContextAccessor ekle
        services.AddHttpContextAccessor();
        
        // Loglama servisleri
        services.AddLoggingServices();
        
        return services;
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

Deneme API, aşağıdaki DI yaşam döngüsü türlerini kullanır:

1. **Singleton**: Uygulama yaşam döngüsü boyunca tek bir örnek oluşturulur
   - Örnek: `IMongoClient`, `ILogRepository`

2. **Scoped**: HTTP isteği başına bir örnek oluşturulur
   - Örnek: `IUserService`, `IUserRepository`, `AppDbContext`

3. **Transient**: Her ihtiyaç duyulduğunda yeni bir örnek oluşturulur
   - Örnek: Durum tutmayan utility sınıfları

## Middleware Yapısı

Deneme API, HTTP istek/yanıt pipeline'ını yapılandırmak için çeşitli middleware'ler kullanır. Bu middleware'ler, `Program.cs` dosyasında aşağıdaki sırada yapılandırılır:

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
    // ... constructor ve field'lar

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

Bu dokümantasyon, N-Tier mimarisi hakkında genel bir bakış sağlar ve Deneme API'nin mimari yapısını, katmanlarını ve bileşenlerini açıklar. Geliştiriciler, bu yapıyı anlayarak uygulamayı daha kolay genişletebilir ve bakımını yapabilir. 