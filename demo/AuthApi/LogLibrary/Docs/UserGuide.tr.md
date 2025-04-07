# LogLibrary Kullanım Kılavuzu

## Versiyon 1.0.0
**Yazar:** AR-GE Mühendisi Aykut Mürkit, İsbak

## İçindekiler
1. [Giriş](#giriş)
2. [Kurulum](#kurulum)
3. [Temel Yapılandırma](#temel-yapılandırma)
4. [Gelişmiş Yapılandırma](#gelişmiş-yapılandırma)
5. [Günlük Yöntemleri](#günlük-yöntemleri)
6. [HTTP İstek Günlüğü](#http-istek-günlüğü)
7. [MongoDB Entegrasyonu](#mongodb-entegrasyonu)
8. [Sorun Giderme](#sorun-giderme)

## Giriş

LogLibrary, çoklu hedef günlük yetenekleri, yapılandırılmış veri desteği ve MongoDB entegrasyonu sağlayan kapsamlı bir .NET uygulamaları için günlük kütüphanesidir. Kütüphane, herhangi bir .NET uygulamasına kolayca entegre edilecek şekilde tasarlanmıştır ve günlük işlemleri için basit bir API sunar.

## Kurulum

LogLibrary'i projenize eklemek için, projenize bir referans olarak ekleyebilirsiniz:

```csharp
<ProjectReference Include="..\LogLibrary\LogLibrary.csproj" />
```

## Temel Yapılandırma

LogLibrary, uygulamanızın `Program.cs` veya `Startup.cs` dosyasında yapılandırılabilir:

```csharp
// Program.cs
builder.Services.AddLogLibrary(builder.Configuration);
```

Temel ayarlar appsettings.json dosyasında yapılandırılabilir:

```json
{
  "LogSettings": {
    "ApplicationName": "UygulamaAdınız",
    "Environment": "Development",
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "VeritabanıAdınız",
    "CollectionName": "Logs",
    "RetentionDays": 30
  }
}
```

## Gelişmiş Yapılandırma

Gelişmiş yapılandırma için, kod içinde günlük ayarlarını özelleştirebilirsiniz:

```csharp
services.Configure<LogSettings>(options =>
{
    options.ApplicationName = "UygulamaAdınız";
    options.Environment = "Production";
    options.ConnectionString = "mongodb://kullanici:sifre@mongodb.example.com:27017";
    options.DatabaseName = "LogVeritabani";
    options.CollectionName = "UygulamaLoglari";
    options.RetentionDays = 90;
});
```

## Günlük Yöntemleri

LogLibrary farklı günlük seviyeleri için çeşitli yöntemler sunar:

```csharp
// Temel günlük
await _logService.LogInfoAsync("Kullanıcı giriş yaptı", "AuthController.Login", kullaniciVerisi);
await _logService.LogWarningAsync("Giriş denemesi başarısız oldu", "AuthController.Login", girisDenemesiVerisi);
await _logService.LogErrorAsync("İstisna oluştu", "PaymentService.Process", exception);
await _logService.LogDebugAsync("Hata ayıklama bilgisi", "DataProcessor", hataAyiklamaVerisi);
await _logService.LogCriticalAsync("Sistem hatası", "SystemMonitor", kritikHata);

// Kullanıcı bağlamı ile
await _logService.LogInfoAsync(
    "Profil güncellendi", 
    "ProfileController.Update", 
    profilVerisi, 
    userId: "123", 
    userName: "john.doe", 
    userEmail: "john@example.com"
);
```

## HTTP İstek Günlüğü

LogLibrary, HTTP istek günlükleri için yerleşik destek sunar:

```csharp
await _logService.LogHttpAsync(
    path: "/api/users", 
    method: "POST", 
    statusCode: 201, 
    durationMs: 150, 
    traceId: Activity.Current?.Id,
    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
    userName: User.FindFirstValue(ClaimTypes.Name),
    ipAddress: HttpContext.Connection.RemoteIpAddress.ToString(),
    requestData: istekGovdesi,
    responseData: yanitGovdesi
);
```

## MongoDB Entegrasyonu

LogLibrary, günlük depolama için gelişmiş özelliklerle MongoDB kullanır:

- Karmaşık nesneler için özel serileştirme
- Günlük saklama için otomatik TTL indeksleri
- Günlük analizi için verimli sorgu desteği

Kütüphane, depolanan verileri daha temiz ve daha verimli hale getirerek tip ayırıcıları olmadan JObject serileştirmesini işler.

## Sorun Giderme

### MongoDB Bağlantı Sorunları

MongoDB bağlantı sorunları yaşıyorsanız:

1. MongoDB sunucunuzun çalıştığından ve erişilebilir olduğundan emin olun
2. Bağlantı dizesi formatını kontrol edin
3. Uygun izinlere sahip olduğunuzdan emin olun

### Eksik Günlükler

Günlükler MongoDB'de görünmüyorsa:

1. Uygulama günlüklerinde MongoDB bağlantı durumunu kontrol edin
2. `ILogService`'in düzgün şekilde enjekte edildiğini doğrulayın
3. Yapılandırmada MongoDB veritabanı/koleksiyon adlarını kontrol edin

### Performans Sorunları

Performans optimizasyonu için:

1. Gereksiz günlük kaydını azaltmak için uygun günlük seviyelerini kullanın
2. Yüksek trafikli uygulamalar için MongoDB bağlantı havuzu boyutunu artırmayı düşünün
3. MongoDB'de sık sorgulanan alanlar için uygun indeksler ekleyin

---

© İsbak, 2023. Tüm hakları saklıdır. 