# LogLibrary Teknik Doküman

Bu doküman, LogLibrary kütüphanesinin teknik yapısını ve mimarisini açıklar.

## Mimari Yapı

LogLibrary kütüphanesi, aşağıdaki temel katmanlardan oluşur:

1. **Core**: Temel model ve arayüzleri içerir
2. **Data**: Veritabanı erişimi ve repository sınıfları
3. **Services**: Uygulama mantığı ve servisler
4. **Configuration**: Yapılandırma ayarları
5. **Extensions**: Servis kayıtları için extension metodları

```
LogLibrary/
│
├── Core/
│   ├── Models/
│   │   └── LogEntry.cs
│   └── Interfaces/
│       ├── ILogService.cs
│       └── ILogRepository.cs
│
├── Data/
│   ├── Context/
│   │   └── MongoDbContext.cs
│   └── Repositories/
│       └── MongoLogRepository.cs
│
├── Services/
│   ├── Concrete/
│   │   └── LogService.cs
│   └── Interfaces/
│       └── ...
│
├── Configuration/
│   ├── Settings/
│   │   ├── LogSettings.cs
│   │   └── settings.json
│   └── ...
│
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

## Bileşenler

### Core Katmanı

#### LogEntry

Log kayıtlarını temsil eden model:

- **Id**: MongoDB doküman ID'si
- **Timestamp**: Log kaydının oluşturulma zamanı (UTC)
- **Level**: Log seviyesi (Info, Warning, Error, Debug, Critical)
- **Message**: Log mesajı
- **Source**: Loglamayı oluşturan kaynak (sınıf/metod adı)
- **UserId**: İşlemi gerçekleştiren kullanıcının ID'si
- **UserName**: İşlemi gerçekleştiren kullanıcının adı
- **UserEmail**: İşlemi gerçekleştiren kullanıcının email adresi
- **TraceId**: İsteğin trace ID'si (correlation için)
- **Data**: İlgili ek veri
- **IpAddress**: İsteğin geldiği IP adresi
- **ApplicationName**: Uygulama adı
- **Environment**: Çalışma ortamı
- **HttpMethod**: HTTP istek metodu
- **HttpPath**: HTTP istek yolu
- **StatusCode**: HTTP durum kodu
- **Duration**: İşlem süresi (milisaniye)

#### ILogService

Loglama işlemleri için ana arayüz:

- **LogInfoAsync**: Bilgi seviyesinde loglama
- **LogWarningAsync**: Uyarı seviyesinde loglama
- **LogErrorAsync**: Hata seviyesinde loglama
- **LogDebugAsync**: Debug seviyesinde loglama
- **LogCriticalAsync**: Kritik seviyesinde loglama
- **LogHttpAsync**: HTTP istek/yanıt loglaması
- **CleanupLogsAsync**: Eski logları temizleme
- **GetLogsAsync**: Logları sorgulama

#### ILogRepository

Veritabanı işlemleri için arayüz:

- **CreateLogAsync**: Log kaydı oluşturma
- **DeleteOldLogsAsync**: Eski logları silme
- **GetLogsAsync**: Logları sorgulama
- **DeleteAllLogsAsync**: Tüm logları silme

### Data Katmanı

#### MongoDbContext

MongoDB bağlantısını yönetir:

- MongoDB istemci bağlantısı oluşturur
- `LogEntry` koleksiyonuna erişim sağlar
- TTL indeksi oluşturur (eski logların otomatik silinmesi için)

#### MongoLogRepository

`ILogRepository` arayüzünün MongoDB implementasyonu:

- MongoDB'ye log kaydı ekler
- MongoDB'den log kayıtlarını sorgular
- MongoDB'den eski log kayıtlarını siler

### Services Katmanı

#### LogService

`ILogService` arayüzünün implementasyonu:

- Farklı log seviyelerinde loglama işlemlerini gerçekleştirir
- Log kayıtlarında hassas verileri maskeler
- HTTP isteklerini ve yanıtlarını loglar
- Yapılandırma ayarlarına göre loglama davranışını yönetir
- Asenkron loglama desteği sağlar

### Configuration Katmanı

#### LogSettings

Loglama ayarlarını tanımlar:

- MongoDB bağlantı ayarları
- Loglama davranışı ayarları
- Uygulama bilgileri
- Hassas veri maskeleme ayarları
- GrayLog ve ELK Stack entegrasyon ayarları

### Extensions Katmanı

#### ServiceCollectionExtensions

DI container için extension metodları:

- `AddLogLibrary`: Tüm LogLibrary servislerini kaydeder

## Tasarım Prensipleri

LogLibrary, aşağıdaki tasarım prensiplerine göre geliştirilmiştir:

1. **Katmanlı Mimari**: Her katmanın ayrı sorumlulukları vardır
2. **Dependency Injection**: Tüm bağımlılıklar DI container üzerinden enjekte edilir
3. **Interface-Based Programming**: Tüm servisler arayüzler üzerinden tanımlanır
4. **Configuration Over Convention**: Davranış, yapılandırma dosyası üzerinden ayarlanabilir
5. **Asynchronous Design**: Tüm veritabanı operasyonları asenkron gerçekleştirilir

## MongoDB Şeması

LogLibrary, MongoDB'de aşağıdaki şemayı kullanır:

```javascript
{
  "_id" : ObjectId("..."),
  "timestamp" : ISODate("2023-05-01T10:15:30Z"),
  "level" : "Info",
  "message" : "Kullanıcı giriş yaptı",
  "source" : "AuthController.Login",
  "userId" : "123",
  "userName" : "john.doe",
  "userEmail" : "john.doe@example.com",
  "traceId" : "abc123def456",
  "data" : {
    // İlgili veriler
  },
  "ipAddress" : "192.168.1.1",
  "applicationName" : "AuthApi",
  "environment" : "Development",
  "httpMethod" : "POST",
  "httpPath" : "/api/auth/login",
  "statusCode" : 200,
  "duration" : 125
}
```

## İndeksler

MongoDB'de aşağıdaki indeksler oluşturulur:

1. `timestamp` üzerinde TTL indeksi (eski kayıtların otomatik silinmesi için)
2. `level` üzerinde indeks (log seviyesine göre sorgulama için)
3. `userId` üzerinde indeks (kullanıcıya göre sorgulama için)
4. `source` üzerinde indeks (kaynağa göre sorgulama için)

## Performans Optimizasyonları

LogLibrary, performans için aşağıdaki optimizasyonları sağlar:

1. **Asenkron Loglama**: Loglama işlemleri asenkron olarak gerçekleştirilir, ana uygulama akışını bloklamaz
2. **Bulk Insert**: Yüksek trafik durumlarında toplu log eklemesi yapılabilir
3. **TTL İndeksi**: Eski loglar otomatik olarak silinir, manuel temizleme gerektirmez
4. **Sorgu İndeksleri**: Sık kullanılan sorgu alanları için indeksler oluşturulur

## Güvenlik Önlemleri

1. **Hassas Veri Maskeleme**: Şifre, token gibi hassas veriler loglanırken otomatik olarak maskelenir
2. **Konfigürasyon Tabanlı Kontrol**: Hangi verilerin loglanacağı yapılandırma üzerinden kontrol edilebilir
3. **Exception Handling**: Tüm exception'lar yakalanır ve uygun şekilde işlenir

## İleri Aşamalar

LogLibrary'in gelecekteki sürümlerinde aşağıdaki özellikler eklenecektir:

1. **GrayLog Entegrasyonu**: GELF protokolü ile GrayLog'a log gönderimi
2. **ELK Stack Entegrasyonu**: Elasticsearch, Logstash ve Kibana entegrasyonu
3. **Dashboard ve Raporlama**: Log analizleri için dashboard ve raporlama araçları
4. **Log Rotation**: Log döndürme ve arşivleme özellikleri
5. **Structured Logging**: Yapılandırılmış loglama desteği

## Troubleshooting

Yaygın sorunlar ve çözümleri:

1. **MongoDB Bağlantı Hatası**: MongoDB bağlantı dizesini ve MongoDB sunucusunun çalışır durumda olduğunu kontrol edin
2. **Performans Sorunları**: Asenkron loglama modunu etkinleştirin
3. **Disk Dolma Sorunu**: TTL indeksini düşük tutun veya log rotasyonu yapılandırın 