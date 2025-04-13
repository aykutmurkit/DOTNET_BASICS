# Veritabanı Seeding Süreci

**Sürüm:** 1.0.0  
**Yazar:** DeviceApi Geliştirme Ekibi  
**Tarih:** 2024

## Genel Bakış

Seeding süreci, DeviceApi uygulamasında veritabanını farklı ortamlar (Geliştirme, Test, Üretim) için gerekli temel verilerle başlatmak üzere tasarlanmıştır. Bu belge, uygulamadaki seeding mekanizmasının nasıl çalıştığını, mimari yapısını, en iyi uygulamaları ve olası sorun giderme yöntemlerini açıklar.

## Veri Modeli ve Tablo İlişkileri

DeviceApi veritabanı aşağıdaki tablolar arasındaki ilişkilere dayanmaktadır:

### 1. Stations ve Platforms (One-to-Many)
- Bir istasyon (Station), birden çok platforma (Platform) sahip olabilir
- Her platform kesinlikle bir istasyona aittir
- İlişki: `Platforms.StationId -> Stations.Id`

### 2. Platforms ve Devices (One-to-Many)
- Bir platform (Platform), birden çok cihaza (Device) sahip olabilir
- Her cihaz kesinlikle bir platforma aittir
- İlişki: `Devices.PlatformId -> Platforms.Id`

### 3. Devices ve DeviceSettings (One-to-One)
- Her cihazın (Device) bir adet ayar kaydı (DeviceSetting) vardır
- Birebir ilişki söz konusudur
- İlişki: `DeviceSettings.DeviceId -> Devices.Id`

### 4. Devices ve DeviceStatuses (One-to-Many)
- Bir cihaz (Device), zaman içinde birden çok durum kaydına (DeviceStatus) sahip olabilir
- Her durum kaydı kesinlikle bir cihaza aittir
- İlişki: `DeviceStatuses.DeviceId -> Devices.Id`

### 5. Platforms ve Predictions (One-to-Many)
- Bir platform (Platform), birden çok tahmin kaydına (Prediction) sahip olabilir
- Her tahmin kaydı kesinlikle bir platforma aittir
- İlişki: `Predictions.PlatformId -> Platforms.Id`

### 6. Devices ve Mesaj Türleri (One-to-Many)
- Bir cihaz (Device), birden çok mesaj kaydına sahip olabilir:
  - FullScreenMessages: `FullScreenMessages.DeviceId -> Devices.Id`
  - ScrollingScreenMessages: `ScrollingScreenMessages.DeviceId -> Devices.Id`
  - BitmapScreenMessages: `BitmapScreenMessages.DeviceId -> Devices.Id`
  - PeriodicMessages: `PeriodicMessages.DeviceId -> Devices.Id`
- Her mesaj kaydı kesinlikle bir cihaza aittir

### 7. Users ve UserRoles (Many-to-Many)
- Bir kullanıcı (User), birden çok role (UserRole) sahip olabilir
- Bir rol (UserRole), birden çok kullanıcıya atanabilir
- İlişki: Ara tablo üzerinden (UserRoleMapping) çoka-çok ilişki

## Seeding Mimarisi

### 1. SeederOrder Enum

Seeding işlemlerinin sırası, `SeederOrder` enum'ında merkezi olarak tanımlanmıştır:

```csharp
public enum SeederOrder
{
    // Kullanıcı rolleri ve kullanıcılar (daha önce eklenmiş olabilir)
    UserRoles = 1,
    Users = 2,
    
    // İstasyon ve platformlar
    Stations = 3,
    Platforms = 4,
    
    // Cihazlar
    Devices = 5,
    DeviceSettings = 6,
    DeviceStatuses = 7,
    
    // Mesajlar ve tahminler
    Predictions = 8,
    FullScreenMessages = 9,
    ScrollingScreenMessages = 10,
    BitmapScreenMessages = 11,
    PeriodicMessages = 12
}
```

### 2. ISeeder Arayüzü

Tüm seeder sınıflarının uygulaması gereken arayüz:

```csharp
public interface ISeeder
{
    /// <summary>
    /// Seed işleminin sırasını belirler. Düşük sayılar önce çalışır.
    /// </summary>
    /// <remarks>
    /// Bu özellik SeederExtensions.GetOrder() metodu ile otomatik olarak elde edilebilir.
    /// </remarks>
    int Order => this.GetOrder();

    /// <summary>
    /// Seed işlemini gerçekleştirir
    /// </summary>
    /// <param name="context">Veritabanı bağlantı context'i</param>
    Task SeedAsync(AppDbContext context);
}
```

### 3. SeederExtensions

`SeederExtensions` sınıfı, seeder sınıfının adına göre otomatik olarak sıra atar:

```csharp
public static class SeederExtensions
{
    public static int GetOrder<T>(this T seeder) where T : ISeeder
    {
        return seeder.GetType().Name switch
        {
            nameof(StationSeeder) => (int)SeederOrder.Stations,
            nameof(PlatformSeeder) => (int)SeederOrder.Platforms,
            nameof(DeviceSeeder) => (int)SeederOrder.Devices,
            nameof(DeviceSettingsSeeder) => (int)SeederOrder.DeviceSettings,
            nameof(DeviceStatusSeeder) => (int)SeederOrder.DeviceStatuses,
            nameof(PredictionSeeder) => (int)SeederOrder.Predictions,
            nameof(FullScreenMessageSeeder) => (int)SeederOrder.FullScreenMessages,
            nameof(ScrollingScreenMessageSeeder) => (int)SeederOrder.ScrollingScreenMessages,
            nameof(BitmapScreenMessageSeeder) => (int)SeederOrder.BitmapScreenMessages,
            nameof(PeriodicMessageSeeder) => (int)SeederOrder.PeriodicMessages,
            _ => throw new NotImplementedException($"{seeder.GetType().Name} için sıralama tanımlanmamış.")
        };
    }
}
```

### 4. DatabaseSeeder Sınıfı

Tüm seeding işlemlerini koordine eden ana sınıf:

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
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // ISeeder interface'ini implement eden tüm sınıfları bul
        var seeders = GetSeeders();

        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            try
            {
                _logger.LogInformation("Seeding: {SeederName} başlatılıyor...", seeder.GetType().Name);
                await _logService.LogInfoAsync(
                    $"Seeding: {seeder.GetType().Name} başlatılıyor...",
                    "DatabaseSeeder.SeedAsync",
                    new { SeederName = seeder.GetType().Name, SeederOrder = seeder.Order });
                
                await seeder.SeedAsync(context);
                
                _logger.LogInformation("Seeding: {SeederName} başarıyla tamamlandı.", seeder.GetType().Name);
                await _logService.LogInfoAsync(
                    $"Seeding: {seeder.GetType().Name} başarıyla tamamlandı.",
                    "DatabaseSeeder.SeedAsync",
                    new { SeederName = seeder.GetType().Name, SeederOrder = seeder.Order });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeding: {SeederName} sırasında hata oluştu: {ErrorMessage}", 
                    seeder.GetType().Name, ex.Message);
                await _logService.LogErrorAsync(
                    $"Seeding: {seeder.GetType().Name} sırasında hata oluştu",
                    "DatabaseSeeder.SeedAsync",
                    ex);
                
                throw;
            }
        }
    }

    /// <summary>
    /// Uygulamadaki tüm ISeeder implementasyonlarını bulur ve örneklerini oluşturur
    /// </summary>
    private List<ISeeder> GetSeeders()
    {
        var seeders = new List<ISeeder>();
        
        // ISeeder interface'ini implement eden tüm tipleri bul
        var seederTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(ISeeder).IsAssignableFrom(t));

        // Her tip için bir örnek oluştur
        foreach (var seederType in seederTypes)
        {
            var seeder = Activator.CreateInstance(seederType) as ISeeder;
            if (seeder != null)
            {
                seeders.Add(seeder);
            }
        }

        return seeders;
    }
}
```

### 5. Seeder Uygulama Örneği

Tipik bir seeder uygulaması örneği:

```csharp
public class StationSeeder : ISeeder
{
    // Order özelliği ISeeder arayüzü tarafından otomatik olarak sağlanır
    // SeederExtensions.GetOrder() metodu ile
    
