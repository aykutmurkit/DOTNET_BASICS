# DeviceAPI - Mimari İncelemesi ve Kullanım Kuralları

## Genel Bakış

DeviceAPI, cihaz yönetimi için geliştirilmiş modern bir .NET Core tabanlı RESTful API'dir. Bu API, elektronik cihazların uzaktan izlenmesi, yönetilmesi ve kontrolü için kapsamlı bir altyapı sunar. Aynı zamanda TCP protokolü üzerinden cihazlarla doğrudan iletişim kurma yeteneğine de sahiptir. Clean Architecture, Domain-Driven Design ve SOLID prensiplerine uygun olarak geliştirilmiştir.

## API Yanıt Yapısı ve Hata Yönetimi

DeviceAPI, tutarlı ve standart bir API yanıt yapısı sunar. Bu yapı, istemcilerin API ile etkileşimini kolaylaştırır ve hata durumlarını anlamlı bir şekilde yönetir.

### 1. Standart API Yanıt Yapısı (ApiResponse<T>)

Tüm API yanıtları, `ApiResponse<T>` sınıfı ile sarmalanmıştır. Bu sınıf, aşağıdaki özelliklere sahiptir:

```csharp
public class ApiResponse<T>
{
    public int StatusCode { get; set; }        // HTTP durum kodu
    public bool IsSuccess { get; set; }        // İşlemin başarılı olup olmadığı
    public T Data { get; set; }                // Dönen veri (başarılı yanıtlarda)
    public Dictionary<string, List<string>> Errors { get; set; }  // Hata detayları
    public string Message { get; set; }        // Bilgi/hata mesajı
}
```

Bu yapı sayesinde:
- İstemciler tek bir tutarlı yapıda yanıt alır
- Başarı/başarısızlık durumu açıkça belirtilir 
- Hata durumunda detaylı hata mesajları sağlanır
- HTTP durum kodları API yanıtında da belirtilir

### 2. API Yanıt Türleri

`ApiResponse<T>` sınıfı, aşağıdaki static factory metodları ile farklı yanıt türleri oluşturmayı sağlar:

#### Başarı Yanıtları
- **Success:** Standart başarılı yanıt (200 OK)
  ```csharp
  ApiResponse<DeviceDto>.Success(device, "Cihaz bilgileri başarıyla alındı");
  ```

- **Created:** Kaynak oluşturma başarılı yanıtı (201 Created)
  ```csharp
  ApiResponse<DeviceDto>.Created(newDevice, "Cihaz başarıyla oluşturuldu");
  ```

- **NoContent:** İçerik döndürmeyen başarılı yanıt (204 No Content)
  ```csharp
  ApiResponse<object>.NoContent("Cihaz başarıyla silindi");
  ```

#### Hata Yanıtları
- **Error:** Genel hata yanıtı (400 Bad Request)
  ```csharp
  ApiResponse<DeviceDto>.Error("Geçersiz cihaz ID'si");
  ```

- **Error (Validation):** Doğrulama hatası yanıtı
  ```csharp
  ApiResponse<DeviceDto>.Error(validationErrors, "Lütfen form alanlarını kontrol ediniz");
  ```

- **Unauthorized:** Kimlik doğrulama hatası (401 Unauthorized)
  ```csharp
  ApiResponse<DeviceDto>.Unauthorized("Bu işlem için giriş yapmanız gerekiyor");
  ```

- **Forbidden:** Yetki hatası (403 Forbidden)
  ```csharp
  ApiResponse<DeviceDto>.Forbidden("Bu işlem için yetkiniz bulunmuyor");
  ```

- **NotFound:** Kaynak bulunamadı hatası (404 Not Found)
  ```csharp
  ApiResponse<DeviceDto>.NotFound("Aradığınız cihaz bulunamadı");
  ```

- **Conflict:** Çakışma hatası (409 Conflict)
  ```csharp
  ApiResponse<DeviceDto>.Conflict("Bu IMEI numarası zaten kullanılıyor");
  ```

- **ServerError:** Sunucu hatası (500 Internal Server Error)
  ```csharp
  ApiResponse<DeviceDto>.ServerError("Sunucu hatası oluştu");
  ```

### 3. Controller Extension Metodları

Controller'lar için tanımlanan extension metodları, API yanıtlarını oluşturma işlemini daha da basitleştirir:

```csharp
// 201 Created yanıtı
return controller.CreatedResponse(newDevice, "Cihaz başarıyla oluşturuldu");

// 404 Not Found yanıtı
return controller.NotFoundResponse<DeviceDto>("Cihaz bulunamadı");

// 400 Bad Request yanıtı
return controller.BadRequestResponse<DeviceDto>("Geçersiz istek");
```

Bu extension metodları, uygun HTTP durum kodlarını otomatik olarak ayarlar ve tutarlı yanıt formatını korur.

### 4. Hata Yönetimi Mekanizmaları

DeviceAPI'de hata yönetimi, çeşitli seviyelerden oluşan bir yaklaşım kullanır:

#### a) Validasyon Filtresi
`ValidationFilter` sınıfı, model validasyon hatalarını otomatik olarak yakalar ve standart bir API yanıtına dönüştürür:

```csharp
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

            var response = ApiResponse<object>.Error(errors);
            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}
```

Bu filtre, Program.cs içinde tüm controller'lar için global olarak kaydedilmiştir:

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
```

#### b) Model Validasyon Davranışı
API davranış seçenekleri özelleştirilmiştir:

```csharp
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
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

        return new BadRequestObjectResult(response);
    };
});
```

#### c) Exception Yönetimi
Servis katmanında, domain-specific exception'lar fırlatılır ve bu exception'lar controller'larda yakalanarak uygun API yanıtlarına dönüştürülür:

```csharp
// Servis katmanında
public async Task<DeviceDto> GetDeviceAsync(int id)
{
    var device = await _deviceRepository.GetDeviceByIdAsync(id);
    if (device == null)
        throw new Exception("Cihaz bulunamadı");
        
    return _mapper.Map<DeviceDto>(device);
}

// Controller katmanında
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    try
    {
        var device = await _deviceService.GetDeviceAsync(id);
        return Ok(ApiResponse<DeviceDto>.Success(device));
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("bulunamadı"))
        {
            return NotFound(ApiResponse<DeviceDto>.NotFound(ex.Message));
        }
        
        return BadRequest(ApiResponse<DeviceDto>.Error(ex.Message));
    }
}
```

#### d) LogLibrary Entegrasyonu
Tüm hatalar, LogLibrary kullanılarak uygun şekilde loglanır:

```csharp
catch (Exception ex)
{
    await _logService.LogErrorAsync(
        ex.Message,
        "DeviceController.GetDevice",
        ex,
        new { DeviceId = id, UserId = userId }
    );
    
    // İstemciye uygun yanıt döndür
    // ...
}
```

### 5. Yaygın Hata Durumları ve Çözümleri

#### Validation Hataları
- **Hata:** Girdi doğrulama hataları
- **HTTP Kodu:** 400 Bad Request
- **Çözüm:** İstemci, hata mesajlarında belirtilen alanları düzeltmelidir

#### Kaynak Bulunamadı Hataları
- **Hata:** İstenen kaynak veritabanında mevcut değil
- **HTTP Kodu:** 404 Not Found
- **Çözüm:** İstemci, geçerli bir ID veya farklı filtreleme parametreleri kullanmalıdır

#### Yetkilendirme Hataları
- **Hata:** Kullanıcı gerekli izinlere sahip değil
- **HTTP Kodu:** 403 Forbidden
- **Çözüm:** İstemci, yeterli yetkiye sahip bir kullanıcı ile işlem yapmalıdır

#### Çakışma Hataları
- **Hata:** Veri bütünlüğü kısıtlamaları ihlali (örn. benzersiz alan)
- **HTTP Kodu:** 409 Conflict
- **Çözüm:** İstemci, benzersiz değerler içeren veri göndermelidir (örn. farklı IMEI)

#### Sunucu Hataları
- **Hata:** Sunucu tarafında oluşan beklenmeyen hatalar
- **HTTP Kodu:** 500 Internal Server Error
- **Çözüm:** Sunucu logları incelenmeli ve altta yatan sorun çözülmelidir

### 6. İyi Uygulamalar

1. **Tutarlı Hata Mesajları:** Hata mesajları açık, anlaşılır ve tutarlı olmalıdır. Kullanıcı dostu formatta olmalıdır.

2. **HTTP Durum Kodlarının Doğru Kullanımı:** Her yanıt, uygun HTTP durum kodunu içermelidir.

3. **Hassas Bilgileri Gizleme:** Hata yanıtlarında iç sistem detayları, stack trace'ler veya hassas bilgiler açıklanmamalıdır.

4. **İşlem Kimliği (Transaction ID):** Karmaşık işlemlerde, takip edilebilirlik için benzersiz işlem kimlikleri kullanılmalıdır.

5. **Ayrıntılı Loglama:** Tüm hatalar, uygun log seviyeleri ile loglanmalıdır (Debug, Info, Warning, Error, Critical).

### 7. Örnek API Yanıtları

#### Başarılı Yanıt (200 OK)
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "Yusufpaşa Cihaz 1",
    "ip": "192.168.1.11",
    "port": 8001,
    "imei": "356158061391111",
    "latitude": 37.7668,
    "longitude": 29.0975,
    "platformId": 1,
    "platformName": "Yusufpaşa - 1. Peron"
  },
  "message": "Cihaz bilgileri başarıyla alındı"
}
```

