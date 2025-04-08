# Veritabanı Seeding İşlem Akışı ve Uygulama Rehberi

Bu belge, DeviceApi projesindeki veritabanı seed işlemlerinin detaylı yapısını, çalışma prensibini, uygulama yöntemlerini ve sorun giderme yöntemlerini açıklar. Seed işlemi, uygulamanın ilk kurulumunda veya veritabanının yeniden yapılandırılması durumunda kullanılacak test ve demo verilerinin otomatik olarak oluşturulmasını sağlar.

## 1. Seeding Mimarisi

### 1.1 Temel Bileşenler

Seeding sistemi aşağıdaki ana bileşenlerden oluşur:

- **ISeeder İnterface**: Tüm seed sınıfları tarafından implemente edilen temel arayüz
- **SeederOrder**: Seed işlemlerinin çalışma sırasını belirleyen numaralandırma
- **DatabaseSeeder**: Tüm seed işlemlerini koordine eden ve çalıştıran ana sınıf
- **Spesifik Seeder Sınıfları**: Her bir veri modeli için özelleştirilmiş seed işlemlerini gerçekleştiren sınıflar
- **SeederExtensions**: Seed işlemlerini kolaylaştıran yardımcı extension metodları

### 1.2 ISeeder İnterface

Tüm seeder sınıflarının uygulaması gereken temel arayüz aşağıdaki gibidir:

```csharp
public interface ISeeder
{
    int Order { get; }
    Task SeedAsync(AppDbContext context);
}
```

- **Order**: Seed işleminin çalışma sırasını belirleyen öncelik değeri
- **SeedAsync**: Veritabanına seed verilerini ekleyen asenkron metod

## 2. Seeder Çalışma Sırası ve Bağımlılıklar

### 2.1 Çalışma Sırası Tablosu

Seeder sınıfları aşağıdaki sıra ve bağımlılıklarla çalışır:

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

### 2.2 Bağımlılık Grafiği

Seeder'ların çalışma sırası ve birbirlerine olan bağımlılıkları aşağıdaki grafik ile gösterilmektedir:

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

## 3. Seeder İşlem Akışı

### 3.1 Ana İşlem Akışı

Seeding işlemi şu adımlarla gerçekleşir:

1. `Program.cs` içerisinde `DatabaseSettings:ResetDatabaseOnStartup` değeri kontrol edilir
2. Yeniden yapılandırma gerekiyorsa, `DatabaseSeeder.SeedAsync()` metodu çağrılır
3. `DatabaseSeeder` sınıfı, yansıma (reflection) kullanarak tüm `ISeeder` tipindeki sınıfları bulur
4. Seeder'lar `Order` değerine göre küçükten büyüğe sıralanır
5. Her seeder sırayla çalıştırılır ve işlem detaylı şekilde loglanır
6. Her seeder çalıştırılmadan önce veri olup olmadığını kontrol eder
7. Eğer ilgili tabloda veri yoksa, seed verileri eklenir

### 3.2 Teknik Detaylar

#### 3.2.1 Veri Ekleme Stratejileri

Seed işlemlerinde temel olarak iki yaklaşım kullanılır:

1. **SQL Komutları ile Doğrudan Veri Ekleme**:
   - IDENTITY_INSERT kullanılarak ID'lerin korunması sağlanır
   - Toplu veri eklemede daha yüksek performans sunar
   - İlişkisel verilerin daha kolay yönetilmesini sağlar

2. **Entity Framework Üzerinden Veri Ekleme**:
   - Daha az kod gerektirir, ancak ID kontrolü daha zordur
   - EF Core'un Change Tracking özelliği ile tutarlılık sağlanır
   - Karmaşık nesnelerde kullanışlıdır

#### 3.2.2 Context Temizleme

Her seeder çalıştıktan sonra, Entity Framework'ün izleme (tracking) mekanizmasının temizlenmesi önemlidir:

```csharp
// Context cache'ini temizle
foreach (var entry in context.ChangeTracker.Entries())
{
    entry.State = EntityState.Detached;
}
```

Bu işlem, farklı seeder'lar arasında olası çakışmaları önler ve bellek kullanımını optimize eder.

## 4. Seeder Detayları ve Özelleştirilmiş Veri Oluşturma