    public async Task SeedAsync(AppDbContext context)
    {
        // İstasyonlar zaten varsa işlem yapma
        if (await context.Stations.AnyAsync())
        {
            return;
        }

        // SQL komutu oluşturma ve yürütme
        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] ON;");
        queryBuilder.AppendLine("INSERT INTO [Stations] ([Id], [Name], [Latitude], [Longitude]) VALUES");

        // İstasyon bilgileri
        var stations = new List<(int id, string name, double latitude, double longitude)>
        {
            (1, "Merkez İstasyon", 41.0082, 28.9784),
            (2, "Doğu İstasyonu", 41.0215, 29.0097),
            (3, "Batı İstasyonu", 40.9937, 28.9527)
        };

        // Değerleri ekle
        for (int i = 0; i < stations.Count; i++)
        {
            var (id, name, lat, lon) = stations[i];
            queryBuilder.Append($"({id}, '{name}', {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)})");
            
            if (i < stations.Count - 1)
            {
                queryBuilder.AppendLine(",");
            }
            else
            {
                queryBuilder.AppendLine(";");
            }
        }

        queryBuilder.AppendLine("SET IDENTITY_INSERT [Stations] OFF;");

        // SQL komutunu çalıştır
        await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
        
        // Context cache'ini temizle
        foreach (var entry in context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
}
```

## Seeding Sırası ve Bağımlılıklar

Seeding süreci, `SeederOrder` enum'ında tanımlanan belirli bir sırayı takip eder:

1. UserRoles (Sıra: 1)
   - Kullanıcı rolü verilerini oluşturur
   - Bağımlılık yok

2. Users (Sıra: 2)
   - Kullanıcı verilerini oluşturur
   - UserRoles'a bağımlı

3. Stations (Sıra: 3)
   - İstasyon verilerini oluşturur
   - Bağımlılık yok

4. Platforms (Sıra: 4)
   - Platform verilerini oluşturur
   - Stations'a bağımlı

5. Devices (Sıra: 5)
   - Cihaz verilerini oluşturur
   - Platforms'a bağımlı

6. DeviceSettings (Sıra: 6)
   - Cihaz ayarlarını oluşturur
   - Devices'a bağımlı

7. DeviceStatuses (Sıra: 7)
   - Cihaz durumu verilerini oluşturur
   - Devices'a bağımlı

8. Predictions (Sıra: 8)
   - Tahmin verilerini oluşturur
   - Platforms'a bağımlı

9. FullScreenMessages (Sıra: 9)
   - Tam ekran mesajlarını oluşturur
   - Devices'a bağımlı

10. ScrollingScreenMessages (Sıra: 10)
    - Kayan ekran mesajlarını oluşturur
    - Devices'a bağımlı

11. BitmapScreenMessages (Sıra: 11)
    - Bitmap ekran mesajlarını oluşturur
    - Devices'a bağımlı

12. PeriodicMessages (Sıra: 12)
    - Periyodik mesajları oluşturur
    - Devices'a bağımlı

## En İyi Uygulamalar

1. **Seeder Uygulaması**
   - `ISeeder` arayüzünü uygulayın
   - Arayüzün `Order` özelliğini sağlamasına izin verin
   - Seeder sınıfının adını `SeederExtensions.GetOrder()` metoduna ekleyin

2. **İdempotence (Yeniden Çalıştırılabilirlik)**
   - Veri eklemeden önce her zaman verilerin var olup olmadığını kontrol edin
   - Çoğaltmaları önlemek için benzersiz kısıtlamalar kullanın
   - Çakışmaları düzgün bir şekilde ele alın

3. **Performans**
   - Toplu ekleme işlemleri kullanın
   - Her seeder'dan sonra context önbelleğini temizleyin
   - Veritabanı gidiş-dönüşlerini en aza indirin

4. **Hata İşleme**
   - Tüm seeding işlemlerini günlüğe kaydedin
   - Ayrıntılı hata mesajları sağlayın
   - İşlem geri almalarını ele alın

5. **Veri Bütünlüğü**
   - Referans bütünlüğünü koruyun
   - Ekleme öncesinde verileri doğrulayın
   - Uygun veri tiplerini kullanın

6. **Ortama Özgü Veriler**
   - Seeding'i kontrol etmek için ortam değişkenlerini kullanın
   - Farklı ortamlar için farklı veri kümeleri sağlayın
   - Ortama özgü gereksinimleri belgelendirin

## Sorun Giderme

### Yaygın Sorunlar

1. **Sıra Tanımlanmamış**
   - Seeder sınıf adının `SeederExtensions.GetOrder()` metoduna eklenip eklenmediğini kontrol edin
   - `SeederOrder` enum'undaki seeder sırasını doğrulayın
   - Seeder sınıf adının tam olarak eşleştiğinden emin olun

2. **Yabancı Anahtar İhlalleri**
   - `SeederOrder` enum'undaki seeding sırasını kontrol edin
   - Bağımlılık ilişkilerini doğrulayın
   - Üst verilerin mevcut olduğundan emin olun

3. **Identity Insert Hataları**
   - Toplu eklemeden önce IDENTITY_INSERT'i etkinleştirin
   - Toplu eklemeden sonra IDENTITY_INSERT'i devre dışı bırakın
   - ID değerlerinin benzersiz olduğunu doğrulayın

4. **Performans Sorunları**
   - Toplu ekleme işlemleri kullanın
   - Context önbelleğini düzenli olarak temizleyin
   - SQL sorgularını optimize edin

### Hata Ayıklama İpuçları

1. **Günlük Kaydı**
   - Uygulama günlüklerini kontrol edin
   - Yapılandırılmış günlük kaydı kullanın
   - Veritabanı işlemlerini izleyin

2. **Doğrulama**
   - Seeding sonrası verileri doğrulayın
   - İlişkileri kontrol edin
   - Kısıtlamaları doğrulayın

3. **Test Etme**
   - Seeding'i izole bir şekilde test edin
   - Ortama özgü davranışı doğrulayın
   - Hata yönetimini kontrol edin

## Yapılandırma

Seeding davranışı, yapılandırma aracılığıyla kontrol edilebilir:

```json
{
  "DatabaseSettings": {
    "ResetDatabaseOnStartup": true,
    "EnableSeeding": true,
    "SeedTestData": false
  }
}
```

## Yeni Seeder Ekleme

Yeni bir seeder eklemek için:

1. Seeder sırasını `SeederOrder` enum'una ekleyin
2. `ISeeder` arayüzünü uygulayan yeni bir sınıf oluşturun
3. Seeder sınıf adını `SeederExtensions.GetOrder()` metoduna ekleyin
4. `SeedAsync` metodunu uygulayın
5. Hata işleme ve günlük kaydı ekleyin
6. Farklı ortamlarda test edin

Örnek:

```csharp
// 1. SeederOrder enum'una ekleyin
public enum SeederOrder
{
    // ... mevcut sıralar ...
    YeniVeri = 13
}

// 2. Seeder sınıfı oluşturun
public class YeniVeriSeeder : ISeeder
{
    // Order özelliği ISeeder arayüzü tarafından otomatik olarak sağlanır

    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.YeniVeri.AnyAsync())
        {
            return;
        }

        // Yeni veri ekle
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO [YeniVeri] ([Ad], [Deger])
            VALUES ('Test', 42)
        ");

        // Context'i temizle
        foreach (var entry in context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
}

// 3. SeederExtensions.GetOrder() metoduna ekleyin
public static int GetOrder<T>(this T seeder) where T : ISeeder
{
    return seeder.GetType().Name switch
    {
        // ... mevcut durumlar ...
        nameof(YeniVeriSeeder) => (int)SeederOrder.YeniVeri,
        _ => throw new NotImplementedException($"{seeder.GetType().Name} için sıralama tanımlanmamış.")
    };
}
```

## Sonuç

Seeding süreci, uygulama başlatma sürecinin kritik bir parçasıdır. Bu yönergelere uyulması ve merkezi `SeederOrder` enum'ının `GetOrder` uzantı metoduyla kullanılması, tüm ortamlarda güvenilir ve sürdürülebilir seeding işlemlerini sağlar. 