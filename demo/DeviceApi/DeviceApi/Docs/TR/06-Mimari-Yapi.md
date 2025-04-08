# Mimari Yapı

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu belge, DeviceApi'nin anahtar bileşenleri, tasarım desenleri ve bunlar arasındaki ilişkileri de dahil olmak üzere mimarisini açıklamaktadır.

## Mimari Genel Bakış

DeviceApi, kaygıları ayıran ve bakım yapılabilirliği, test edilebilirliği ve ölçeklenebilirliği teşvik eden temiz, çok katmanlı bir mimari kullanır. Uygulama, Domain-Driven Design (DDD) prensiplerine uyar ve Clean Architecture yaklaşımını kullanır.

### Yüksek Seviye Mimari Diyagram

```
┌─────────────────────────────────────────────────────────┐
│                     İstemci Uygulamaları                │
│    (Web, Mobil, IoT Cihazları, Üçüncü Taraf Uyg.)      │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                         API Katmanı                      │
│          (Kontrolcüler, Middleware, Filtreler)          │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                     Uygulama Katmanı                     │
│         (Servisler, DTO'lar, Doğrulama, Mapping)        │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                       Domain Katmanı                     │
│  (Varlıklar, Değer Nesneleri, Domain Servisleri, Events)│
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                   Altyapı Katmanı                        │
│  (Depolar, Harici Servisler, Kalıcılık, G/Ç)            │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                    Harici Sistemler                      │
│     (Veritabanları, Mesaj Aracıları, Harici Servisler)  │
└─────────────────────────────────────────────────────────┘
```

## Katmanlar ve Bileşenler

### 1. API Katmanı

Bu, sistemle tüm dış etkileşimlerin giriş noktasıdır. HTTP isteklerini işler, bunları uygun işleyicilere yönlendirir ve yanıtları dönüştürür.

#### Ana Bileşenler:

- **Kontrolcüler**: Gelen HTTP isteklerini işler ve uygun yanıtları döndürür
- **Middleware**: İstekleri ve yanıtları işler (kimlik doğrulama, loglama, istisna yönetimi)
- **Filtreler**: Doğrulama, önbellekleme ve performans izleme gibi kesişen kaygıları uygular
- **API Modelleri**: API uç noktalarına özgü giriş ve çıkış modelleri

#### Tasarım İlkeleri:

- Kontrolcüler ince tutulur, iş mantığını Uygulama Katmanına devreder
- Uygun kaynak adlandırma ve HTTP metotları ile RESTful tasarım
- Geriye dönük uyumluluğu sağlamak için sürümlendirme stratejisi
- Tutarlı hata işleme ve yanıt formatları

### 2. Uygulama Katmanı

Bu katman, iş kurallarını içermeden veri akışını düzenler ve iş operasyonlarını koordine eder.

#### Ana Bileşenler:

- **Servisler**: Domain nesnelerini koordine ederek uygulama kullanım durumlarını uygular
- **DTO'lar (Veri Transfer Nesneleri)**: Katmanlar arasında veri transfer etmek için nesneler
- **Doğrulayıcılar**: İşlemeden önce veri geçerliliğini sağlar
- **Mappers**: Domain modelleri ve DTO'lar arasında dönüşüm yapar
- **Komut/Sorgu İşleyicileri**: CQRS kullanılıyorsa, komutları ve sorguları işler

#### Tasarım İlkeleri:

- Servisler iş mantığı içermez ancak domain nesnelerini koordine eder
- Her servisin belirli bir sorumluluğu vardır (SRP)
- İşlemler bu seviyede yönetilir
- Domain olayları burada gönderilir ve işlenir

### 3. Domain Katmanı

Bu, uygulamanın kalbidir ve iş kurallarını, domain mantığını ve varlıkları içerir.

#### Ana Bileşenler:

