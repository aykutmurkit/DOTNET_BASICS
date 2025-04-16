# 06 - Veritabanı Seeding

Bu bölüm, AuthApi projesindeki veritabanı başlangıç verilerinin (seed data) nasıl oluşturulduğunu açıklamaktadır.

## Seeding Mimarisi

AuthApi, uygulamanın ilk çalıştırılması sırasında veritabanını başlangıç verileriyle doldurmak için kapsamlı bir seeding sistemi kullanır. Bu sistem, aşağıdaki özellikleri sağlar:

- **Sıralı Veri Yükleme**: Veri bağımlılıklarını göz önünde bulundurarak sıralı yükleme
- **Modüler Yapı**: Her entity için ayrı seeder sınıfları
- **Tekrar Kullanılabilirlik**: Farklı ortamlarda (development, test, production) kullanılabilir
- **Genişletilebilirlik**: Yeni seeder'lar kolayca eklenebilir

## Seeding Altyapısı

Seeding işlemi, `Program.cs` dosyasında yapılandırılır:

```csharp
// Program.cs'den seeding örneği
// Veritabanını sıfırlama ve seed etme
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var context = serviceProvider.GetRequiredService<Data.Context.AppDbContext>();
            var seeder = serviceProvider.GetRequiredService<Data.Seeding.DatabaseSeeder>();
            
            logger.LogInformation("Veritabanı sıfırlanıyor...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            logger.LogInformation("Veritabanı başlangıç verileri ekleniyor...");
            await seeder.SeedAsync();
            logger.LogInformation("Veritabanı başlangıç verileri başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Veritabanı başlatma sırasında hata oluştu");
            throw;
        }
    }
}
```

## ISeeder Arayüzü

Tüm seed sınıfları, `ISeeder` arayüzünü uygular:

```csharp
/// <summary>
/// Veritabanı seed işlemlerini sağlayan arayüz
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Seederin çalışma sırası
    /// Düşük değerler önce çalışır
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Seed işlemini gerçekleştirir
    /// </summary>
    Task SeedAsync(AppDbContext context);
}
```

## DatabaseSeeder

`DatabaseSeeder` sınıfı, tüm seed işlemlerini koordine eder:

```csharp
/// <summary>
/// Veritabanı seed işlemlerini yönetir
/// </summary>
public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Tüm seed işlemlerini gerçekleştirir
    /// </summary>
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
                await seeder.SeedAsync(context);
                _logger.LogInformation("Seeding: {SeederName} başarıyla tamamlandı.", seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeding: {SeederName} sırasında hata oluştu: {ErrorMessage}", 
                    seeder.GetType().Name, ex.Message);
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

## UserRoleSeeder

Kullanıcı rollerini eklemek için `UserRoleSeeder` kullanılır:

```csharp
/// <summary>
/// Kullanıcı rollerini seed eder
/// </summary>
public class UserRoleSeeder : ISeeder
{
    // Roller önce eklenmelidir (düşük sayı önce çalışır)
    public int Order => 10;

