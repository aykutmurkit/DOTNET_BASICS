# Kurulum

**Sürüm:** 1.0.0  
**Şirket:** DevOps 2025

---

Bu belge, RateLimitLibrary'nin ASP.NET Core uygulamanıza nasıl kurulacağını ve temel yapılandırmasını açıklar.

## Sistem Gereksinimleri

RateLimitLibrary'yi kullanabilmek için aşağıdaki gereksinimlere ihtiyacınız vardır:

- .NET 8.0 veya daha yeni bir sürüm
- ASP.NET Core 8.0+ web uygulaması
- Visual Studio 2022, Visual Studio Code veya JetBrains Rider

## Paket Yükleme Seçenekleri

### 1. NuGet Paketi Olarak Yükleme

RateLimitLibrary'yi NuGet üzerinden yüklemek için aşağıdaki yöntemlerden birini kullanabilirsiniz:

#### Visual Studio'da NuGet Paket Yöneticisi ile:

1. Solution Explorer'da projenize sağ tıklayın
2. "Manage NuGet Packages..." seçeneğini tıklayın
3. "Browse" sekmesine geçin
4. Arama kutusuna "RateLimitLibrary" yazın
5. İlgili paketi bulun ve "Install" butonuna tıklayın

#### NuGet Paket Yöneticisi Konsolu ile:

```powershell
Install-Package RateLimitLibrary
```

#### dotnet CLI ile:

```bash
dotnet add package RateLimitLibrary
```

### 2. Proje Referansı Olarak Ekleme

Eğer RateLimitLibrary'nin kaynak kodunu doğrudan projenize dahil etmek isterseniz:

1. RateLimitLibrary projesini çözümünüze ekleyin
2. Hedef projenizde, RateLimitLibrary projesine referans ekleyin:

```xml
<ItemGroup>
  <ProjectReference Include="..\path\to\RateLimitLibrary\RateLimitLibrary.csproj" />
</ItemGroup>
```

## Yapılandırma Dosyası Ekleme

RateLimitLibrary, ayarlarını `RateLimitLibrarySettings.json` dosyasından okur. Bu dosyayı projenize eklemeniz gerekir:

1. Projenizin kök dizininde `RateLimitLibrarySettings.json` adında bir dosya oluşturun
2. Aşağıdaki temel yapılandırmayı ekleyin:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 100
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "5m",
        "Limit": 10,
        "EnableConcurrencyLimit": false,
        "ConcurrencyLimit": 0
      }
    ]
  }
}
```

3. Proje dosyanızda (.csproj), bu dosyanın çıktı dizinine kopyalandığından emin olun:

```xml
<ItemGroup>
  <None Update="RateLimitLibrarySettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Alternatif: appsettings.json İçinde Yapılandırma

Isterseniz, RateLimitLibrary ayarlarını mevcut `appsettings.json` dosyanızda da tanımlayabilirsiniz:

```json
{
  "ConnectionStrings": {
    // ...mevcut ayarlar
  },
  "Logging": {
    // ...mevcut ayarlar
  },
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 100
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "5m",
        "Limit": 10,
        "EnableConcurrencyLimit": false,
        "ConcurrencyLimit": 0
      }
    ]
  }
}
```

## Servis Kayıtları

Kütüphaneyi ASP.NET Core uygulamanıza entegre etmek için `Program.cs` dosyanızı düzenlemeniz gerekir:

```csharp
using RateLimitLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Diğer servis kayıtları
// ...

// Rate limiting servislerini ekleyin
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

// Middleware pipeline'ı yapılandırma
// ...

// Rate limiting middleware'ini ekleyin (UseAuthentication'dan önce olmalıdır)
app.UseRateLimiting();

// Diğer middleware'ler
app.UseAuthentication();
app.UseAuthorization();
// ...

// Endpoint'lere rate limit politikası uygulama
app.MapControllers().RequireRateLimiting("ip");

app.Run();
```

## Doğrulama

Kurulumun başarılı olduğunu doğrulamak için:

1. Uygulamanızı başlatın
2. Hızlı bir şekilde art arda API çağrıları yapın (yapılandırdığınız limitlerden daha fazla)
3. HTTP 429 (Too Many Requests) yanıtı alıp almadığınızı kontrol edin

## Sorun Giderme

**Sorun:** Rate limitleri çalışmıyor, sınırlama gerçekleşmiyor.

**Çözüm:**
- `Program.cs` dosyasında `UseRateLimiting()` çağrısının doğru sırada olduğundan emin olun
- `RateLimitLibrarySettings.json` dosyasının çıktı dizinine kopyalandığından emin olun
- Yapılandırma değerlerinin beklendiği gibi olduğunu kontrol edin

**Sorun:** "Could not find file 'RateLimitLibrarySettings.json'" hatası alıyorum.

**Çözüm:**
- Dosyanın proje kök dizininde olduğundan emin olun
- Proje dosyasında `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` ayarının yapıldığından emin olun

---

[◀ Giriş](01-Giris.md) | [Ana Sayfa](../README.md) | [İleri: Hızlı Başlangıç ▶](03-Hizli-Baslangic.md) 