#### Doğrulama Hatası (400 Bad Request)
```json
{
  "statusCode": 400,
  "isSuccess": false,
  "errors": {
    "Name": ["Cihaz adı boş olamaz"],
    "IMEI": ["IMEI numarası 15 karakter olmalıdır"],
    "IP": ["Geçerli bir IP adresi giriniz"]
  },
  "message": "Lütfen form alanlarını kontrol ediniz"
}
```

#### Kaynak Bulunamadı (404 Not Found)
```json
{
  "statusCode": 404,
  "isSuccess": false,
  "message": "Cihaz bulunamadı"
}
```

## Mimari Yapı

DeviceAPI, katmanlı bir mimari kullanarak sorumlulukların net bir şekilde ayrılmasını sağlar. Sistem, aşağıdaki ana bileşenlerden oluşmaktadır:

### 1. API Katmanı
Bu katman, dış dünya ile iletişimi sağlayan REST endpoint'lerini içerir.

**Controllers**: API'nin tüm endpoint'leri bu klasörde tanımlanmıştır.
- `AlignmentTypesController.cs`: Metin hizalama türlerini yönetir
- `BitmapScreenMessagesController.cs`: Bitmap ekran mesajlarını yönetir
- `DeviceSettingsController.cs`: Cihaz ayarlarını yönetir
- `DeviceStatusesController.cs`: Cihaz durumlarını izler ve yönetir
- `DevicesController.cs`: Cihaz varlıklarını yönetir (ekleme, silme, güncelleme)
- `FontTypesController.cs`: Yazı tiplerini yönetir
- `FullScreenMessagesController.cs`: Tam ekran mesajlarını yönetir
- `PeriodicMessagesController.cs`: Periyodik mesajları yönetir
- `PlatformsController.cs`: Platform bilgilerini yönetir
- `PredictionsController.cs`: Tahmin verilerini yönetir
- `ScrollingScreenMessagesController.cs`: Kayan ekran mesajlarını yönetir
- `StationsController.cs`: İstasyon bilgilerini yönetir
- `TcpListenerController.cs`: TCP dinleyici ile ilgili işlemleri yönetir

**Models**: API'nin dış dünyaya açtığı veri yapılarını (DTO'lar) içerir.
- `EnumDto.cs`: Enum değerlerini temsil eden DTO
- **Auth Klasörü**: Kimlik doğrulama ile ilgili model sınıfları

**Extensions**: API katmanına özgü extension metodlarını içerir.
- Controller extension'ları
- API davranış yapılandırmaları
- Response modelleri için extension metodları

### 2. Business Katmanı
İş mantığının uygulandığı katmandır.

**Services**: İş mantığı servislerini içerir.
- Her bir varlık için özel servisler
- Veri işleme ve doğrulama mantığı
- İş kurallarının uygulanması
- Çapraz kesişen iş kuralları (cross-cutting concerns)

**Mappings**: Veri dönüşümlerini yönetir.
- AutoMapper profilleri
- DTO-Entity dönüşümleri
- Özel dönüşüm kuralları

**Extensions**: Business katmanına özgü extension metodları.
- Servis kayıt metodları
- AutoMapper yapılandırmaları
- Validation yapılandırmaları

### 3. DataAccess Katmanı
Veritabanı işlemlerini yönetir.

**Context**: Entity Framework DbContext sınıfları.
- `AppDbContext.cs`: Ana veritabanı bağlam sınıfı

**Repositories**: Repository pattern uygulaması.
- Generic Repository
- Özel Repository uygulamaları
- Veri erişim metodları

**Configurations**: Entity konfigürasyonları.
- Entity Framework Fluent API yapılandırmaları
- İlişki tanımlamaları
- İndeks ve kısıtlamalar

