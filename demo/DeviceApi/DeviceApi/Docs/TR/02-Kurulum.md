# Kurulum

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölüm, DeviceApi'yi hem geliştirme hem de üretim ortamları için kurma sürecinde size rehberlik eder.

## Önkoşullar

DeviceApi'yi kurmadan önce, sisteminizin aşağıdaki gereksinimleri karşıladığından emin olun:

### Geliştirme Ortamı

- **.NET SDK 8.0** veya daha yeni bir sürüm
- **Visual Studio 2022** (herhangi bir sürüm) veya C# uzantıları ile **Visual Studio Code**
- **SQL Server 2019** veya daha yeni (Express sürümü geliştirme için yeterlidir)
- Kaynak kontrolü için **Git**
- API testi için **Postman** veya benzeri bir araç (isteğe bağlı ancak önerilir)

### Üretim Ortamı

- Docker desteği ile **Windows Server 2019/2022** veya **Linux**
- **SQL Server 2019** veya daha yeni
- **Redis** (dağıtılmış önbellek için)
- Güvenilir bir sertifika yetkilisinden **HTTPS sertifikası**
- **Docker ve Docker Compose** (konteynerler ile dağıtıyorsanız)

## Kurulum Seçenekleri

DeviceApi, gereksinimlerinize bağlı olarak birkaç şekilde kurulabilir:

### Seçenek 1: GitHub'dan Klonlama (Geliştirme)

Bu, geliştirme ortamları için önerilen yaklaşımdır:

```bash
# Depoyu klonla
git clone https://github.com/isbak/DeviceApi.git

# Proje dizinine git
cd DeviceApi

# Bağımlılıkları geri yükle
dotnet restore

# Çözümü derle
dotnet build
```

### Seçenek 2: Docker ile Dağıtım (Geliştirme veya Üretim)

Docker kullanarak hızlı bir kurulum için:

```bash
# Depoyu klonla
git clone https://github.com/isbak/DeviceApi.git

# Proje dizinine git
cd DeviceApi

# Docker konteynerlerini oluştur ve başlat
docker-compose up -d
```

Bu, SQL Server ve Redis örnekleriyle birlikte DeviceApi'yi başlatacaktır.

### Seçenek 3: IIS'e Manuel Dağıtım (Üretim)

Geleneksel Windows barındırma için:

1. Uygulamayı derleyin:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Yeni bir IIS sitesi oluşturun:
   - IIS Yöneticisi'ni açın
   - Yeni bir uygulama havuzu oluşturun (.NET CLR Sürümü: "Yönetilen Kod Yok")
   - Yayınlanan klasörü gösteren yeni bir web sitesi oluşturun
   - HTTPS sertifikanızla bağlama yapılandırın

3. Uygulama havuzu kimliği için gerekli izinleri yapılandırın

## Veritabanı Kurulumu

DeviceApi, veritabanı işlemleri için Entity Framework Core kullanır. Veritabanını kurmak için iki seçeneğiniz vardır:

### Seçenek 1: Migrations Kullanımı (Önerilen)

```bash
# Proje dizinine git
cd DeviceApi

# Veritabanını oluşturmak veya güncellemek için migrations'ları uygula
dotnet ef database update
```

### Seçenek 2: Manuel Script Çalıştırma

Veritabanı oluşturmayı manuel olarak kontrol etmeyi tercih ediyorsanız:

1. SQL komut dosyasını oluşturun:
   ```bash
   dotnet ef migrations script -o create_database.sql
   ```

2. Komut dosyasını SQL Server Management Studio veya diğer SQL araçlarını kullanarak SQL Server örneğinizde çalıştırın

## Yapılandırma

DeviceApi, çeşitli kaynaklarda saklanan ayarlarla ASP.NET Core yapılandırma sistemini kullanır:

### appsettings.json

Ana yapılandırma dosyası `appsettings.json`'dır. İşte en önemli ayarlarla bir örnek:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DeviceApi;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "Secret": "en-az-32-karakterli-super-gizli-anahtariniz",
    "Issuer": "deviceapi",
    "Audience": "deviceapi-client",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Ortam Değişkenleri

Üretim ortamları için, hassas ayarları geçersiz kılmak üzere ortam değişkenlerini kullanmanız önerilir:

```bash
# Windows
setx DEVICEAPI_ConnectionStrings__DefaultConnection "Server=prod-db;Database=DeviceApi;User Id=app_user;Password=secure_password;"
setx DEVICEAPI_JwtSettings__Secret "cok-guvenli-olan-uretim-gizli-anahtari"

# Linux/macOS
export DEVICEAPI_ConnectionStrings__DefaultConnection="Server=prod-db;Database=DeviceApi;User Id=app_user;Password=secure_password;"
export DEVICEAPI_JwtSettings__Secret="cok-guvenli-olan-uretim-gizli-anahtari"
```

### Kullanıcı Sırları (Geliştirme)

Geliştirme sırasında, hassas bilgileri saklamak için .NET User Secrets'ı kullanın:

```bash
# Proje dizinine git
cd DeviceApi

# Kullanıcı sırlarını başlat
dotnet user-secrets init

# Sırları ayarla
dotnet user-secrets set "JwtSettings:Secret" "yerel-test-icin-gelistirme-gizli-anahtari"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=DeviceApi;Trusted_Connection=True;"
```

## Doğrulama

Kurulumun başarılı olduğunu doğrulamak için:

1. Uygulamayı başlatın:
   ```bash
   dotnet run
   ```

2. Bir web tarayıcısı açın ve şu adreslere gidin:
   - `https://localhost:5001/swagger` (Swagger UI için)
   - `https://localhost:5001/health` (sağlık kontrolü uç noktası için)

3. Swagger UI'ı görüyorsanız ve sağlık kontrolü "Healthy" döndürüyorsa, kurulum başarılıdır

## Sorun Giderme

Yaygın kurulum sorunları ve çözümleri:

### Veritabanı Bağlantı Sorunları

- **Sorun**: "Veritabanına bağlanılamıyor" hatası
- **Çözüm**: SQL Server'ın çalıştığını ve bağlantı dizesinin doğru olduğunu doğrulayın

### Sertifika Sorunları

- **Sorun**: HTTPS sertifika hataları
- **Çözüm**: Geliştirme için şunları kullanabilirsiniz:
  ```bash
  dotnet dev-certs https --clean
  dotnet dev-certs https --trust
  ```

### Port Çakışmaları

- **Sorun**: "Adres zaten kullanımda" hatası
- **Çözüm**: `Properties/launchSettings.json` dosyasında portu değiştirin veya çakışan portu kullanan işlemi durdurun

### Docker Sorunları

- **Sorun**: "Docker daemon'a bağlanılamıyor" hatası
- **Çözüm**: Docker servisinin `docker info` ile çalıştığından emin olun

## Sonraki Adımlar

Artık DeviceApi'yi kurduğunuza göre:

1. İlk API çağrılarınızı yapmak için [Hızlı Başlangıç](03-Hizli-Baslangic.md) kılavuzunu takip edin
2. [API Uç Noktaları](04-API-Uc-Noktalari.md) belgesini keşfedin
3. API'nizi güvence altına almak için [Kimlik Doğrulama](05-Kimlik-Dogrulama.md) hakkında bilgi edinin

---

[◀ Giriş](01-Giris.md) | [Ana Sayfa](README.md) | [İleri: Hızlı Başlangıç ▶](03-Hizli-Baslangic.md) 