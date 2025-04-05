# Giriş

**Sürüm:** 1.0.0  
**Şirket:** DevOps 2025

---

## RateLimitLibrary Nedir?

RateLimitLibrary, ASP.NET Core uygulamaları için basit ve etkili bir hız sınırlama (rate limiting) kütüphanesidir. Bu kütüphane, web API'lerinizi yüksek trafikten ve kötü niyetli istek bombardımanlarından korumak için tasarlanmıştır.

Hız sınırlama, bir API'ye belirli bir zaman diliminde gelebilecek istek sayısını kontrol ederek, hizmetinizin kaynaklarını korur ve tüm kullanıcılara adil bir hizmet sunulmasını sağlar. RateLimitLibrary, bu işlevselliği ASP.NET Core uygulamalarına kolayca entegre etmenizi sağlar.

## Neden RateLimitLibrary?

### Çözülen Sorunlar

RateLimitLibrary, aşağıdaki yaygın API sorunlarını çözer:

- **Aşırı Yükleme Koruması**: Yoğun trafik anlarında sunucunuzun çökmesini önler
- **DDoS Saldırılarına Karşı Koruma**: Kötü niyetli istek bombardımanlarına karşı koruma sağlar
- **Kaynak Kullanımını Optimizasyon**: Sistemin kaynakları dengeli kullanmasını sağlar
- **Adil Kullanım**: Tüm API kullanıcılarının kaynakları adil şekilde kullanmasını sağlar
- **Maliyet Kontrolü**: Bulut tabanlı sistemlerde işlem maliyetlerini kontrol altında tutar

### Temel Özellikleri

- **Global Hız Sınırlama**: Tüm API isteklerini sınırlandırma
- **IP Tabanlı Hız Sınırlama**: Belirli IP adreslerinden gelen istekleri sınırlandırma
- **Endpoint Bazlı Hız Sınırlama**: Belirli API endpoint'leri için özel sınırlar belirleme
- **Zamana Dayalı Sınırlandırma**: Saniye, dakika, saat veya gün bazında sınırlandırma ayarlama
- **Eşzamanlılık Limitleri**: Aynı anda işlenen istek sayısını kontrol etme
- **Kolay Yapılandırma**: JSON tabanlı basit yapılandırma

## Temel Kavramlar

### Rate Limiting Stratejileri

RateLimitLibrary, aşağıdaki rate limiting stratejilerini destekler:

#### 1. Fixed Window (Sabit Pencere)

Belirli bir zaman diliminde (örneğin her dakika) yapılabilecek maksimum istek sayısını belirler. Zaman dilimi tamamlandığında sayaç sıfırlanır.

```
│        Dakika 1         │         Dakika 2        │
├─────────────────────────┼─────────────────────────┤
│ Maks 100 istek          │ Maks 100 istek          │
```

#### 2. Concurrent Limiting (Eşzamanlılık Sınırlaması)

Aynı anda işlenebilecek maksimum istek sayısını belirler. Özellikle kaynak yoğun işlemler için faydalıdır.

```
Aynı anda en fazla 5 istek işlenebilir:
[İstek 1] [İstek 2] [İstek 3] [İstek 4] [İstek 5]
```

### Limit Aşımı Davranışı

Bir istek tanımlanan limitleri aştığında, API 429 Too Many Requests (Çok Fazla İstek) HTTP durum kodu döndürür. Bu durumda istemcinin bir süre bekleyip tekrar denemesi gerekir.

## Kullanım Senaryoları

RateLimitLibrary aşağıdaki senaryolarda özellikle faydalıdır:

- **Halka Açık API'ler**: Ücretsiz veya ücretli olarak sunulan ve çok sayıda kullanıcı tarafından erişilen API'ler
- **Mikro Servis Mimarileri**: Servisler arasındaki iletişimi düzenlemek ve korumak için
- **Ödeme ve Finansal İşlemler**: Hassas işlemlerde güvenliği artırmak için
- **Yüksek Trafikli Web Siteleri**: DDoS saldırılarına karşı koruma için
- **Çoklu Kiracılı Sistemler**: Farklı kiracılar arasında kaynakların adil dağılımını sağlamak için

---

[◀ Ana Sayfa](../README.md) | [İleri: Kurulum ▶](02-Kurulum.md) 