**Seeding**: Başlangıç verilerini yönetir.
- `DatabaseSeeder.cs`: Veritabanı seed işlemleri

**Interfaces**: Repository ve UnitOfWork arayüzleri.
- IRepository
- IUnitOfWork
- Özel repository arayüzleri

**Extensions**: DataAccess katmanı için extension metodları.
- Servis kayıt metodları
- DbContext yapılandırmaları

### 4. Core Katmanı
Tüm katmanlar tarafından kullanılan ortak bileşenleri içerir.

**Models**: Temel model sınıfları.
- Temel varlık sınıfı
- Ortak özellikler
- Değer nesneleri (Value Objects)

**Interfaces**: Çekirdek arayüzler.
- Servis arayüzleri
- Entity arayüzleri
- İş mantığı arayüzleri

**Utilities**: Yardımcı sınıflar ve metodlar.
- `ApiResponse<T>`: Standart API yanıt yapısı
- Hata işleme yardımcıları
- Tarih/zaman yardımcıları
- String işleme yardımcıları

**Extensions**: Core katmanı için extension metodları.
- Servis kayıt metodları
- String/Date/Collection extension'ları

### 5. TCPListener Katmanı
Cihazlarla TCP protokolü üzerinden iletişim kurmayı sağlar.

**Services**: TCP dinleyici servisleri.
- `TcpListenerService.cs`: TCP bağlantılarını yönetir

**Communication**: İletişim protokollerini yönetir.
- Mesaj formatlama
- Protokol kuralları
- Veri alışverişi

**Models**: TCP iletişiminde kullanılan model sınıfları.
- TCP mesaj modelleri
- Cihaz komut modelleri
- Durum ve olay modelleri

**Core**: TCP işlemleri için çekirdek bileşenler.
- Bağlantı yöneticileri
- Mesaj işlemciler
- Hata yönetimi

**Utils**: TCP işlemleri için yardımcı sınıflar.
- Mesaj ayrıştırıcılar
- Doğrulama yardımcıları
- Log yardımcıları

**Extensions**: TCP katmanı için extension metodları.
- Servis kayıt metodları
- TCP dinleyici yapılandırmaları

### 6. Entities Katmanı
Veri tabanı varlıklarını tanımlar.

- Cihaz varlıkları
- Durum ve ayar varlıkları
- İlişki varlıkları
- Mesaj ve bildirim varlıkları

## Kullanılan Tasarım Desenleri

### 1. Repository Pattern
- Veri erişimi soyutlama
- Generic Repository uygulaması
- Özel Repository'ler ile genişletilebilirlik

### 2. Unit of Work Pattern
- Transaksiyonel veri işlemleri
- Tutarlı veri yönetimi
- İlişkili repository'lerin koordinasyonu

### 3. Dependency Injection
- Tüm katmanlarda bağımlılıkların enjekte edilmesi
- Service Locator kullanımından kaçınma
- Testlenebilirliği artırma

### 4. Mediator Pattern (MediatR)
- Servisler arası iletişimi düzenleme
- Command ve Query işlemlerini ayırma
- Pipeline davranışları (validation, caching, logging)

### 5. Options Pattern
- Uygulama yapılandırması için standart yaklaşım
- Strongly-typed yapılandırma nesneleri
- Bağımlılık enjeksiyonu ile kolay erişim

### 6. Builder Pattern
- Fluent API tasarımı
- Karmaşık nesnelerin oluşturulması
- Okunabilir ve bakımı kolay kod

### 7. Specification Pattern
- Karmaşık sorguların kapsüllenmesi
- Sorgu mantığının ayrılması
- Yeniden kullanılabilir filtreleme kriterleri

### 8. Factory Pattern
- Kompleks nesnelerin oluşturulması
- Nesne oluşturma mantığının merkezileştirilmesi
- Concrete sınıflardan bağımsızlık

## Veritabanı Seed İşlemleri

DeviceAPI, veritabanını başlangıç verileriyle doldurmak için kapsamlı ve esnek bir seed mekanizması kullanır. Bu mekanizma, uygulamanın ilk çalıştırılması sırasında temel verilerin otomatik olarak oluşturulmasını sağlar.

### 1. Seed Mimarisi

Seed işlemleri, aşağıdaki temel bileşenlerden oluşur:

#### **ISeeder Arayüzü**
```csharp
public interface ISeeder
{
    int Order { get; }
    Task SeedAsync(AppDbContext context);
}
```

