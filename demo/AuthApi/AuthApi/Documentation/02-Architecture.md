# 02 - Mimari Yapı

## N-Tier Mimari

AuthApi projesi, N-Tier (çok katmanlı) mimarisi üzerine inşa edilmiştir. Bu mimari yaklaşım, uygulamanın farklı sorumluluklarını katmanlara ayırarak modüler, bakımı kolay ve test edilebilir bir yapı sunmaktadır.

### Katmanlar ve Sorumlulukları

1. **API Katmanı (Sunum Katmanı)**
   - Dış dünya ile iletişimi yönetir
   - HTTP isteklerini alır ve yanıtları döndürür
   - Rota tanımlarını içerir
   - İstek doğrulamasını gerçekleştirir
   - Middleware bileşenlerini yönetir

2. **Business Katmanı (İş Mantığı Katmanı)**
   - Uygulamanın iş kurallarını içerir
   - Verilerin işlenmesini yönetir
   - Doğrulama işlemlerini gerçekleştirir
   - Servisler aracılığıyla erişim sağlar

3. **Data Katmanı (Veri Erişim Katmanı)**
   - Veritabanı işlemlerini gerçekleştirir
   - ORM (Entity Framework Core) kullanır
   - Repository pattern uygular
   - Veritabanı modellerini yapılandırır

4. **Core Katmanı (Çekirdek Katmanı)**
   - Tüm katmanlar tarafından kullanılan altyapıyı sağlar
   - Güvenlik, yardımcı sınıflar, sabitler içerir
   - Extension metotlarını barındırır

5. **Entities Katmanı (Varlık Katmanı)**
   - Veritabanı varlıklarını tanımlar
   - DTO'ları (Data Transfer Objects) içerir

## Bağımlılık Enjeksiyonu ve IoC

Proje, bağımlılık enjeksiyonu prensiplerine dayanmaktadır. .NET Core'un yerleşik IoC (Inversion of Control) konteyneri kullanılarak bağımlılıklar yönetilmektedir.

### Extension Metotları ile Servis Kayıtları

Her katman, kendi servislerini kaydetmek için extension metodları sunar:

```csharp
// Program.cs'den örnek
// DataAccess Layer servisleri
builder.Services.AddDataAccessServices(builder.Configuration);

// Business Layer servisleri
builder.Services.AddBusinessServices();

// Core layer servisleri
builder.Services.AddCoreServices(builder.Configuration);

// LogLibrary servislerini ekle
builder.Services.AddLogLibrary(builder.Configuration);
```

## Repository Pattern

Veri erişim katmanı, Repository Pattern kullanılarak tasarlanmıştır. Bu model, veritabanı işlemlerini soyutlayarak:

- İş mantığı katmanının veritabanı detaylarını bilmesini engeller
- Tekrar kullanılabilir veri erişim kodu sağlar
- Test edilebilirliği artırır
- Veritabanı değişikliklerinin uygulamanın geri kalanını etkilemesini önler

```csharp
// Repository arayüzü örneği
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
}
```

## Servis Katmanı

Business layer, iş mantığını servisler aracılığıyla yönetir. Her servis, belirli bir işlevsellikten sorumludur:

- **AuthService**: Kimlik doğrulama işlemlerini yönetir
- **UserService**: Kullanıcı işlemlerini yönetir
- **EmailService**: E-posta gönderim işlemlerini yönetir

Servisler, soyutlama sağlamak için arayüzlerle tanımlanır:

```csharp
public interface IAuthService
{
    Task<object> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<AuthResponse> VerifyTwoFactorAsync(TwoFactorVerifyRequest request);
    // Diğer metotlar...
}
```

## API Response Standardizasyonu

Tüm API yanıtları, tutarlı bir yapıda olacak şekilde standardize edilmiştir:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
    
    // Static factory metotları...
    public static ApiResponse<T> Success(T data, string message = null, int statusCode = 200) { /* ... */ }
    public static ApiResponse<T> Error(string message, int statusCode = 400) { /* ... */ }
    // Diğer metotlar...
}
```

## Middleware Kullanımı

Uygulamada çeşitli middleware bileşenleri kullanılmaktadır:

- **Exception Handling Middleware**: Hataları yakalar ve API yanıt formatına dönüştürür
- **Authentication Middleware**: JWT token doğrulama işlemlerini gerçekleştirir
- **Authorization Middleware**: Rol tabanlı erişim kontrolünü sağlar
- **Rate Limiting Middleware**: İstek sınırlamalarını uygular

## Yapılandırma Yönetimi

Uygulama ayarları `appsettings.json` dosyasında tutulur ve farklı ortamlar için (`Development`, `Production` gibi) özelleştirilebilir:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DenemeApiDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "MongoDb": "mongodb://localhost:27017"
  },
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
  },
  // Diğer ayarlar...
}
``` 