- **Varlıklar**: Kimliği ve yaşam döngüsü olan iş nesneleri
- **Değer Nesneleri**: Domain'in yönlerini tanımlayan değiştirilemez nesneler
- **Domain Servisleri**: Doğal olarak varlıklara ait olmayan domain işlemlerini kapsüller
- **Domain Olayları**: Domain içindeki önemli olayları temsil eder
- **Aggregate'ler**: Bir birim olarak ele alınan varlık ve değer nesneleri kümeleri
- **Arayüzler**: Depolar ve servisler için sözleşmeleri tanımlar

#### Tasarım İlkeleri:

- Varlıklarda kapsüllenmiş iş mantığı ile zengin domain modeli
- Aggregate'ler tutarlılık sınırlarını uygular
- Domain katmanı diğer katmanlardan ve dış kaygılardan bağımsızdır
- İş kuralları kodda açıkça ifade edilir

### 4. Altyapı Katmanı

Bu katman, harici sistem erişimi, kalıcılık ve teknik hizmetler için uygulamalar sağlar.

#### Ana Bileşenler:

- **Depolar**: Domain varlıkları için veri erişimini uygular
- **Unit of Work**: İşlemleri yönetir ve tutarlılığı sağlar
- **Harici Servis İstemcileri**: Harici API'ler ve servislerle etkileşime girer
- **Önbellekleme**: Önbellekleme stratejilerini uygular
- **Mesajlaşma**: Mesaj yayınlama ve aboneliği ele alır
- **Kalıcılık**: Veritabanı erişimi ve ORM yapılandırmalarını uygular

#### Tasarım İlkeleri:

- Depolar, kalıcılık detaylarını domain'den soyutlar
- Altyapı uygulamaları, domain'de tanımlanan arayüzlere dayanır
- Önbellekleme, loglama ve mesajlaşma gibi kaygılar burada uygulanır
- Daha yüksek katmanlara uygulamalar sağlamak için bağımlılık enjeksiyonu

## Kesişen Kaygılar

### Kimlik Doğrulama ve Yetkilendirme

DeviceApi, kimlik doğrulama için JWT (JSON Web Tokens) ve rol tabanlı bir yetkilendirme sistemi kullanır:

- **Kimlik Doğrulama Middleware'i**: Tokenleri doğrular ve kimliği belirler
- **Yetkilendirme Politikaları**: Çeşitli işlemler için izinleri tanımlar
- **Rol Sağlayıcıları**: Kullanıcıları roller ve izinlerle eşleştirir

### Önbellekleme Stratejisi

Performansı optimize etmek için çok seviyeli bir önbellekleme yaklaşımı kullanılır:

- **Bellek İçi Önbellek**: Sık erişilen, kısa ömürlü veriler için
- **Dağıtılmış Önbellek**: Kümelenmiş bir ortamda paylaşılan veriler için
- **Entity Framework İkinci Seviye Önbellek**: Veritabanı sorgu sonuçları için

### Loglama ve İzleme

Kapsamlı loglama ve izleme şunları kullanarak uygulanır:

- **Yapılandırılmış Loglama**: Tutarlı log girişleri için Serilog ile
- **Sağlık Kontrolleri**: Sistem ve bağımlılık durumunu doğrulamak için
- **Performans Metrikleri**: Anahtar performans göstergelerini izlemek için
- **Dağıtılmış İzleme**: Servisler arasında istekleri izlemek için

### Hata Yönetimi

Merkezi bir hata yönetimi stratejisi:

- **İstisna Middleware'i**: İstisnaları yakalar ve işler
- **Problem Detayları Formatı**: RFC 7807'yi takip eden standartlaştırılmış hata yanıtları
- **Hata Kodları**: İstemci yorumlaması için tutarlı hata kodları

## Veri Akışı

### İstek İşleme Akışı

