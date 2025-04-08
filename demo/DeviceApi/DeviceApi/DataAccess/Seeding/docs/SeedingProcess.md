# Veri Seeding İşlem Sırası

Bu döküman, uygulamadaki veritabanı seed işlemlerinin hangi sıra ile gerçekleştirildiğini açıklar. Seeder sınıfları, `ISeeder` arayüzünü implemente eder ve sıralı çalışması `Order` özelliği ile belirlenir.

## Seeder Çalışma Sırası

Seeder sınıfları aşağıdaki sıra ile çalışır:

| Sıra | Seeder Adı | Order Değeri | Açıklama |
| ---- | ---------- | ------------ | -------- |
| 1 | StationSeeder | 3 | İstasyon verilerini oluşturur |
| 2 | PlatformSeeder | 4 | Platform verilerini oluşturur (İstasyonlara bağlı) |
| 3 | PredictionSeeder | 5 | Tren tahmin verilerini oluşturur (Platformlara bağlı) |
| 4 | DeviceSeeder | 5 | Cihaz verilerini oluşturur (Platformlara bağlı) |
| 5 | DeviceSettingSeeder | 6 | Cihaz ayarlarını oluşturur (Cihazlara bağlı) |
| 6 | FullScreenMessageSeeder | 40 | Tam ekran mesajları oluşturur (Cihazlara bağlı) |
| 7 | ScrollingScreenMessageSeeder | 41 | Kayan ekran mesajları oluşturur (Cihazlara bağlı) |
| 8 | BitmapScreenMessageSeeder | 42 | Bitmap ekran mesajları oluşturur (Cihazlara bağlı) |
| 9 | PeriodicMessageSeeder | 43 | Periyodik mesajları oluşturur (Cihazlara bağlı) |

## Seeder İşlem Akışı

Seeding işlemi şu adımlarla gerçekleşir:

1. `DatabaseSeeder` sınıfı, tüm `ISeeder` tipindeki sınıfları bulur
2. Seeder'ları `Order` değerine göre küçükten büyüğe sıralar
3. Her seeder sırayla çalıştırılır
4. Her seeder çalıştırılmadan önce veri olup olmadığını kontrol eder
5. Eğer ilgili tabloda veri yoksa, seed verilerini ekler

## Seeder Detayları

### 1. StationSeeder (Order: 3)
- İstasyon kayıtlarını oluşturur
- Merkez, Doğu ve Batı istasyonları ekler
- SQL komutu ile `IDENTITY_INSERT` kullanarak ID'leri belirler

### 2. PlatformSeeder (Order: 4)
- Platform kayıtlarını oluşturur
- Her istasyon için farklı platformlar ekler
- StationSeeder'dan sonra çalışır çünkü platformlar istasyonlara bağlıdır
- SQL komutu ile `IDENTITY_INSERT` kullanarak ID'leri belirler

### 3. PredictionSeeder (Order: 5)
- Tren tahmin verilerini oluşturur
- Her platform için örnek tahmin verileri ekler (bazı tahminer null değer içerir)
- PlatformSeeder'dan sonra çalışır çünkü tahminler platformlara bağlıdır
- SQL komutu ile `IDENTITY_INSERT` kullanarak ID'leri belirler

### 4. DeviceSeeder (Order: 5)
- Cihaz kayıtlarını oluşturur
- Her platform için 2 cihaz ekler
- PlatformSeeder'dan sonra çalışır çünkü cihazlar platformlara bağlıdır
- SQL komutu ile `IDENTITY_INSERT` kullanarak ID'leri belirler

### 5. DeviceSettingSeeder (Order: 6)
- Cihaz ayarlarını oluşturur
- İlk 3 cihaz için ayarlar ekler
- DeviceSeeder'dan sonra çalışır çünkü ayarlar cihazlara bağlıdır
- SQL komutu ile `IDENTITY_INSERT` kullanarak ID'leri belirler

### 6. FullScreenMessageSeeder (Order: 40)
- Tam ekran mesajları oluşturur
- İlk 3 cihaz için farklı mesajlar ekler
- DeviceSeeder'dan sonra çalışır çünkü mesajlar cihazlara bağlıdır
- Farklı Türkçe ve İngilizce mesaj içerikleri ile örnek veriler oluşturur

