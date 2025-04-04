# LogLib - Gelişmiş Günlük Kütüphanesi

**Versiyon:** 1.0.0  
**Yazar:** AR-GE Mühendisi Aykut Mürkit  
**Şirket:** İsbak  

## Genel Bakış
LogLib, karmaşık JSON nesneleri için gelişmiş seri hale getirme desteği ile hem dosya hem de MongoDB tabanlı günlük yetenekleri sağlayan kapsamlı bir .NET uygulamaları için günlük kütüphanesidir.

## Özellikler

### Temel Özellikler
- Çoklu hedef günlük tutma (Konsol, Dosya, MongoDB)
- Meta veri desteği ile yapılandırılmış günlük tutma
- Esnek yapılandırma seçenekleri
- Asenkron günlük işlemleri
- Özel günlük seviyesi desteği
- Zengin bağlam verisi yakalama

### MongoDB Entegrasyonu
- Otomatik TTL indeks oluşturma
- Karmaşık nesneler için özel seri hale getirme
- Newtonsoft.Json.Linq tipleri için destek (JObject, JArray)
- Bağlantı havuzu ve hata yönetimi

### Performans
- Asenkron işlemler
- Toplu yazma
- Optimize edilmiş serileştirme

## Yapılandırma

### Temel Kurulum
```csharp
// Program.cs dosyasına ekleyin
builder.Services.AddLogLib(builder.Configuration);
```

### appsettings.json
```json
{
  "LogSettings": {
    // Dosya günlüğü ayarları
    "LogToFile": true,
    "LogFilePath": "Logs/app.log",
    "FileSizeLimitBytes": 10485760,
    "RetainedFileCount": 7,
    
    // MongoDB ayarları
    "LogToMongoDB": true,
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "LogsDb",
    "CollectionName": "Logs",
    "RetentionDays": 30
  }
}
```

## Kullanım Örnekleri

### Temel Günlük
```csharp
// Yapıcı enjeksiyonu
private readonly ILogService _logService;

public SomeController(ILogService logService)
{
    _logService = logService;
}

// Bilgi günlüğü
await _logService.LogInfoAsync(
    "Kullanıcı girişi başarılı", 
    "AuthController.Login",
    new { Username = "user123" });

// Hata günlüğü
try
{
    // Bir işlem
}
catch (Exception ex)
{
    await _logService.LogErrorAsync(
        "İşlem sırasında hata",
        "ServiceName.MethodName",
        ex);
}
```

### Gelişmiş Bağlam
```csharp
// Kullanıcı bağlamı ile günlük
await _logService.LogInfoAsync(
    "Profil güncellendi", 
    "ProfileController.Update",
    new { Fields = new[] { "email", "name" } },
    userId: "123",
    userName: "user123",
    userEmail: "user@example.com");
```

## Teknik Detaylar

### MongoDB Serileştirme
Kütüphane, karmaşık nesne tiplerini düzgün şekilde işlemek için MongoDB için özel serileştirme uygular:

- `JObject` ve `JArray` tipleri için özel serileştiriciler
- `_t`/`_v` yapısı içeren eski verileri okuma ve temiz veri yazma desteği
- Daha temiz veri yapısı sağlamak için devre dışı bırakılan tip ayrıştırıcıları
- Performans için verimli ikili serileştirme

### Uygulama Öne Çıkanları

```csharp
// Özel JObject serileştirici
private class JObjectSerializer : SerializerBase<JObject>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }
        
        // Tip ayrıştırıcıları olmadan doğrudan serileştirme
        var document = BsonDocument.Parse(value.ToString());
        BsonSerializer.Serialize(context.Writer, document);
    }
    
    // ... eski formatları destekleyen deserializasyon
}
```

## Sürüm Notları

### Versiyon 1.0.0
- İlk sürüm
- Özel serileştirme ile MongoDB entegrasyonu
- Newtonsoft.Json.Linq nesneleri için destek
- Dosya ve konsol günlük kaydı

## Lisans
Özel yazılım. Telif hakkı © 2025 İsbak. 