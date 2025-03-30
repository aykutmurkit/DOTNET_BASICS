# Veritabanı Yapılandırması ve Seeding

Bu bölüm, Deneme API'nin veritabanı yapılandırması, entity konfigürasyonları ve seed işlemlerini açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [AppDbContext Yapısı](#appdbcontext-yapısı)
- [Entity Konfigürasyonları](#entity-konfigürasyonları)
- [Seed İşlemleri](#seed-işlemleri)
- [Seed Veri Yapısı](#seed-veri-yapısı)
- [Best Practices](#best-practices)

## Genel Bakış

Deneme API, veritabanı yönetimi için Entity Framework Core kullanır. Veritabanı işlemleri üç ana bileşenden oluşur:

1. **AppDbContext**: Veritabanı bağlantısını yöneten ana sınıf
2. **Entity Konfigürasyonları**: Veritabanı şemasını kod-odaklı bir şekilde tanımlayan sınıflar
3. **Seeding Altyapısı**: Başlangıç verilerini yükleyen modüler yapı

## AppDbContext Yapısı

`AppDbContext`, Entity Framework Core'un `DbContext` sınıfını genişleterek veritabanı işlemlerini yönetir. Bu sınıf, veritabanı bağlantısını sağlar ve entity yapılandırmalarını uygular.

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Veritabanı tabloları
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tüm yapılandırma sınıflarını otomatik olarak uygula
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

**Önemli Özellikler**:

- Entity setleri `DbSet<T>` özellikleri olarak tanımlanır
- `ApplyConfigurationsFromAssembly` yöntemi tüm entity konfigürasyonlarını otomatik olarak keşfeder ve uygular
- Bağlantı dizesi ve diğer ayarlar dışarıdan DI (Dependency Injection) ile enjekte edilir

## Entity Konfigürasyonları

Entity yapılandırmaları, `IEntityTypeConfiguration<T>` arabirimini uygulayan ayrı sınıflarda tanımlanır. Bu yaklaşımın avantajları:

- Her entity için ayrı bir konfigürasyon sınıfı
- Daha düzenli ve bakımı kolay kod yapısı
- Daha iyi iş bölümü imkanı
- Kolay test edilebilirlik

**Örnek Konfigürasyon Sınıfı**:

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key
        builder.HasKey(u => u.Id);
        
        // Id alanı yapılandırması - auto increment
        builder.Property(u => u.Id)
               .UseIdentityColumn();
        
        // Benzersiz indeksler
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // Diğer alan yapılandırmaları
        builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
        
        // İlişkiler
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Konfigürasyon Tipleri**:

1. **Alan Yapılandırmaları**: Sütun tipleri, uzunluklar, null olabilirlik
2. **İndeksler**: Benzersiz ve genel indeksler
3. **İlişkiler**: Foreign key ilişkileri ve cascade davranışları
4. **Constraints**: Check kısıtlamaları ve diğer veritabanı kuralları

## Seed İşlemleri

Seed işlemleri, uygulamanın ilk çalıştırılmasında veya veritabanı yeniden oluşturulduğunda gerekli başlangıç verilerini yükler. Deneme API, modüler ve genişletilebilir bir seed altyapısı kullanır.

### Seed Altyapısı Bileşenleri

1. **ISeeder Arabirimi**: Tüm seed sınıfları için standart arabirim
2. **Seed Sınıfları**: Her entity için ayrı seed sınıfları
3. **DatabaseSeeder**: Tüm seed işlemlerini koordine eden merkezi sınıf

**ISeeder Arabirimi**:

```csharp
public interface ISeeder
{
    int Order { get; }
    Task SeedAsync(AppDbContext context);
}
```

**DatabaseSeeder Sınıfı**:

DatabaseSeeder, tüm `ISeeder` implementasyonlarını bulur ve çalıştırır:

```csharp
public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Tüm seed sınıflarını bul ve sırala
        var seeders = GetSeeders().OrderBy(s => s.Order);

        foreach (var seeder in seeders)
        {
            try
            {
                _logger.LogInformation("Seeding: {SeederName} başlatılıyor...", seeder.GetType().Name);
                await seeder.SeedAsync(context);
                _logger.LogInformation("Seeding: {SeederName} başarıyla tamamlandı.", seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeding: {SeederName} sırasında hata oluştu", seeder.GetType().Name);
                throw;
            }
        }
    }

    private List<ISeeder> GetSeeders()
    {
        // Assembly'deki tüm ISeeder implementasyonlarını bul
        var seederTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(ISeeder).IsAssignableFrom(t));

        var seeders = new List<ISeeder>();
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

### Program.cs'de Seed İşlemi

```csharp
// Veritabanını başlangıçta yapılandır
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Veritabanı sıfırlanıyor...");
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        logger.LogInformation("Veritabanı başlangıç verileri ekleniyor...");
        await seeder.SeedAsync();
        logger.LogInformation("Veritabanı başlangıç verileri başarıyla eklendi.");
    }
}
```

## Seed Veri Yapısı

Deneme API, iki temel seed sınıfı kullanır:

### 1. UserRoleSeeder

Kullanıcı rollerini ekler. Order=1 ile ilk sırada çalışır, çünkü diğer veriler bu rollere bağımlıdır.

```csharp
public class UserRoleSeeder : ISeeder
{
    public int Order => 1;

    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.UserRoles.AnyAsync())
        {
            return;
        }

        // Doğrudan SQL ile rolleri ekle
        await context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT [UserRoles] ON;
            
            INSERT INTO [UserRoles] ([Id], [Name])
            VALUES (1, 'User'), (2, 'Developer'), (3, 'Admin');
            
            SET IDENTITY_INSERT [UserRoles] OFF;
        ");
        
        // Context cache'ini temizle
        foreach (var entry in context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
}
```

### 2. UserSeeder

Varsayılan kullanıcıları ekler. Order=2 ile ikinci sırada çalışır, çünkü kullanıcılar rollere bağımlıdır.

```csharp
public class UserSeeder : ISeeder
{
    public int Order => 2;

    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        // SQL komutu oluşturma ve yürütme
        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SET IDENTITY_INSERT [Users] ON;");
        queryBuilder.AppendLine("INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [PasswordSalt], [RoleId], [CreatedDate], [TwoFactorEnabled], [TwoFactorCodeExpirationMinutes]) VALUES");

        // Kullanıcı bilgileri
        var users = new List<(string username, string email, string password, int roleId, int id)>
        {
            ("admin", "admin@example.com", "Admin123!", 3, 1),
            ("developer", "developer@example.com", "Developer123!", 2, 2),
            ("user", "user@example.com", "User123!", 1, 3)
        };

        // SQL komutunu oluştur ve çalıştır
        // ... (detaylar için UserSeeder.cs dosyasına bakın)
        
        await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
    }
}
```

## Best Practices

### Entity Konfigürasyon Best Practices

1. **Tek Sorumluluk İlkesi**: Her entity için ayrı bir konfigürasyon sınıfı kullanın
2. **Açık Yapılandırma**: Varsayılan davranışlara güvenmek yerine tüm özellikleri açıkça yapılandırın
3. **İndeks Kullanımı**: Sık sorgulanan alanlara indeks ekleyin
4. **İlişkilerde DeleteBehavior**: Cascade silme davranışlarını dikkatli yapılandırın

### Seed İşlemi Best Practices

1. **Modülerlik**: Her entity için ayrı bir seed sınıfı kullanın
2. **Sıralama**: Bağımlılıkları dikkate alarak seed işlemlerini sıralayın (Order özelliği)
3. **İdempotent Kod**: Seed işlemlerini birden fazla kez çalıştırılabilir şekilde tasarlayın
4. **Identity Insert**: ID değerlerini açıkça belirttiğinizde IDENTITY_INSERT kullanın
5. **SQL Enjeksiyon**: Doğrudan SQL kullanırken enjeksiyon risklerine dikkat edin

---

Deneme API'nin veritabanı yapılandırması ve seed işlemleri, modern ve genişletilebilir bir mimariye dayanır. Entity Framework Core'un code-first yaklaşımı ile oluşturulmuş bu yapı, kolay bakım, test edilebilirlik ve genişletilebilirlik sağlar. 