- `Order`: Seeder'ın çalışma sırasını belirler. Düşük sayılar önce çalışır.
- `SeedAsync`: Veritabanı bağlantı context'ini parametre olarak alır ve asenkron olarak seed işlemini gerçekleştirir.

#### **SeederOrder Enum**
Seed işlemlerinin çalışma sırasını merkezi olarak yönetir:

```csharp
public enum SeederOrder
{
    UserRoles = 1,        // Temel kullanıcı rolleri
    Users = 2,            // Kullanıcılar
    Stations = 3,         // İstasyonlar
    Platforms = 4,        // Platformlar
    AlignmentTypes = 5,   // Metin hizalama türleri
    FontTypes = 6,        // Yazı tipleri
    Devices = 7,          // Cihazlar
    DeviceSettings = 8,   // Cihaz ayarları
    DeviceStatuses = 9,   // Cihaz durumları
    Predictions = 10,     // Tahminler
    FullScreenMessages = 11,      // Tam ekran mesajları
    ScrollingScreenMessages = 12, // Kayan ekran mesajları
    BitmapScreenMessages = 13,    // Bitmap ekran mesajları
    PeriodicMessages = 14         // Periyodik mesajlar
}
```

Bu enum, bağımlılık ilişkilerine göre verilerin doğru sırada eklenebilmesini sağlar. Örneğin, cihazları ekleyebilmek için önce platformların eklenmesi gerekir.

#### **DatabaseSeeder Sınıfı**
Tüm seed işlemlerini koordine eden ana sınıftır:

```csharp
public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly ILogService _logService;

    public DatabaseSeeder(
        IServiceProvider serviceProvider, 
        ILogger<DatabaseSeeder> logger,
        ILogService logService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _logService = logService;
    }

    public async Task SeedAsync()
    {
        // ISeeder arayüzünü implement eden tüm sınıfları bulur
        var seeders = GetSeeders();

        // Seed işlemlerini sırayla çalıştırır
        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            await seeder.SeedAsync(context);
        }
    }

    private List<ISeeder> GetSeeders()
    {
        // Reflection kullanarak ISeeder implementasyonlarını bulur
    }
}
```

### 2. Seed İşlem Akışı

1. Uygulama başlatıldığında, Program.cs içindeki konfigürasyona göre veritabanı sıfırlanıp sıfırlanmayacağı kontrol edilir:
   ```csharp
   if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
   {
       using (var scope = app.Services.CreateScope())
       {
           var context = serviceProvider.GetRequiredService<AppDbContext>();
           var seeder = serviceProvider.GetRequiredService<DatabaseSeeder>();
            
           await context.Database.EnsureDeletedAsync(); // Veritabanını sil
           await context.Database.EnsureCreatedAsync(); // Veritabanını oluştur
            
           await seeder.SeedAsync(); // Seed işlemlerini başlat
       }
   }
   ```

2. `DatabaseSeeder` sınıfı, reflection ile `ISeeder` arayüzünü implement eden tüm sınıfları bulur.

3. Bulunan seeder'lar `Order` özelliklerine göre sıralanır ve sırayla çalıştırılır.

4. Her seeder, veritabanında ilgili verilerin zaten olup olmadığını kontrol eder. Varsa, işlemi atlar.

5. Seeder'lar genellikle SQL komutları oluşturur ve `ExecuteSqlRaw` metoduyla çalıştırır. Bu, büyük miktarda veri eklemek için performans avantajı sağlar.

### 3. Özel Seeder Sınıfları

Her entity türü için özel bir seeder sınıfı bulunur:

- **DeviceSeeder**: Cihaz verilerini ekler
- **StationSeeder**: İstasyon verilerini ekler
- **PlatformSeeder**: Platform verilerini ekler
- **FontTypeSeeder**: Yazı tipi verilerini ekler
- **AlignmentTypeSeeder**: Metin hizalama türlerini ekler
- **DeviceSettingsSeeder**: Cihaz ayarlarını ekler
- **DeviceStatusSeeder**: Cihaz durumlarını ekler
- **PredictionSeeder**: Tahmin verilerini ekler
- **FullScreenMessageSeeder**: Tam ekran mesajlarını ekler
- **ScrollingScreenMessageSeeder**: Kayan ekran mesajlarını ekler
- **BitmapScreenMessageSeeder**: Bitmap ekran mesajlarını ekler
- **PeriodicMessageSeeder**: Periyodik mesajları ekler

### 4. Örnek Seeder Uygulaması