### 4.1 AlignmentTypeSeeder (Order: 1)
- Hizalama türlerini (sola, sağa, ortaya ve tam) tanımlar
- Diğer seed sınıflarından önce çalışır, çünkü temel bir referans tablosudur
- Yeni hizalama türleri eklenirse, burada tanımlanmalıdır

### 4.2 StationSeeder (Order: 3)
- Ana veri yapısının temelini oluşturan istasyon kayıtlarını ekler
- 3 temel istasyon oluşturur: Merkez, Doğu ve Batı istasyonları 
- Her istasyon için konum (enlem/boylam) ve açıklama tanımlar
- SQL komutu ile ID'leri garantileyerek ekleme yapar

### 4.3 PlatformSeeder (Order: 4)
- Her istasyon için 1-3 platform oluşturur
- Platformlar için konum bilgilerini ve yön tanımlarını içerir
- İstasyon-Platform ilişkisini foreign key ile sağlar
- Platformların yön ve durum bilgilerini içeren açıklamaları ekler

### 4.4 PredictionSeeder (Order: 5)
- Her platform için 1-3 tren tahmini oluşturur
- Gün içindeki farklı zaman dilimlerini içeren tahminler ekler
- Rötarlı, zamanında ve iptal edilmiş tren bilgileri içerir
- Gerçekçi durum, güncelleme zamanları ve varyasyonlar içerir

### 4.5 DeviceSeeder (Order: 5)
- Her platform için 1-2 cihaz tanımlar
- IP adresleri, portlar ve cihaz konumları gerçekçi verilerle oluşturulur
- Her cihaz için platform ilişkisi kurulur
- Farklı cihaz tipleri ve isimler tanımlanır (İstikmal LEDs, Platform Displays, vb.)

### 4.6 DeviceSettingSeeder (Order: 6)
- Her cihaz için temel ayarlar oluşturur
- Network ayarları (APN, Server IP, TCP/UDP portları)
- FTP durumu ve güvenlik ayarları
- Temel çalışma parametreleri

### 4.7 MessageSeeder'lar (Order: 40-43)
- FullScreenMessageSeeder: Tam ekran acil durum, bilgilendirme mesajları
- ScrollingScreenMessageSeeder: Tek satırlık kayan metro bilgilendirme metinleri
- BitmapScreenMessageSeeder: Özel görüntü ve logo içeren mesajlar
- PeriodicMessageSeeder: Cihaz durum ve sensör verilerini içeren periyodik mesajlar

## 5. Veri İlişkileri ve Örnek Veri Yapısı

```
İstasyon (Station) 1─┐
                     ├── Platform 1───┬─── Prediction 1, 2, 3 (Farklı trenler)
                     │                ├─── Cihaz 1 ────┬─── Cihaz Ayarları 1
                     │                │                ├─── Tam Ekran Mesaj 1
                     │                │                ├─── Kayan Ekran Mesaj 1
                     │                │                ├─── Bitmap Ekran Mesaj 1
                     │                │                └─── Periyodik Mesaj 1
                     │                └─── Cihaz 2 ────┬─── Cihaz Ayarları 2
                     │                                 ├─── Tam Ekran Mesaj 2
                     │                                 └─── ...
                     │
                     └── Platform 2───┬─── Prediction 4, 5 (Farklı trenler)
                                      ├─── Cihaz 3 ────┬─── ...
                                      └─── Cihaz 4 ────┴─── ...

İstasyon (Station) 2─┐
                     ├── Platform 3───┬─── ...
                     └── Platform 4───┴─── ...

İstasyon (Station) 3─┐
                     └── Platform 5───┬─── ...
```

## 6. Performans Optimizasyonu

### 6.1 Toplu Veri Ekleme
- Büyük veri setlerinde SQL batch işlemleri kullanılır
- Tek seferde çok sayıda kayıt eklemek için StringBuilder ile SQL sorguları oluşturulur
- Sorgu parametreleri için SQL enjeksiyon koruması uygulanır

### 6.2 Önbelleğe Alma Stratejileri
- EF Core context yeniden kullanımı yerine her seeder'da bağlamsız SQL ekleme
- İç içe ilişkili veri yapılarında ID referanslarının korunması
- Özellikle büyük veri setlerinde ilişkisel eşleştirme için ID'lerin önbelleğe alınması