### 7. ScrollingScreenMessageSeeder (Order: 41)
- Kayan ekran mesajları oluşturur
- İlk 3 cihaz için farklı mesajlar ekler
- FullScreenMessageSeeder'dan sonra çalışır
- Her cihaz için Türkçe ve İngilizce tek satırlık kayan mesaj içerikleri oluşturur
- Metro kuralları ve bilgilerini içeren örnek metinler ekler

### 8. BitmapScreenMessageSeeder (Order: 42)
- Bitmap ekran mesajları oluşturur
- İlk 3 cihaz için farklı mesajlar ekler
- ScrollingScreenMessageSeeder'dan sonra çalışır
- Her cihaz için Türkçe ve İngilizce bitmap formatında görsel içerikleri oluşturur
- Base64 kodlu bitmap görselleri içeren örnek veriler ekler

### 9. PeriodicMessageSeeder (Order: 43)
- Periyodik mesajları oluşturur
- İlk 3 cihaz için durum ve sensör verilerini içeren mesajlar ekler
- BitmapScreenMessageSeeder'dan sonra çalışır
- Her cihaz için sıcaklık, nem, gaz seviyesi, ışık seviyeleri, LED arızaları, kabin durumu, fan durumu, gösterge durumu, RS232 durumu ve güç kaynağı durumu içeren veriler ekler
- Cihaz durum ve sensör bilgilerini içeren örnek periyodik mesajlar oluşturur

## Veri İlişkileri

```
İstasyon (Station) 1─┐
                     ├── Platform 1───┬─── Prediction 1 (3 tren bilgisi)
                     │                ├─── Cihaz 1 ────┬─── Cihaz Ayarları 1
                     │                │                ├─── Tam Ekran Mesaj 1
                     │                │                ├─── Kayan Ekran Mesaj 1
                     │                │                ├─── Bitmap Ekran Mesaj 1
                     │                │                └─── Periyodik Mesaj 1
                     │                └─── Cihaz 2 ────┬─── Cihaz Ayarları 2
                     │                                 ├─── Tam Ekran Mesaj 2
                     │                                 ├─── Kayan Ekran Mesaj 2
                     │                                 ├─── Bitmap Ekran Mesaj 2
                     │                                 └─── Periyodik Mesaj 2
                     │
                     └── Platform 2───┬─── Prediction 2 (2 tren bilgisi)
                                      ├─── Cihaz 3 ────┬─── Cihaz Ayarları 3
                                      │                ├─── Tam Ekran Mesaj 3
                                      │                ├─── Kayan Ekran Mesaj 3
                                      │                ├─── Bitmap Ekran Mesaj 3
                                      │                └─── Periyodik Mesaj 3
                                      └─── Cihaz 4 ────── Cihaz Ayarları 4
```

## Önemli Notlar

- Seed işlemi yalnızca veritabanı yeniden oluşturulduğunda ya da ilgili tablolarda hiç veri yoksa çalışır
- Tüm seeder'lar SQL komutları kullanarak veri ekler ve `IDENTITY_INSERT` açılıp kapatılır
- Her seeder, ilişkili olduğu tablolar için gerekli verilerin önceden oluşturulduğundan emin olur
- Seed işlemi, `Order` değerlerine göre sırayla çalıştırılır, böylece veri bütünlüğü korunur

## Yeni Seeder Ekleme

Yeni bir seeder eklemek için:

1. `ISeeder` arayüzünü implement eden bir sınıf oluşturun
2. `Order` özelliğini, bu seeder'ın çalışma sırasına göre ayarlayın
3. `SeedAsync` metodunda:
   - İlgili tabloda veri olup olmadığını kontrol edin
   - Gerekli bağımlılıkların (diğer tablolardaki veriler) oluşturulduğundan emin olun
   - SQL komutu ile veri ekleyin
   - Context'i temizleyin

Örnek:

```csharp
public class YeniSeeder : ISeeder
{
    public int Order => 7; // DeviceSettingSeeder'dan sonra çalışsın
    
    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.YeniTablo.AnyAsync())
        {
            return; // Zaten veri varsa işlem yapma
        }
        
        // SQL komutu oluşturma ve çalıştırma
        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SET IDENTITY_INSERT [YeniTablo] ON;");
        // ...
        queryBuilder.AppendLine("SET IDENTITY_INSERT [YeniTablo] OFF;");
        
        await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
        
        // Context cache'ini temizle
        foreach (var entry in context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
} 