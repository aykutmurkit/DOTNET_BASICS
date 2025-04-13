# RecurringDays NULL Değer Sorunu Çözümü

Bu dokümanda, ScheduleRule entity'sinin RecurringDays alanında oluşan NULL değer sorununu çözmek için yapılan değişiklikler ve alınan önlemler detaylı bir şekilde anlatılmaktadır.

## Sorun Tanımı

`ScheduleRule` sınıfında `RecurringDays` alanı, tekrarlayan kuralların hangi günlerde aktif olacağını belirten bir string olarak tanımlanmıştır. Bu alan, veritabanında NULL değer almaması gereken bir alandır, çünkü:

1. IsActive metodu içinde bu alanın Split metodu kullanılarak işlenmesi, NULL değerlerde hatalara yol açmaktadır.
2. Tekrarlayan kurallar için hangi günlerde tekrarlanacağı bilgisi kritik öneme sahiptir.
3. Tekrarlamayan kurallar için bile bu alanın bir değer taşıması gerekir (bu durumda "0" değeri kullanılmaktadır).

Veritabanında bu alanın NULL değer alması, uygulama çalışırken `NullReferenceException` hatalarına ve beklenmeyen davranışlara neden olmaktadır.

## Çözüm Yaklaşımı

Sorunu çözmek için aşağıdaki adımlar uygulanmıştır:

1. ScheduleRule entity'sine varsayılan değer tanımlama
2. Entity Framework konfigürasyonu ile veritabanı seviyesinde varsayılan değer atama
3. Seeder içinde NULL kontrol mekanizması ekleme
4. Repository katmanında NULL değerleri tespit edip düzeltecek metot ekleme
5. Business katmanında ve uygulama başlangıcında bu düzeltme metodunun çağrılması

## 1. ScheduleRule Entity'sine Varsayılan Değer Tanımlama

`ScheduleRule` sınıfına bir constructor eklenerek, nesne oluşturulduğunda `RecurringDays` alanına otomatik olarak "0" değeri atanması sağlanmıştır:

```csharp
public class ScheduleRule
{
    public ScheduleRule()
    {
        // RecurringDays için varsayılan değer ata
        RecurringDays = "0";
    }
    
    // ... mevcut kod ...
}
```

Bu sayede, yeni bir `ScheduleRule` nesnesi oluşturulduğunda, özellikle belirtilmediği sürece `RecurringDays` alanı NULL değil "0" değerini alacaktır.

## 2. Entity Framework Konfigürasyonu ile Veritabanı Seviyesinde Varsayılan Değer Atama

Veritabanı seviyesinde de NULL değerlerin önüne geçmek için, Entity Framework Core konfigürasyonu aracılığıyla `RecurringDays` alanına varsayılan değer atanmıştır:

```csharp
public class ScheduleRuleConfiguration : IEntityTypeConfiguration<ScheduleRule>
{
    public void Configure(EntityTypeBuilder<ScheduleRule> builder)
    {
        // ... mevcut kod ...
        
        builder.Property(e => e.RecurringDays)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("0"); // "0" for non-recurring rules
            
        // ... mevcut kod ...
    }
}
```

Bu konfigürasyon sayesinde:
1. `.IsRequired()` ile alanın boş geçilemez olması sağlanmıştır.
2. `.HasMaxLength(20)` ile maksimum uzunluk sınırlaması konulmuştur.
3. `.HasDefaultValue("0")` ile veritabanı seviyesinde bir varsayılan değer atanmıştır.

## 3. Seeder İçinde NULL Kontrol Mekanizması Ekleme

Veri ekleme (seeding) sürecinde, oluşturulan kuralların `RecurringDays` alanının NULL olup olmadığını kontrol eden ve gerekirse düzelten bir mekanizma eklenmiştir:

```csharp
public class ScheduleRuleSeeder : ISeeder
{
    public async Task SeedAsync(AppDbContext context)
    {
        // ... mevcut kod ...
        
        // Örnek kurallar oluştur
        var scheduleRules = new List<ScheduleRule>();
        
        // ... kurallar ekleniyor ...
        
        // Her kuralın RecurringDays alanının null olmadığından emin olalım
        foreach (var rule in scheduleRules)
        {
            if (string.IsNullOrEmpty(rule.RecurringDays))
            {
                // Eğer null veya boş ise, tekrarlamayan kurallar için "0" değerini ata
                rule.RecurringDays = rule.IsRecurring ? "1,2,3,4,5,6,7" : "0";
            }
        }
        
        // Veritabanına ekle
        await context.ScheduleRules.AddRangeAsync(scheduleRules);
        await context.SaveChangesAsync();
    }
}
```

Bu kontrol mekanizması sayesinde, veri ekleme sırasında oluşturulan kuralların `RecurringDays` alanının NULL olması durumunda:
- Eğer kural tekrarlayan bir kural ise (IsRecurring=true), her gün tekrarlandığını belirten "1,2,3,4,5,6,7" değeri atanır.
- Eğer kural tekrarlamayan bir kural ise (IsRecurring=false), tek seferlik olduğunu belirten "0" değeri atanır.

## 4. Repository Katmanında NULL Değerleri Tespit Edip Düzeltecek Metot Ekleme

Repository katmanına, veritabanındaki mevcut kayıtlardaki NULL `RecurringDays` değerlerini tespit eden ve düzelten bir metot eklenmiştir:

```csharp
public class ScheduleRuleRepository : IScheduleRuleRepository
{
    // ... mevcut kod ...
    
    /// <summary>
    /// Veritabanındaki tüm kurallar için RecurringDays alanının null olmamasını sağlar
    /// </summary>
    public async Task FixNullRecurringDaysAsync()
    {
        var rules = await _context.ScheduleRules.ToListAsync();
        var hasChanges = false;
        
        foreach (var rule in rules)
        {
            if (string.IsNullOrEmpty(rule.RecurringDays))
            {
                _logger.LogWarning("Kural ID {RuleId} için null RecurringDays değeri düzeltiliyor", rule.Id);
                
                // IsRecurring true ise her gün (1-7), false ise tek seferlik (0)
                rule.RecurringDays = rule.IsRecurring ? "1,2,3,4,5,6,7" : "0";
                hasChanges = true;
            }
        }
        
        if (hasChanges)
        {
            _logger.LogInformation("Null RecurringDays değerleri düzeltildi, değişiklikler kaydediliyor...");
            await _context.SaveChangesAsync();
            _logger.LogInformation("Değişiklikler başarıyla kaydedildi.");
        }
        else
        {
            _logger.LogInformation("Null RecurringDays değeri bulunamadı.");
        }
    }
    
    // ... mevcut kod ...
}
```

Bu metot, aşağıdaki işlemleri gerçekleştirir:
1. Tüm `ScheduleRule` kayıtlarını veritabanından çeker.
2. Her bir kaydın `RecurringDays` alanını kontrol eder.
3. NULL veya boş değer bulduğunda, kural tekrarlayan bir kural mı değil mi durumuna göre uygun bir değer atar.
4. Değişiklik yapıldıysa değişiklikleri kaydeder ve log mesajları oluşturur.

Ayrıca, IScheduleRuleRepository interface'ine de bu metot eklenmiştir:

```csharp
public interface IScheduleRuleRepository
{
    // ... mevcut kod ...
    
    /// <summary>
    /// Veritabanındaki tüm kurallar için RecurringDays alanının null olmamasını sağlar
    /// </summary>
    Task FixNullRecurringDaysAsync();
}
```

## 5. IsRuleActive Metodunda NULL Kontrol Mekanizması Ekleme

`ScheduleRuleRepository` sınıfındaki `IsRuleActive` metoduna, `RecurringDays` alanının NULL olması durumunda nasıl davranılacağına ilişkin bir kontrol mekanizması eklenmiştir:

```csharp
private bool IsRuleActive(ScheduleRule rule, DateTime currentDateTime)
{
    // ... mevcut kod ...
    
    // Haftanın günü kontrolü
    if (!string.IsNullOrEmpty(rule.RecurringDays))
    {
        // RecurringDays formatı: "1,2,5" -> 1=Pazartesi, 7=Pazar
        var dayOfWeek = (int)currentDateTime.DayOfWeek;
        if (dayOfWeek == 0) dayOfWeek = 7; // Pazar için 0 yerine 7 kullan
        
        var days = rule.RecurringDays.Split(',').Select(int.Parse).ToList();
        
        // Bugün belirtilen günlerden biri değilse, aktif değil
        if (!days.Contains(dayOfWeek))
        {
            return false;
        }
    }
    else
    {
        // RecurringDays null veya boş ise, tekrarlayan kural için her gün olarak kabul et
        _logger.LogWarning("Kural ID {RuleId} için RecurringDays alanı null veya boş. Her gün aktif olarak kabul ediliyor.", rule.Id);
    }
    
    // Tüm kontrolleri geçtiyse kural aktiftir
    return true;
}
```

Bu sayede, metodun çalışması sırasında `RecurringDays` alanının NULL olması durumunda:
1. Bir uyarı log mesajı oluşturulur.
2. Kural her gün aktif olarak kabul edilir (NULL değer yerine her gün aktif olduğu varsayılır).

## 6. Business Katmanında ve Uygulama Başlangıcında Düzeltme Metodunun Çağrılması

`ScheduleRuleService` sınıfındaki `ApplyActiveRulesAsync` ve `ApplyActiveRulesForDeviceAsync` metotlarında, aktif kuralları uygulamadan önce NULL `RecurringDays` değerlerinin düzeltilmesi sağlanmıştır:

```csharp
public async Task ApplyActiveRulesAsync()
{
    try
    {
        _logger.LogInformation("Tüm cihazlar için aktif kurallar uygulanıyor...");
        
        // Önce tüm null RecurringDays değerlerini düzeltelim
        await _scheduleRuleRepository.FixNullRecurringDaysAsync();
        
        // ... mevcut kod ...
    }
    catch (Exception ex)
    {
        // ... mevcut kod ...
    }
}

public async Task ApplyActiveRulesForDeviceAsync(int deviceId)
{
    try
    {
        _logger.LogInformation("Cihaz ID {DeviceId} için aktif kurallar uygulanıyor...", deviceId);
        
        // Önce tüm null RecurringDays değerlerini düzeltelim
        await _scheduleRuleRepository.FixNullRecurringDaysAsync();
        
        // ... mevcut kod ...
    }
    catch (Exception ex)
    {
        // ... mevcut kod ...
    }
}
```

Ayrıca, uygulama başlangıcında da `FixNullRecurringDaysAsync` metodu çağrılarak, mevcut NULL değerlerin düzeltilmesi sağlanmıştır:

```csharp
// Program.cs içinde
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    // ... veritabanını sıfırlama ve seed etme kodu ...
}
else
{
    // Veritabanını sıfırlamıyorsak, mevcut ScheduleRule kayıtlarındaki null RecurringDays değerlerini düzelt
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Null RecurringDays değerleri kontrol ediliyor...");
            var scheduleRuleRepository = serviceProvider.GetRequiredService<Data.Interfaces.IScheduleRuleRepository>();
            await scheduleRuleRepository.FixNullRecurringDaysAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RecurringDays düzeltme işlemi sırasında hata oluştu");
        }
    }
}
```

Bu sayede:
1. Uygulama ilk başlatıldığında, veritabanı sıfırlanmıyorsa mevcut NULL değerler düzeltilir.
2. Kural uygulama işlemleri sırasında, aktif kuralları uygulamadan önce NULL değerler düzeltilir.

## Sonuç

Yukarıda detaylandırılan çözüm adımları sayesinde:

1. Yeni oluşturulan `ScheduleRule` nesneleri her zaman varsayılan bir RecurringDays değerine sahip olacaktır.
2. Veritabanı seviyesinde NULL değerlerin oluşması engellenecektir.
3. Veri ekleme (seeding) sürecinde NULL değerlerin oluşması kontrol edilecektir.
4. Mevcut veritabanındaki NULL değerler uygulama başlangıcında ve kural uygulama işlemleri öncesinde düzeltilecektir.
5. Herhangi bir NULL değer bulunması durumunda uygun bir şekilde işlenecektir.

Bu çok katmanlı yaklaşım, `RecurringDays` alanında NULL değerlerin oluşmasını ve bunların uygulamada sorunlara yol açmasını engelleyecektir. 