    public async Task SeedAsync(AppDbContext context)
    {
        // Veritabanında roller yoksa ekle
        if (!context.UserRoles.Any())
        {
            var roles = new List<UserRole>
            {
                new UserRole
                {
                    Name = "Admin",
                    Description = "Yönetici",
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Name = "Developer",
                    Description = "Geliştirici",
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Name = "User",
                    Description = "Standart Kullanıcı",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.UserRoles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}
```

## UserSeeder

Yönetici ve varsayılan kullanıcıları eklemek için `UserSeeder` kullanılır:

```csharp
/// <summary>
/// Kullanıcıları seed eder
/// </summary>
public class UserSeeder : ISeeder
{
    // Kullanıcılar, rollerden sonra eklenmelidir
    public int Order => 20;

    public async Task SeedAsync(AppDbContext context)
    {
        // Roller mevcut, şimdi kullanıcıları ekle
        if (!context.Users.Any())
        {
            // İlk önce rolleri al (önceki seeder tarafından eklenmiş olmalı)
            var adminRole = await context.UserRoles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var developerRole = await context.UserRoles.FirstOrDefaultAsync(r => r.Name == "Developer");
            var userRole = await context.UserRoles.FirstOrDefaultAsync(r => r.Name == "User");

            if (adminRole == null || developerRole == null || userRole == null)
            {
                throw new InvalidOperationException("Kullanıcı rolleri bulunamadı. UserRoleSeeder önce çalıştırılmalı.");
            }

            // Şifreleri güvenli bir şekilde hash'le
            var adminPassword = PasswordHelper.HashPassword("Admin123!");
            var developerPassword = PasswordHelper.HashPassword("Developer123!");
            var userPassword = PasswordHelper.HashPassword("User123!");

            // Kullanıcıları oluştur
            var users = new List<User>
            {
                // Admin kullanıcısı
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = adminPassword,
                    FullName = "System Admin",
                    UserRoleId = adminRole.Id,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow
                },
                
                // Developer kullanıcısı
                new User
                {
                    Username = "developer",
                    Email = "developer@example.com",
                    PasswordHash = developerPassword,
                    FullName = "Test Developer",
                    UserRoleId = developerRole.Id,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow
                },
                
                // Standart kullanıcı
                new User
                {
                    Username = "user",
                    Email = "user@example.com",
                    PasswordHash = userPassword,
                    FullName = "Test User",
                    UserRoleId = userRole.Id,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            
            // Kullanıcılar için refresh token'lar ekle (opsiyonel)
            var refreshTokens = new List<RefreshToken>();
            foreach (var user in users)
            {
                refreshTokens.Add(new RefreshToken
                {
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString("N"),
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            await context.RefreshTokens.AddRangeAsync(refreshTokens);
            await context.SaveChangesAsync();
        }
    }
}
```

## Yapılandırma

Veritabanı seeding işlemi, `appsettings.json` dosyasında yapılandırılır:

```json
"DatabaseSettings": {
  "ResetDatabaseOnStartup": true
}
```

## Seeding Adımları

Veritabanı seeding işlemi aşağıdaki adımları içerir:

1. **DatabaseSettings** yapılandırmasını kontrol et
2. **EnsureDeletedAsync()** ile veritabanını temizle (yapılandırılmışsa)
3. **EnsureCreatedAsync()** ile veritabanı şemasını oluştur
4. **GetSeeders()** ile tüm seeder sınıflarını bul
5. Seeder'ları **Order** özelliğine göre sırala
6. Her seeder'ı sırayla çalıştır
7. Loglama ile işlemi izle

## Elle Seeding Tetikleme

Geliştirme veya test aşamasında, seeding işlemini manuel olarak da tetikleyebilirsiniz:

```csharp
// Seeding işlemini manuel tetikleme örneği
public async Task ResetAndSeedDatabase()
{
    using (var scope = _serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
                
        await seeder.SeedAsync();
    }
}
```

## Özel Seed Data Ekleme

Projeye yeni bir seed sınıfı eklemek için aşağıdaki adımları izleyin:

1. `ISeeder` arayüzünü uygulayan yeni bir sınıf oluşturun:

```csharp
/// <summary>
/// Ürün kategorilerini seed eder
/// </summary>
public class ProductCategorySeeder : ISeeder
{
    // Önce roller ve kullanıcılar, sonra kategoriler eklenecek
    public int Order => 30;

    public async Task SeedAsync(AppDbContext context)
    {
        if (!context.ProductCategories.Any())
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory
                {
                    Name = "Elektronik",
                    Description = "Elektronik ürünler",
                    CreatedAt = DateTime.UtcNow
                },
                new ProductCategory
                {
                    Name = "Mobilya",
                    Description = "Ev mobilyaları",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.ProductCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
```

2. Yeni oluşturduğunuz seeder sınıfını otomatik olarak algılanmak üzere Data.Seeding namespace'inde tanımlayın.

3. Uygulamayı yeniden başlattığınızda, `DatabaseSeeder` yeni sınıfı otomatik olarak algılayacak ve çalıştıracaktır.

## Test Verileri ile Seeding

Test senaryoları için özel test verileri ile seeding:

```csharp
/// <summary>
/// Test verileri için seeder
/// </summary>
public class TestDataSeeder : ISeeder
{
    public int Order => 100; // En son çalışsın
    
    public async Task SeedAsync(AppDbContext context)
    {
        // Sadece geliştirme ortamında test verilerini ekle
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            return;
            
        if (!context.TestData.Any())
        {
            // Test verileri ekle
            // ...
            await context.SaveChangesAsync();
        }
    }
}
```