1. İstemci API'ye bir HTTP isteği gönderir
2. Middleware isteği işler (kimlik doğrulama, loglama)
3. Kontrolcü isteği alır ve girişleri doğrular
4. İstek bir komut veya sorguya eşlenir
5. Uygun uygulama servisi çağrılır
6. Servis, depolar ve domain servislerini kullanarak domain işlemlerini düzenler
7. Domain mantığı çalışır, muhtemelen domain olayları tetikler
8. Sonuçlar API modellerine geri eşlenir
9. Kontrolcü uygun bir HTTP yanıtı döndürür

### Veri Kalıcılık Akışı

1. Domain işlemleri varlıkları oluşturur veya değiştirir
2. Uygulama servisleri Unit of Work aracılığıyla değişiklikleri taahhüt eder
3. Depolar domain nesnelerini veri modeline dönüştürür
4. Entity Framework (veya diğer ORM) uygun SQL'i oluşturur
5. Veritabanı değişiklikleri bir işlemde taahhüt edilir
6. Domain olayları ek işlemler tetikleyebilir
7. Önbellekler güncellenir veya geçersiz kılınır

## Teknoloji Yığını

DeviceApi aşağıdaki teknolojiler kullanılarak oluşturulmuştur:

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: Microsoft SQL Server (diğerlerine olası geçiş yolları ile)
- **Önbellekleme**: Memory Cache ve Redis
- **Kimlik Doğrulama**: Yenileme tokenleri ile JWT
- **Doğrulama**: FluentValidation
- **Nesne Mapping**: AutoMapper
- **Dokümantasyon**: Swagger/OpenAPI
- **Mesajlaşma**: Azure Service Bus veya RabbitMQ ile opsiyonel entegrasyon
- **Test**: xUnit, Moq ve FluentAssertions

## Dağıtım Mimarisi

DeviceApi birden fazla dağıtım modelini destekler:

### Tek Sunucu Dağıtımı

Küçük ve orta ölçekli dağıtımlar için uygulama tek bir sunucuda çalışabilir:

- API uygulaması
- SQL Server veritabanı
- Redis önbellek

### Mikroservis Tabanlı Dağıtım

Daha büyük dağıtımlar için DeviceApi mikroservislere ayrılabilir:

- **Kimlik Servisi**: Kimlik doğrulama ve kullanıcı yönetimini ele alır
- **Cihaz Kayıt Servisi**: Cihaz kaydı ve meta verileri yönetir
- **Telemetri Servisi**: Cihaz verilerini işler ve depolar
- **Analitik Servisi**: Veri analizi ve raporlama sağlar
- **Bildirim Servisi**: Uyarıları ve bildirimleri yönetir

### Bulut Dağıtımı

DeviceApi, bulut ortamlarında iyi çalışacak şekilde tasarlanmıştır:

- Docker ile konteynerizasyon
- Kubernetes ile orkestrasyon
- Depolama, önbellekleme ve mesajlaşma için bulut temelli servisler
- Yüke dayalı otomatik ölçeklendirme
- Global dağıtımlar için coğrafi dağıtım

## Geliştirme Hususları

### Genişletilebilirlik

DeviceApi genişleme için tasarlanmıştır:

- **Eklenti Mimarisi**: Özel cihaz türleri ve protokoller için
- **Özel İşleyiciler**: Özelleştirilmiş veri işleme için
- **API Uzantıları**: Yeni uç noktaların eklenmesi için iyi tanımlanmış noktalar
- **Middleware Hattı**: Özel işleme için genişletilebilir

### Ölçeklenebilirlik

Ölçeklenebilirlik için çeşitli stratejiler kullanılır:

- **Durumsuz Tasarım**: Yatay ölçeklendirmeye izin verir
- **Asenkron İşleme**: Tüm sistemde bloklamayan G/Ç
- **Bağlantı Havuzu**: Verimli kaynak kullanımı
- **Geri Basınç İşleme**: Sistem aşırı yüklenmesini önler
- **Veri Bölümleme**: Çok büyük veri setleri için

### Test Edilebilirlik

Mimari, kapsamlı testleri kolaylaştırır:

- **Birim Testleri**: Domain mantığı ve servisler için
- **Entegrasyon Testleri**: Depolar ve harici entegrasyonlar için
- **API Testleri**: Uç nokta doğrulaması için
- **Performans Testleri**: Yük ve stres testi için
- **Taklit Etme (Mocking)**: Sistemdeki arayüzler taklit etmeye olanak tanır

## Veritabanı Başlatma (Seeding) Mimarisi

DeviceApi, gelişmiş bir veritabanı seed mekanizması kullanır. Bu mekanizma, uygulama ilk başlatıldığında veya veritabanı yeniden yapılandırıldığında test, geliştirme ve demo amaçlı verilerin otomatik olarak oluşturulmasını sağlar.

### Seed Bileşenleri

Seeding mimarisi aşağıdaki bileşenlerden oluşur:

1. **ISeeder Interface**: Tüm seeder sınıflarının uygulaması gereken temel sözleşme
2. **DatabaseSeeder**: Tüm seeder'ları koordisyon içinde çalıştıran ana sınıf
3. **Spesifik Seeder Sınıfları**: Her bir veri modeli için özelleştirilmiş seeder'lar
4. **SeederOrder Enum**: Seed işlemlerinin çalışma sırasını belirleyen sabitler
5. **SeederExtensions**: Seed işlemlerini kolaylaştıran yardımcı metodlar

### Çalışma Prensibi

Seeding mimarisi aşağıdaki prensiplere göre çalışır:

1. **Sıralı Çalışma**: Seeder'lar, veritabanı ilişkilerini doğru şekilde kurmak için belirli bir sırada çalışır
2. **İdempotent Yapı**: Seed işlemleri tekrar edilebilir, aynı veri birden fazla kez eklenmez
3. **Hiyerarşik İlişkiler**: Veri modeli hiyerarşisine uygun olarak (İstasyon->Platform->Cihaz) yukarıdan aşağıya doğru veri oluşturulur
4. **Performans Optimizasyonu**: Toplu veri ekleme ve SQL düzeyinde IDENTITY_INSERT kullanarak hızlı veri yükleme

### Mimari Akış

Seed işlem sürecinin mimarisi şu şekilde tasarlanmıştır:

```
┌─────────────────────┐       ┌───────────────────┐
│                     │       │                   │
│  Program.cs         │       │  appsettings.json │
│  (Seed başlatıcı)   │       │  (Yapılandırma)   │
│                     │       │                   │
└──────────┬──────────┘       └─────────┬─────────┘
           │                            │
           │                            │
           ▼                            ▼
┌─────────────────────────────────────────────────┐
│                                                 │
│              DatabaseSeeder                     │
│        (Seeder'ları bulma ve çalıştırma)       │
│                                                 │
└───────────────────────┬─────────────────────────┘
                        │
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌────────────────┐ ┌────────────┐ ┌────────────────┐
│                │ │            │ │                │
│ Temel Seeder'lar│ │ Ana Veriler│ │ İlişkisel      │
│ (Enum, Sabitler)│ │ (İstasyon) │ │ Veriler        │
│                │ │            │ │ (Cihaz, Mesaj) │
└────────────────┘ └────────────┘ └────────────────┘
```

### Entegrasyon Noktaları

Seed mimarisi şu bileşenlerle entegre olur:

- **EF Core**: Seed verileri EF Core context üzerinden veya doğrudan SQL ile eklenir
- **Logging**: Seed işlemleri LogLibrary ile detaylı şekilde loglanır
- **Dependency Injection**: DatabaseSeeder DI sistemi üzerinden erişilebilir
- **Yapılandırma**: Seed davranışı appsettings.json üzerinden yapılandırılabilir

Kapsamlı seed mimarisi hakkında daha fazla bilgi için: [Veritabanı Seeding Süreci](../TR/Seeding-Sureci.md)

---

[◀ Veri Modelleri](05-Veri-Modelleri.md) | [Ana Sayfa](README.md) | [İleri: Yapılandırma ▶](07-Yapilandirma.md) 