DeviceSeeder sınıfı örneği:

```csharp
public class DeviceSeeder : ISeeder
{
    public int Order => (int)SeederOrder.Devices; // 7
    
    public async Task SeedAsync(AppDbContext context)
    {
        // Veritabanında zaten cihaz var mı kontrol et
        if (await context.Devices.AnyAsync())
            return; // Zaten varsa işlem yapma

        // Bağımlı verileri kontrol et
        var platforms = await context.Platforms.ToListAsync();
        if (!platforms.Any())
            throw new Exception("Platformlar bulunamadı. Önce PlatformSeeder çalıştırılmalıdır.");

        // SQL sorgusu oluştur
        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] ON;");
        queryBuilder.AppendLine("INSERT INTO [Devices] ([Id], [Name], [Ip], [Port], [IMEI], [Latitude], [Longitude], [PlatformId]) VALUES");

        // Cihaz verilerini ekle
        // ... Veri ekleme işlemleri ...

        queryBuilder.AppendLine("SET IDENTITY_INSERT [Devices] OFF;");

        // SQL sorgusunu çalıştır
        await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
    }
}
```

### 5. Seed İşlemi İyi Uygulamaları

1. **Bağımlılık Kontrolü**: Seeder'lar, bağımlı oldukları verilerin varlığını kontrol etmelidir.
   ```csharp
   if (!platforms.Any())
       throw new Exception("Platformlar bulunamadı. Önce PlatformSeeder çalıştırılmalıdır.");
   ```

2. **İdempotent Uygulamalar**: Seeder'lar, aynı verinin birden fazla kez eklenmesini önlemek için kontrol yapmalıdır.
   ```csharp
   if (await context.Devices.AnyAsync())
       return; // Zaten veri varsa işlem yapma
   ```

3. **Toplu Veri Ekleme**: Büyük veri setleri için `ExecuteSqlRaw` ile doğrudan SQL sorguları kullanılmalıdır.
   ```csharp
   await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
   ```

4. **Hata Yönetimi**: Seed işlemi sırasında oluşabilecek hatalar yakalanmalı ve loglanmalıdır.
   ```csharp
   try {
       // Seed işlemi
   } catch (Exception ex) {
       _logger.LogError(ex, "Seed işlemi sırasında hata: {Message}", ex.Message);
       throw;
   }
   ```

5. **Context Temizleme**: SQL komutları çalıştırıldıktan sonra Entity Framework context cache'i temizlenmelidir.
   ```csharp
   foreach (var entry in context.ChangeTracker.Entries())
   {
       entry.State = EntityState.Detached;
   }
   ```

### 6. Yapılandırma

Seed işleminin etkinleştirilmesi `appsettings.json` dosyasındaki ayarlar ile kontrol edilir:

```json
"DatabaseSettings": {
  "ResetDatabaseOnStartup": true
}
```

Bu ayar, geliştirme ortamında veritabanının her başlatmada sıfırlanıp seed edilmesini sağlarken, üretim ortamında `false` olarak ayarlanarak mevcut verilerin korunması sağlanır.

### 7. Seed Verilerinin Güncellenmesi

Uygulama geliştirildikçe, seed verilerinin güncellenmesi gerekebilir. Bu durumda:

1. İlgili seeder sınıfındaki veriler güncellenir.
2. Geliştirme ortamında `ResetDatabaseOnStartup` ayarı `true` yapılarak tüm seed işlemi yeniden çalıştırılır.
3. Üretim ortamında ise veri değişiklikleri için özel migration script'leri oluşturulmalıdır.

## Kod Standartları ve Kurallar

### 1. Naming Conventions
- **Pascal Case**: Sınıflar, metodlar, properties ve public/internal alanlar
  ```csharp
  public class DeviceService { public string GetDeviceStatus() { ... } }
  ```
- **Camel Case**: Yerel değişkenler, method parametreleri
  ```csharp
  private void ProcessDevice(DeviceEntity deviceEntity) { var deviceStatus = deviceEntity.Status; }
  ```
- **Interface İsimlendirme**: "I" öneki
  ```csharp
  public interface IDeviceRepository { ... }
  ```
- **Değişken İsimlendirme**: Anlamlı ve açıklayıcı isimler
  ```csharp
  // Kötü: var x = GetData();
  // İyi: var deviceSettings = GetDeviceSettings();
  ```