## 7. Sorun Giderme ve Yaygın Hatalar

### 7.1 Genel Sorun Giderme

- **Foreign Key Kısıtlaması Hataları**: İlişkili tablolar yanlış sırayla doldurulduğunda oluşur
- **Duplicate Key Hataları**: Aynı ID değerine sahip kayıtlar eklenmeye çalışıldığında oluşur
- **Bağlantı Zaman Aşımı**: Büyük veri setlerinde uzun süren işlemlerde görülür

### 7.2 Loglar ve Tanılama

Seeding işlemi sırasında kapsamlı logging yapılmaktadır:

- Her seeder başlangıcı ve bitişi loglanır
- Hata durumları detaylı şekilde kaydedilir
- Seed tamamlandığında özet bilgiler loglanır

### 7.3 Seed İşlemini Zorla Çalıştırma

Geliştirme ortamında seed işlemini zorla çalıştırmak için:

1. `appsettings.json` veya `appsettings.Development.json` dosyasını açın
2. `DatabaseSettings:ResetDatabaseOnStartup` değerini `true` olarak ayarlayın
3. Uygulamayı yeniden başlatın

## 8. Yeni Seeder Ekleme Rehberi

### 8.1 Temel Adımlar

Yeni bir seeder eklemek için izlenmesi gereken adımlar:

1. `ISeeder` arayüzünü implement eden yeni bir sınıf oluşturun
2. `Order` özelliğini, veri bağımlılıklarınıza göre uygun değere ayarlayın
3. `SeedAsync` metodunu implementasyon edin
4. `SeederOrder` enum'ına yeni seeder'ınız için bir değer ekleyin
5. İlgili seeder'ı ilişkili veri testleriyle test edin

### 8.2 Şablon Kod

Yeni bir seeder için örnek şablon kod:

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
            
            // Bağımlılık kontrolü - örnek olarak cihazların önceden yaratılmış olması
            var devices = await context.Devices.ToListAsync();
            if (!devices.Any())
            {
                throw new InvalidOperationException("Cihazlar oluşturulmadan YeniVeri eklenemez!");
            }
            
            // SQL komutu oluşturma
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [YeniVeriTablosu] ON;");
            
            // Veri satırları
            foreach (var device in devices)
            {
                queryBuilder.AppendLine($"INSERT INTO [YeniVeriTablosu] ([Id], [DeviceId], [Ad], [Deger], [OlusturmaTarihi]) VALUES");
                queryBuilder.AppendLine($"({devices.IndexOf(device) + 1}, {device.Id}, 'Örnek Veri {device.Name}', 42.5, GETDATE()),");
            }
            
            // SQL query sonlandırma
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

## 9. İleri Seviye Konular

### 9.1 Rastgele Veri Üretimi

Gerçekçi test verileri oluşturmak için:

- Bogus kütüphanesi ile rastgele veri oluşturma
- Gerçekçi tarih, konum, IP adresleri üretimi
- Tutarlı veri ilişkileri sağlama teknikleri

### 9.2 Seed Veri Yönetimi

Büyük projelerde seed veri yönetimi:

- Harici kaynaklardan (JSON, CSV, XML) veri içe aktarma
- Ortam bazlı farklı seed stratejileri (Test, Geliştirme, Demo)
- Kademeli seed yaklaşımı (temel + opsiyonel veri setleri)

### 9.3 CI/CD Entegrasyonu

Otomatik dağıtım süreçlerinde seed verilerinin yönetimi:

- Migration ve seed işlemlerinin entegrasyonu
- Seed datanın versiyon kontrolü
- Ortama özgü (environment-specific) seed verisi yapılandırması

## 10. En İyi Uygulamalar ve Tavsiyeler

- Seed verilerini gerçek kullanım senaryolarını yansıtacak şekilde tasarlayın
- Çok büyük veri setleri yerine temsili örnekler oluşturun
- İlişkisel verilerde tutarlılığı sağlayacak bağımlılık kontrollerini ekleyin
- Seed işlemlerini performans açısından optimize edin
- Seed işlemi sırasında oluşabilecek hataları ayrıntılı loglayın
- Test senaryolarını kapsayan çeşitlilikte veri oluşturun
- Hassas verileri (şifreler, kişisel bilgiler) anonimleştirin veya test verileri kullanın
