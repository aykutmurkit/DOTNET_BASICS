# Deneme API Dokümantasyonu

Bu klasör, Deneme API projesi ile ilgili teknik dokümantasyonu içerir.

## İçindekiler

- [Mimari Dokümantasyonu](#mimari-dokümantasyonu)
- [Loglama Sistemi](#loglama-sistemi)
- [API Kullanımı](#api-kullanımı)
- [Veritabanı Şeması](#veritabanı-şeması)
- [Klasör Yapısı Rehberi](#klasör-yapısı-rehberi)

## Mimari Dokümantasyonu

Deneme API, modern .NET 8 teknolojilerini kullanan bir N-Tier (Çok Katmanlı) mimari üzerine inşa edilmiştir. Bu yapı, uygulamayı mantıksal olarak birbirinden ayrılmış katmanlara bölerek, daha modüler, test edilebilir ve bakımı kolay hale getirir.

Detaylı bilgi için [N-Tier Mimari Dokümantasyonu](N-Tier-Architecture.md) sayfasına bakabilirsiniz.

## Loglama Sistemi

Deneme API, MongoDB tabanlı kapsamlı bir loglama sistemi kullanır. Bu sistem, gelen istekleri, yanıtları ve sistem olaylarını kaydeder ve analiz amacıyla saklar. Loglama sistemi yalnızca dahili olarak çalışır ve harici erişim endpoint'leri bulunmaz.

Detaylı bilgi için [Loglama Sistemi Dokümantasyonu](logging-system.md) sayfasına bakabilirsiniz.

## API Kullanımı

Deneme API, REST standartlarını takip eden bir API sunar. Tüm istekler JSON formatında veri alır ve döndürür.

API dokümantasyonu, OpenAPI (Swagger) kullanılarak otomatik olarak oluşturulur ve şu adresten erişilebilir: `https://{base-url}/swagger`

## Veritabanı Şeması

Deneme API, birincil veritabanı olarak SQL Server, loglama için ise MongoDB kullanır. Veritabanı şeması, Entity Framework Core Code-First yaklaşımı ile oluşturulmuştur.

Veritabanı yapılandırması ve migration işlemleri hakkında detaylı bilgi için [Veritabanı Şeması Dokümantasyonu](database-configuration.md) sayfasına bakabilirsiniz.

## Klasör Yapısı Rehberi

Deneme API projesi, yeniden düzenlenmiş klasör yapısıyla N-Tier mimariye uygun olarak aşağıdaki klasör yapısını kullanır:

```
DeviceApi/
│
├── API/                      # Presentation Layer - API katmanı
│   ├── Controllers/          # API endpoint'lerini içeren controller'lar
│   ├── Middleware/           # HTTP Pipeline'ında kullanılan middleware'ler
│   ├── Models/               # API'ye özgü model sınıfları
│   └── Extensions/           # API katmanına özgü extension metotları
│
├── Business/                 # Business Layer - İş mantığı katmanı
│   ├── Services/             # İş mantığını içeren servisler
│   │   ├── Concrete/         # Servis implementasyonları
│   │   └── Interfaces/       # Servis arayüzleri
│   └── Extensions/           # Business katmanına özgü extension metotları
│
├── DataAccess/               # Data Access Layer - Veri erişim katmanı
│   ├── Context/              # EF Core DbContext sınıfları
│   ├── Repositories/         # Repository pattern implementasyonları
│   │   ├── Concrete/         # Somut repository sınıfları
│   │   └── Interfaces/       # Repository arayüzleri
│   ├── Configurations/       # Entity Configurations
│   ├── Seeding/              # Seed sınıfları ve veritabanı ilk veriler
│   └── Extensions/           # DataAccess katmanına özgü extension metotları
│
├── Entities/                 # Entity Layer - Varlık katmanı
│   ├── Concrete/             # Veritabanı entity'leri
│   └── DTOs/                 # Veri Transfer Objeleri
│
├── Core/                     # Core Layer - Çekirdek katmanı
│   ├── Security/             # Güvenlik ile ilgili sınıflar
│   ├── Utilities/            # Yardımcı sınıflar ve metotlar
│   ├── Extensions/           # Genel extension metotları
│   └── Logging/              # Loglama altyapısı (Controller'sız)
│
├── Documentation/            # Proje dokümantasyonu
│
├── Program.cs                # Uygulama giriş noktası ve konfigürasyon
└── appsettings.json          # Uygulama ayarları
```

### Namespace Yapısı

Projenin namespace yapısı, klasör yapısına paralel olarak düzenlenmiştir:

```
DeviceApi.API.*                  # API katmanı namespace'leri
DeviceApi.Business.*             # Business katmanı namespace'leri
DeviceApi.DataAccess.*           # DataAccess katmanı namespace'leri
DeviceApi.Entities.*             # Entity katmanı namespace'leri
DeviceApi.Core.*                 # Core katmanı namespace'leri
```

## Katkıda Bulunma

Projeye katkıda bulunmak için aşağıdaki adımları izleyebilirsiniz:

1. Bu repo'yu fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylı bilgi için `LICENSE` dosyasına bakabilirsiniz. 