# Seeding Süreci

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## Seeding Nedir?

Seeding, veritabanına başlangıç verilerinin yüklenmesi işlemidir. Bu işlem, uygulamanın ilk kurulumunda veya test ortamında gerekli olan temel verilerin otomatik olarak oluşturulmasını sağlar.

## Seeding Sınıfları

### 1. PlatformSeeder

```csharp
public class PlatformSeeder : ISeeder
{
    public int Order => 1;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Platform verilerini ekler
    }
}
```

### 2. StationSeeder

```csharp
public class StationSeeder : ISeeder
{
    public int Order => 2;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // İstasyon verilerini ekler
    }
}
```

### 3. DeviceSeeder

```csharp
public class DeviceSeeder : ISeeder
{
    public int Order => 7;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Cihaz verilerini ekler
    }
}
```

### 4. FullScreenMessageSeeder

```csharp
public class FullScreenMessageSeeder : ISeeder
{
    public int Order => 8;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Tam ekran mesaj verilerini ekler
    }
}
```

### 5. ScrollingScreenMessageSeeder

```csharp
public class ScrollingScreenMessageSeeder : ISeeder
{
    public int Order => 9;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Kaydırma ekran mesaj verilerini ekler
    }
}
```

### 6. BitmapScreenMessageSeeder

```csharp
public class BitmapScreenMessageSeeder : ISeeder
{
    public int Order => 10;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Bitmap ekran mesaj verilerini ekler
    }
}
```

### 7. PeriodicMessageSeeder

```csharp
public class PeriodicMessageSeeder : ISeeder
{
    public int Order => 11;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Periyodik mesaj verilerini ekler
    }
}
```

## Seeding Sırası

Seeding işlemi, `Order` özelliğine göre sıralı bir şekilde gerçekleştirilir:

1. PlatformSeeder (Order: 1)
2. StationSeeder (Order: 2)
3. DeviceSeeder (Order: 7)
4. FullScreenMessageSeeder (Order: 8)
5. ScrollingScreenMessageSeeder (Order: 9)
6. BitmapScreenMessageSeeder (Order: 10)
7. PeriodicMessageSeeder (Order: 11)

## Seeding Çalıştırma

Seeding işlemi, uygulama başlatıldığında otomatik olarak çalıştırılır. Ayrıca, aşağıdaki komut ile manuel olarak da çalıştırılabilir:

```bash
dotnet run --seed
```

## Özel Seeding Senaryoları

### 1. Test Ortamı için Seeding

Test ortamı için özel seeding verileri oluşturmak için:

```csharp
public class TestEnvironmentSeeder : ISeeder
{
    public int Order => 100; // En son çalışacak
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            // Test verilerini ekle
        }
    }
}
```

### 2. Geliştirme Ortamı için Seeding

Geliştirme ortamı için özel seeding verileri oluşturmak için:

```csharp
public class DevelopmentEnvironmentSeeder : ISeeder
{
    public int Order => 100; // En son çalışacak
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            // Geliştirme verilerini ekle
        }
    }
}
```

## Seeding Verilerini Güncelleme

Seeding verilerini güncellemek için:

1. İlgili seeder sınıfını bulun
2. `SeedAsync` metodunu güncelleyin
3. Uygulamayı yeniden başlatın

## Önemli Notlar

1. Seeding işlemi, veritabanındaki mevcut verileri silmez
2. Her seeder sınıfı, kendi verilerini bağımsız olarak ekler
3. Seeding sırası, veri bağımlılıklarına göre belirlenir
4. Seeding işlemi, transaction içinde gerçekleştirilir

## Hata Yönetimi

Seeding sırasında oluşabilecek hatalar:

1. **Veri Çakışması**: Aynı veri tekrar eklenmeye çalışılırsa
2. **Bağımlılık Hatası**: Bağımlı veriler henüz eklenmemişse
3. **Veritabanı Hatası**: Veritabanı bağlantısı kesilirse

Bu hatalar için uygun hata yönetimi mekanizmaları eklenmiştir.

---

[◀ Yapılandırma](06-Yapilandirma.md) | [İleri: Versiyonlama ▶](08-Versiyonlama.md) 