# Yapılandırma

**Sürüm:** 1.0.0  
**Şirket:** DevOps 2025

---

Bu belge, RateLimitLibrary için mevcut tüm yapılandırma seçeneklerini, anlamlarını ve nasıl ayarlanacaklarını ayrıntılı olarak açıklar.

## RateLimitLibrarySettings.json Yapısı

RateLimitLibrary, yapılandırma ayarlarını `RateLimitLibrarySettings.json` dosyasından okur. Bu yapılandırma dosyasının genel yapısı aşağıdaki gibidir:

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
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 5
      },
      {
        "Endpoint": "/api/device",
        "Period": "10m",
        "Limit": 3
      }
    ]
  }
}
```

## Temel Ayarlar

### Global Rate Limiting Ayarları

| Ayar | Tip | Varsayılan | Açıklama |
|------|-----|------------|----------|
| `EnableGlobalRateLimit` | boolean | `true` | Global rate limiting'in etkin olup olmadığını belirler. `true` olarak ayarlanırsa, tüm API istekleri global olarak sınırlandırılır. |
| `GlobalRateLimitPeriod` | string | `"1m"` | Global rate limiting için zaman penceresi süresini belirler. Değerler: "Xs" (saniye), "Xm" (dakika), "Xh" (saat), "Xd" (gün) olabilir. X yerine sayısal değer gelmelidir, örneğin "30s", "15m". |
| `GlobalRateLimitRequests` | integer | `100` | Belirtilen zaman penceresi içinde izin verilen maksimum istek sayısını belirler. Örneğin, `GlobalRateLimitPeriod` "1m" ve `GlobalRateLimitRequests` 100 ise, her dakika en fazla 100 istek işlenir. |

### IP Bazlı Rate Limiting Ayarları

IP tabanlı rate limiting, istekleri IP adreslerine göre sınırlandırır. Bu, her bir kullanıcı/cihazın belirli bir oran dahilinde istekte bulunmasını sağlar.

| Ayar | Tip | Varsayılan | Açıklama |
|------|-----|------------|----------|
| `IpRateLimiting.EnableIpRateLimiting` | boolean | `true` | IP tabanlı rate limiting'in etkin olup olmadığını belirler. |
| `IpRateLimiting.IpRateLimitPeriod` | string | `"1m"` | IP tabanlı rate limiting için zaman penceresi süresini belirler. Formatı global ayarlarla aynıdır (örn. "5m"). |
| `IpRateLimiting.IpRateLimitRequests` | integer | `100` | Her IP adresi için zaman penceresi başına izin verilen maksimum istek sayısı. |

## Endpoint Bazlı Limitler

Endpoint bazlı rate limiting, belirli API endpoint'lerine göre özel sınırlamalar tanımlamanıza olanak tanır. Bu, bazı API yollarını diğerlerinden daha sıkı şekilde kısıtlamanızı sağlar.

Endpoint limitleri bir dizi olarak yapılandırılır, her bir endpoint için ayrı bir yapılandırma tanımlanır:

| Ayar | Tip | Varsayılan | Açıklama |
|------|-----|------------|----------|
| `Endpoint` | string | gerekli | Limitlendirmek istediğiniz API endpoint'inin yolu. Örneğin, "/api/auth/login". |
| `Period` | string | gerekli | Bu endpoint için zaman penceresi süresi. Formatı global ayarlarla aynıdır (örn. "5m"). |
| `Limit` | integer | gerekli | Belirtilen zaman penceresi içinde bu endpoint için izin verilen maksimum istek sayısı. |
| `EnableConcurrencyLimit` | boolean | `false` | Eşzamanlılık sınırlamasının etkinleştirilip etkinleştirilmeyeceği. Bu, endpoint'e yapılan eşzamanlı istek sayısını sınırlar. |
| `ConcurrencyLimit` | integer | `0` | Endpoint'e izin verilen maksimum eşzamanlı istek sayısı. Sadece `EnableConcurrencyLimit` true olduğunda kullanılır. |

## Zaman Birimi Formatları

Zaman penceresi süreleri, aşağıdaki formatları kullanabilir:

| Format | Açıklama | Örnek |
|--------|----------|-------|
| `Xs` | X saniye | "30s" (30 saniye) |
| `Xm` | X dakika | "5m" (5 dakika) |
| `Xh` | X saat | "2h" (2 saat) |
| `Xd` | X gün | "1d" (1 gün) |

## Yapılandırma Örnekleri

### Örnek 1: Basit Global Sınırlama

Sadece global rate limiting'i etkinleştiren basit bir yapılandırma:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": false
    },
    "EndpointLimits": []
  }
}
```

### Örnek 2: Yalnızca IP Tabanlı Sınırlama

Global sınırlama olmadan sadece IP tabanlı sınırlama:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": false,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 50
    },
    "EndpointLimits": []
  }
}
```

### Örnek 3: Endpoint Odaklı Sınırlama

Belirli endpoint'ler için özel sınırlar:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": false,
    "IpRateLimiting": {
      "EnableIpRateLimiting": false
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "15m",
        "Limit": 5,
        "EnableConcurrencyLimit": false
      },
      {
        "Endpoint": "/api/payment/process",
        "Period": "1h",
        "Limit": 10,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 2
      }
    ]
  }
}
```

### Örnek 4: Kapsamlı Sınırlama

Global, IP ve endpoint'lerin hepsini birlikte kullanan kapsamlı bir yapılandırma:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 1000,
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
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 5
      },
      {
        "Endpoint": "/api/users",
        "Period": "10m",
        "Limit": 50
      },
      {
        "Endpoint": "/api/reports/generate",
        "Period": "1h",
        "Limit": 5,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 2
      }
    ]
  }
}
```

## Yapılandırma Önceliği

RateLimitLibrary şu öncelik sıralamasını kullanır:

1. Endpoint-spesifik limitler (en yüksek öncelik)
2. IP bazlı limitler
3. Global limitler (en düşük öncelik)

Tüm konfigürasyon türlerini aynı anda etkinleştirirseniz, her istek tüm uygulanabilir limitleri kontrol eder ve herhangi birinden reddedilirse istek engellenir.

## Kod İçinde Yapılandırma Değiştirme

Kod içinde dinamik olarak ayarları değiştirmek istiyorsanız, RateLimitLibrary extension metotlarını kullanabilirsiniz:

```csharp
// RateLimiter options'ı doğrudan yapılandırma
builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 200, // Özel değer
            Window = TimeSpan.FromMinutes(2), // Özel değer
            QueueLimit = 0
        });
    });
    
    // Diğer özel yapılandırmalar...
});
```

---

[◀ Hızlı Başlangıç](03-Hizli-Baslangic.md) | [Ana Sayfa](../README.md) | [İleri: API Referansı ▶](05-API-Referansi.md) 