### 2. Code Organization
- Sınıf başına bir dosya kuralı
- İlgili sınıflar aynı namespace altında
- Mantıksal gruplamaya göre klasör yapısı
- En fazla 300-500 satır kod, tek sorumluluk prensibi

### 3. Error Handling
- Exception yönetiminde global exception middleware kullanımı
- Domain-specific exception sınıfları
- Hataların uygun şekilde loglanması
- Dış dünyaya detaylı hata bilgisi vermeme

### 4. Logging
- Yapılandırılabilir log seviyeleri
- Performansa duyarlı logging
- Hassas verilerin loglanmaması
- LogLibrary kullanımı ve standartları

### 5. API Responses
- Standart API yanıt formatı
- `ApiResponse<T>` wrapper kullanımı
- HTTP durum kodlarının doğru kullanımı
- Hata mesajları ve kodlarının standardizasyonu

### 6. Validation
- Giriş verileri için FluentValidation kullanımı
- ValidationFilter ile otomatik validasyon
- Domain mantığı içinde iş kuralı validasyonu
- Cross-field validasyonlar için özel validatorlar

## API Yapılandırması ve Kullanımı

### 1. Controller Yapısı
Tüm controller'lar aşağıdaki yapıyı takip etmelidir:
- Base Controller sınıfından türetilme
- ApiController ve Route attribute'larının kullanımı
- Dependency Injection ile servislerin alınması
- CRUD operasyonlarının RESTful standartlara uygun tanımlanması

Örnek Controller:
```csharp
[ApiController]
[Route("api/[controller]")]
public class DevicesController : BaseController
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(ApiResponse<IEnumerable<DeviceDto>>.Success(devices));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device == null)
            return NotFound(ApiResponse<DeviceDto>.Error("Cihaz bulunamadı", StatusCodes.Status404NotFound));
            
        return Ok(ApiResponse<DeviceDto>.Success(device));
    }

    // Diğer endpoint'ler...
}
```

### 2. Route Tanımları
- Resource tabanlı route yapısı
- HTTP metodlarının doğru kullanımı
- Versiyonlama stratejisi
- Query parametreleri için tutarlı isimlendirme

### 3. HTTP Status Codes
- 200 OK: Başarılı işlemler
- 201 Created: Başarılı oluşturma işlemleri
- 204 No Content: İçerik dönmeyen başarılı işlemler
- 400 Bad Request: Validasyon hataları
- 401 Unauthorized: Kimlik doğrulama hataları
- 403 Forbidden: Yetkilendirme hataları
- 404 Not Found: Kaynak bulunamadı
- 409 Conflict: Çakışma durumları
- 500 Internal Server Error: Sunucu hataları

### 4. Güvenlik Yapılandırması
- JWT Bearer token doğrulama (JWTVerifyLibrary)
- Authorize attribute ile yetkilendirme
- Rate limiting (RateLimitLibrary)
- CORS yapılandırması
- HTTPS zorunluluğu

### 5. Swagger Yapılandırması
- API belgelendirmesi için Swagger/OpenAPI
- JWT kimlik doğrulama desteği
- Endpoint gruplandırma
- Model şemalarının doğru gösterimi

## TCP Listener Bileşeni

### 1. TCP İletişim Protokolü
- Binary protokol formatı
- Karakter tabanlı mesaj yapısı
- Özel başlangıç, ayırıcı ve bitiş karakterleri
- Mesaj doğrulama ve hata kontrolü

### 2. TCP Listener Yapılandırması
```json
"TcpListenerSettings": {
  "Port": 3456,
  "IpAddress": "0.0.0.0",
  "MaxConnections": 100,
  "ConnectionTimeout": 30000,
  "BufferSize": 1024,
  "StartChar": "^",
  "DelimiterChar": "+",
  "EndChar": "~"
}
```

### 3. TCP Mesaj İşleme Adımları
1. Bağlantı kabul etme ve istemci yönetimi
2. Mesaj alma ve ayrıştırma
3. Komut tanımlama ve iş mantığı işleme
4. Yanıt oluşturma ve gönderme
5. Bağlantı durumu izleme ve yönetme

### 4. TCP Listener Güvenlik Kuralları
- IP tabanlı erişim kontrolü
- Bağlantı sayısı sınırlandırma
- Zaman aşımı yönetimi
- DoS koruması için throttling
- Mesaj boyutu sınırlandırma

## Veritabanı Tasarımı ve Yönetimi

