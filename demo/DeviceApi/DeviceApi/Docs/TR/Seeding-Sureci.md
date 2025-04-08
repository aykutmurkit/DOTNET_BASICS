# Veritabanı Seeding Süreci

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu belge, DeviceApi projesindeki veritabanı seed işlemlerinin detaylı yapısını, çalışma prensibini, uygulama yöntemlerini ve sorun giderme stratejilerini açıklar. Seed süreci, uygulamanın ilk kurulumunda veya veritabanının sıfırlanması durumunda gerekli verilerin otomatik olarak oluşturulmasını sağlar.

## İçindekiler

1. [Genel Bakış](#1-genel-bakış)
2. [Seeding Mimarisi](#2-seeding-mimarisi)
3. [Seeder Türleri ve Çalışma Sırası](#3-seeder-türleri-ve-çalışma-sırası)
4. [Veri İlişkileri ve Yapılar](#4-veri-ilişkileri-ve-yapılar)
5. [Seed İşlem Akışı](#5-seed-işlem-akışı)
6. [Performans Optimizasyonu](#6-performans-optimizasyonu)
7. [Sorun Giderme](#7-sorun-giderme)
8. [Yeni Seeder Ekleme](#8-yeni-seeder-ekleme)
9. [İleri Düzey Konular](#9-ileri-düzey-konular)
10. [En İyi Uygulamalar](#10-en-iyi-uygulamalar)

## 1. Genel Bakış

DeviceApi, projenin farklı ortamlarda (geliştirme, test, demo) hızlı bir şekilde çalışır hale gelmesi için otomatik veritabanı seeding mekanizması sağlar. Bu mekanizma sayesinde:

- Yeni geliştiriciler hızlıca çalışan bir sistem elde edebilir
- Test ortamları tutarlı veri kümeleriyle başlatılabilir 
- Demo ortamları gerçekçi verilerle hazırlanabilir
- Geliştirme sırasında veritabanını kolayca sıfırlayıp yeniden başlatabilirsiniz

Seed süreci, `Program.cs` dosyasında `DatabaseSettings:ResetDatabaseOnStartup` yapılandırma değeri `true` olduğunda otomatik olarak çalışır. Bu süreç, veritabanını tamamen siler, yeniden oluşturur ve ardından temel verileri ekler.

## 2. Seeding Mimarisi

### 2.1 Temel Bileşenler

Seeding mimarisi şu temel bileşenlerden oluşur:

- **ISeeder Interface**: Tüm seed sınıflarının uygulaması gereken sözleşme
- **DatabaseSeeder**: Tüm seed işlemlerini koordine eden ana sınıf
- **Spesifik Seeder Sınıfları**: Her veri türü için özelleştirilmiş seed işlemlerini gerçekleştiren sınıflar 
- **SeederOrder Enum**: Seed işlemlerinin çalışma sırasını belirleyen numaralandırma
- **SeederExtensions**: Seed işlemlerini kolaylaştıran yardımcı metotlar

### 2.2 ISeeder Interface

Tüm seeder'ların uygulaması gereken temel arayüz:

```csharp
public interface ISeeder
{
    int Order { get; }
    Task SeedAsync(AppDbContext context);
}
```

Bu arayüz iki temel öğe içerir:
- **Order**: Seeder'ın çalışma sırasını belirleyen öncelik değeri
- **SeedAsync**: Veritabanına veri ekleyen asenkron metot

### 2.3 Mimari Yapı

Seeding mimarisi, veri hiyerarşisini ve bağımlılıkları takip eden katmanlı bir yapı kullanır:

```
┌───────────────────────────────────────────────────────────────┐
│                        Program.cs                              │
│           (Uygulama başlangıcında seed işlemini başlatır)     │
└───────────────────────────────┬───────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                      DatabaseSeeder                            │
│      (Tüm seeder'ları bulur, sıralar ve çalıştırır)           │
└───────────────────────────────┬───────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────┐
│             Reflection ile ISeeder sınıflarını bulma         │
└──────────────────────────────┬───────────────────────────────┘
                               │
                ┌──────────────┼─────────────┬─────────────────┐
                │              │             │                 │
                ▼              ▼             ▼                 ▼
┌───────────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────────┐
│  Referans Veriler │ │    Temel    │ │ Ana Varlık  │ │İlişkisel Varlıklar│
│(AlignmentTypeSeeder)│ │(StationSeeder)│ │(DeviceSeeder) │ │ (MessageSeeders) │
└───────────────────┘ └─────────────┘ └─────────────┘ └─────────────────┘
```

## 3. Seeder Türleri ve Çalışma Sırası

### 3.1 Seeder Türleri

DeviceApi'deki seeder'lar işlevsel olarak dört ana kategoriye ayrılır:

1. **Referans Verisi Seeder'ları**: Temel sabitler ve enumlar için (AlignmentTypeSeeder)
2. **Ana Veri Seeder'ları**: Sistemin temel varlıkları için (StationSeeder, PlatformSeeder)
3. **İş Varlıkları Seeder'ları**: Ana iş varlıkları için (DeviceSeeder, DeviceSettingsSeeder)
4. **İlişkisel Veri Seeder'ları**: İş varlıklarına bağlı veriler için (MessageSeeder'lar)

### 3.2 Seeder Çalışma Sırası

Seeder'lar, tablolar arasındaki ilişkileri doğru kurabilmek için belirli bir sırada çalışır:

| Sıra | Seeder Adı | Order Değeri | Bağımlılıklar | Açıklama |
| ---- | ---------- | ------------ | ------------- | -------- |
| 1 | AlignmentTypeSeeder | 1 | Yok | Temel hizalama türlerini ekler |
| 2 | StationSeeder | 3 | Yok | İstasyon verilerini oluşturur |
| 3 | PlatformSeeder | 4 | StationSeeder | Platform verilerini oluşturur |
| 4 | PredictionSeeder | 5 | PlatformSeeder | Tren tahmin verilerini oluşturur |
| 5 | DeviceSeeder | 5 | PlatformSeeder | Cihaz verilerini oluşturur |
| 6 | DeviceSettingSeeder | 6 | DeviceSeeder | Cihaz ayarlarını oluşturur |
| 7 | FullScreenMessageSeeder | 40 | DeviceSeeder | Tam ekran mesajları oluşturur |
| 8 | ScrollingScreenMessageSeeder | 41 | DeviceSeeder | Kayan ekran mesajları oluşturur |
| 9 | BitmapScreenMessageSeeder | 42 | DeviceSeeder | Bitmap ekran mesajları oluşturur |
| 10 | PeriodicMessageSeeder | 43 | DeviceSeeder | Periyodik mesajları oluşturur |

### 3.3 Bağımlılık Grafiği

Seeder'lar arasındaki bağımlılık ilişkileri ve çalışma sırası:

```
AlignmentTypeSeeder (1) 
          │
          ▼
StationSeeder (3)
          │
          ▼
PlatformSeeder (4)
      ┌───┴────┐
      │        │
      ▼        ▼
PredictionSeeder (5)  DeviceSeeder (5)
                           │
                           ▼
                  DeviceSettingSeeder (6)
                           │
                           ▼
                 FullScreenMessageSeeder (40)
                           │
                           ▼
               ScrollingScreenMessageSeeder (41)
                           │
                           ▼
                BitmapScreenMessageSeeder (42)
                           │
                           ▼
                 PeriodicMessageSeeder (43)
```

## 4. Veri İlişkileri ve Yapılar

### 4.1 Veri Hiyerarşisi

DeviceApi'de oluşturulan seed verileri aşağıdaki hiyerarşik yapıya sahiptir:

```
İstasyon (Station) 1─┐
                     ├── Platform 1───┬─── Prediction 1, 2, 3 (Farklı trenler)
                     │                ├─── Cihaz 1 ────┬─── Cihaz Ayarları 1
                     │                │                ├─── Tam Ekran Mesaj 1
                     │                │                ├─── Kayan Ekran Mesaj 1
                     │                │                ├─── Bitmap Ekran Mesaj 1
                     │                │                └─── Periyodik Mesaj 1
                     │                └─── Cihaz 2 ────┬─── Cihaz Ayarları 2
                     │                                 └─── ...
                     │
                     └── Platform 2───┬─── Prediction 4, 5 (Farklı trenler)
                                      ├─── Cihaz 3 ────┬─── ...
                                      └─── Cihaz 4 ────┴─── ...
```

### 4.2 Örnek Veri Yapısı

Tipik bir istasyon ve platformlarındaki cihazlar şu şekilde yapılandırılır:

- **İstasyon A (Merkez)**
  - **Platform 1 (Ana Giriş)**
    - *Cihaz 1:* LED Gösterge (192.168.1.101:8001)
    - *Cihaz 2:* Bilgi Ekranı (192.168.1.102:8001)
  - **Platform 2 (Yan Giriş)**
    - *Cihaz 3:* LED Gösterge (192.168.1.103:8001)
    - *Cihaz 4:* Bilgi Ekranı (192.168.1.104:8001)

- **İstasyon B (Doğu)**
  - **Platform 3 (Ana)**
    - *Cihaz 5:* LED Gösterge (192.168.1.105:8001)
    - *Cihaz 6:* Bilgi Ekranı (192.168.1.106:8001)

Bu yapı, gerçek bir metro istasyonu senaryosunu yansıtır ve test, geliştirme ve demo amaçları için uygun veri sağlar.

## 5. Seed İşlem Akışı

### 5.1 Genel İşlem Akışı

Seed işlemi şu adımlarla gerçekleştirilir:

1. Uygulama başlatılır ve `DatabaseSettings:ResetDatabaseOnStartup` değeri kontrol edilir
2. Eğer değer `true` ise, veritabanı silinir ve yeniden oluşturulur
3. `DatabaseSeeder.SeedAsync()` metodu çağrılır
4. DatabaseSeeder, reflection kullanarak tüm `ISeeder` tipindeki sınıfları bulur
5. Seeder'lar `Order` değerine göre sıralanır
6. Her seeder sırayla çalıştırılır
7. Tüm seed işlemleri detaylı şekilde loglanır

### 5.2 Teknik Uygulama

Seed işlemi, `DatabaseSeeder` sınıfında şu kodla gerçekleştirilir:

```csharp
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
            // Log işlemleri
            _logger.LogInformation("Seeding: {SeederName} başlatılıyor...", seeder.GetType().Name);
            await _logService.LogInfoAsync($"Seeding: {seeder.GetType().Name} başlatılıyor...",
                "DatabaseSeeder.SeedAsync",
                new { SeederName = seeder.GetType().Name });
            
            // Seed işlemini gerçekleştir
            await seeder.SeedAsync(context);
            
            // Log işlemleri
            _logger.LogInformation("Seeding: {SeederName} başarıyla tamamlandı.", seeder.GetType().Name);
            await _logService.LogInfoAsync($"Seeding: {seeder.GetType().Name} başarıyla tamamlandı.",
                "DatabaseSeeder.SeedAsync",
                new { SeederName = seeder.GetType().Name });
        }
        catch (Exception ex)
        {
            // Hata loglama
            _logger.LogError(ex, "Seeding: {SeederName} sırasında hata oluştu: {ErrorMessage}", 
                seeder.GetType().Name, ex.Message);
            await _logService.LogErrorAsync($"Seeding: {seeder.GetType().Name} sırasında hata oluştu",
                "DatabaseSeeder.SeedAsync", ex);
            throw;
        }
    }
}
```

## 6. Performans Optimizasyonu

### 6.1 Toplu Veri Ekleme

Büyük veri setlerinde performans artışı sağlamak için:

- **SQL Batch İşlemleri**: SQL komutları ile toplu veri ekleme yaygın olarak kullanılır
- **StringBuilder Kullanımı**: Çok sayıda SQL ifadesi tek seferde oluşturulur
- **IDENTITY_INSERT Kullanımı**: ID değerlerinin korunması ve ilişkilerin doğru kurulması sağlanır

### 6.2 Context Yönetimi

Entity Framework'ün performanslı kullanımı için:

- **Context Temizleme**: Her seeder çalıştıktan sonra ChangeTracker temizlenir
- **Eager Loading Azaltma**: Bulk ekleme için EF Core'un tracking mekanizması devre dışı bırakılır
- **Paralel Çalışma**: Bağımsız seeder'lar potansiyel olarak paralel çalıştırılabilir

## 7. Sorun Giderme

### 7.1 Yaygın Hatalar ve Çözümleri

| Hata | Olası Neden | Çözüm |
| ---- | ----------- | ----- |
| The INSERT statement conflicted with the FOREIGN KEY constraint | İlişkili tablolarda veriler yanlış sırayla ekleniyor | Seeder'ların Order değerlerini kontrol edin |
| Cannot insert explicit value for identity column | IDENTITY_INSERT açık değil | SQL sorgusunda SET IDENTITY_INSERT [Tablo] ON; komutunu ekleyin |
| Violation of PRIMARY KEY constraint | Aynı ID değerine sahip kayıt eklenmeye çalışılıyor | ID değerlerinin benzersiz olduğundan emin olun |
| Timeout expired | Çok büyük veri seti ekleme işlemi | Veri setini küçültün veya batch size'ı ayarlayın |

### 7.2 Log İnceleme

Seed işlemlerinde hata ayıklarken:

- Uygulama loglarını inceleyerek hangi seeder'da hata oluştuğunu bulun
- LogLibrary kullanılarak yapılandırılmış loglar sayesinde detaylı bilgilere ulaşın
- SQL profiler ile veritabanı işlemlerini izleyin

### 7.3 Seed'i Zorla Çalıştırma

Geliştirme sırasında seed işlemini zorla çalıştırmak için:

```json
// appsettings.Development.json
{
  "DatabaseSettings": {
    "ResetDatabaseOnStartup": true
  }
}
```

## 8. Yeni Seeder Ekleme

### 8.1 Adım Adım Seeder Oluşturma

Yeni bir seeder eklemek için izlenmesi gereken adımlar:

1. `SeederOrder` enum'ına yeni seeder'ınızın sıra değerini ekleyin
2. `ISeeder` interface'ini implement eden yeni bir sınıf oluşturun
3. `Order` özelliğini, bağımlılıklara göre uygun bir değere ayarlayın
4. `SeedAsync` metodunda:
   - Tabloda veri olup olmadığını kontrol edin
   - Bağımlılıkları kontrol edin (diğer tablolardaki veriler)
   - Verileri ekleyin
   - Context'i temizleyin

### 8.2 Şablon Kod Örneği

Yeni bir seeder için temel şablon:

```csharp
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    public class YeniVeriSeeder : ISeeder
    {
        public int Order => (int)SeederOrder.YeniVeri; // Enum'da tanımlayın
        
        public async Task SeedAsync(AppDbContext context)
        {
            // Mevcut veri kontrolü
            if (await context.YeniVeriTablosu.AnyAsync())
            {
                return; // Zaten veri varsa işlem yapma
            }
            
            // Bağımlılık kontrolü
            var devices = await context.Devices.ToListAsync();
            if (!devices.Any())
            {
                throw new InvalidOperationException("Cihazlar oluşturulmadan YeniVeri eklenemez!");
            }
            
            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [YeniVeriTablosu] ON;");
            
            // Veri ekleme
            foreach (var device in devices)
            {
                queryBuilder.AppendLine(
                    $"INSERT INTO [YeniVeriTablosu] ([Id], [DeviceId], [Ad], [Deger]) VALUES " +
                    $"({devices.IndexOf(device) + 1}, {device.Id}, 'Örnek {device.Name}', 42.5);"
                );
            }
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [YeniVeriTablosu] OFF;");
            
            // SQL komutunu çalıştır
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Context cache'ini temizle
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
```

## 9. İleri Düzey Konular

### 9.1 Rastgele Test Verileri

Gerçekçi test verileri oluşturmak için:

- **Bogus** kütüphanesi: Sahte veriler oluşturmak için kullanılabilir
- **Random değerler**: Değişken veriler için random generator kullanın
- **Gerçekçi veri kümesi**: Gerçek senaryoları yansıtan veri aralıkları tanımlayın

### 9.2 Ortam Bazlı Seed Verisi

Farklı ortamlara (Development, Test, Production) özgü seed verileri oluşturmak için:

```csharp
public async Task SeedAsync(AppDbContext context)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    
    switch (environment)
    {
        case "Development":
            await SeedDevelopmentDataAsync(context);
            break;
        case "Staging":
            await SeedStagingDataAsync(context);
            break;
        case "Production":
            // Prod ortamında seed işlemi yapmayın veya minimum veri ekleyin
            await SeedMinimalDataAsync(context);
            break;
    }
}
```

### 9.3 Veri Seti Dağıtımı

Büyük projelerde seed verilerinin yönetilmesi:

- **JSON/XML veri dosyaları**: Seed verilerini kod dışında saklama
- **Konteyner imajları**: Seed verileriyle birlikte dağıtım
- **CI/CD entegrasyonu**: Otomatik build süreçlerinde seed verilerini oluşturma

## 10. En İyi Uygulamalar

Seed işlemleri için önerilen en iyi uygulamalar:

1. **Minimalist Yaklaşım**: Sadece ihtiyaç duyulan ve test için anlamlı verileri ekleyin
2. **İdempotent Tasarım**: Aynı seeder'ı birden fazla kez çalıştırabilme yeteneği
3. **Performans Odaklı**: Büyük veri setleri için toplu ekleme yöntemleri kullanın
4. **Sıra Yönetimi**: Bağımlılık grafini dikkatlice tasarlayın ve uygun Order değerleri atayın
5. **Detaylı Loglama**: Seeder'larda kapsamlı hata ayıklama bilgileri ekleyin
6. **Güvenli Veri**: Test verileri gerçek kredensiyaller veya hassas kişisel veriler içermemeli
7. **Test Kapsamı**: Seed verileri, uygulama akışının tüm önemli alanlarını test edebilmeli

---

## Kaynaklar

- [Entity Framework Core Veri Seed Dokümantasyonu](https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [SQL Server Bulk Insert Performans Optimizasyonu](https://docs.microsoft.com/en-us/sql/t-sql/statements/bulk-insert-transact-sql)
- [Bogus - Sahte Veri Üretim Kütüphanesi](https://github.com/bchavez/Bogus)

---

[◀ Mimari Yapı](06-Mimari-Yapi.md) | [Ana Sayfa](README.md) 