### 1. Entity Framework Core Yapılandırması
- Code-First yaklaşımı
- Fluent API ile entity konfigürasyonu
- İlişki tanımlamaları ve kısıtlamalar
- Indeks ve performans optimizasyonları

### 2. Migration Stratejisi
- Geliştirme ortamında otomatik migration
- Test ve üretim ortamında kontrollü migration
- Migration script'leri
- Database seeding

### 3. Entity Yapılandırma Örnekleri
```csharp
public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.SerialNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(d => d.SerialNumber)
            .IsUnique();
            
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasOne(d => d.Station)
            .WithMany(s => s.Devices)
            .HasForeignKey(d => d.StationId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Diğer yapılandırmalar...
    }
}
```

### 4. Performans Optimizasyonları
- İndeks stratejileri
- Include sorguları
- Projection sorguları
- AsNoTracking kullanımı
- Lazy loading kontrolü

## Test Stratejisi

### 1. Birim Testleri
- Servis ve repository katmanları için
- Mocking kullanımı (Moq, NSubstitute)
- xUnit test framework
- FluentAssertions ile assertion

### 2. Entegrasyon Testleri
- WebApplicationFactory ile API testleri
- Test veritabanı kullanımı
- API endpoint'lerinin testi
- Middleware ve filtrelerin testi

### 3. Test Coverage Hedefleri
- Core ve Business katmanları: min. %80
- Controller ve Services: min. %70
- Utility ve Helpers: min. %60

## Güvenlik İyi Uygulamaları

### 1. Kimlik Doğrulama ve Yetkilendirme
- JWT token güvenliği (JWTVerifyLibrary)
- Claim-based authorization
- Role-based access control
- Policy-based authorization

### 2. Veri Güvenliği
- Hassas verilerin şifrelenmesi
- Veritabanı şifreleme (TDE)
- Transport Layer Security (HTTPS)
- Cihaz kimlik doğrulama

### 3. Güvenlik Önlemleri
- Input validation
- XSS koruması
- CSRF koruması
- SQL Injection koruması
- Rate limiting
- Brute force koruması

## Deployment ve DevOps

### 1. CI/CD Pipeline
- GitHub Actions/Azure DevOps pipeline
- Otomatik build, test ve deployment
- Environment-specific yapılandırma
- Container'ization (Docker)

### 2. Deployment Ortamları
- Development
- Testing
- Staging
- Production

### 3. Monitoring ve Logging
- Application Insights
- Health checks
- Performans metrikleri
- Olay izleme
- Log aggregation

## Performans Optimizasyonu

### 1. Caching Stratejisi
- Memory cache
- Distributed cache (Redis)
- Response caching
- Entity Framework ikinci seviye önbellek

### 2. Async/Await Kullanımı
- Tüm I/O işlemlerinde async metodlar
- ConfigureAwait(false) kullanımı
- Task.WhenAll ile paralel işlemler
- CancellationToken kullanımı

### 3. API Optimizasyonu
- Pagination
- Filtering
- Sorting
- Projection
- Compression

## İzleme ve Hata Ayıklama

### 1. Logging Stratejisi (LogLibrary)
- Yapılandırılabilir log seviyeleri
- Performansa duyarlı loglama
- Requestlerin izlenmesi
- Exception loglama

### 2. Health Checks
- Servis durumu
- Veritabanı bağlantısı
- Dış bağımlılıklar
- TCP listener durumu

### 3. Diagnostics
- Performans sayaçları
- Thread ve bellek analizi
- Darboğaz tespiti
- Profiling

## Kaynaklar ve Bağlantılar

### 1. İlgili Projeler
- JWTVerifyLibrary
- LogLibrary
- RateLimitLibrary

### 2. Dış Bağımlılıklar
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- Newtonsoft.Json
- System.Drawing.Common

### 3. Referans Dokümanlar
- API Dokümantasyonu
- TCP Protokol Spesifikasyonu
- Veri Modeli Diyagramları
- Deployment Dokümanları

## Sonuç

DeviceAPI, modern yazılım mimarisi ve tasarım prensiplerini takip eden, genişletilebilir, bakımı kolay ve güvenli bir API'dir. Clean Architecture prensiplerine uygun katmanlı yapısı, SOLID prensiplerine bağlılığı ve en iyi uygulamaları takip eden tasarımı ile, cihaz yönetimi için güçlü bir altyapı sunar. Bu belge, geliştirme ekibinin DeviceAPI'nin mimarisini, kullanımını ve kurallarını anlamaları için kapsamlı bir rehber olarak hazırlanmıştır. 