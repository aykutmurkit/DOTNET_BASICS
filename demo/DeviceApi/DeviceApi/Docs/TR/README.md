# DeviceApi

## Genel Bakış

DeviceApi, IoT cihazlarını ve verilerini yönetmek için tasarlanmış kapsamlı bir REST API'dir. ASP.NET Core ile oluşturulan bu API, cihaz kaydı, yönetimi ve veri toplama için güvenli, ölçeklenebilir ve esnek bir çözüm sunar.

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## İçindekiler

- [Giriş](01-Giris.md)
- [Kurulum](02-Kurulum.md)
- [Hızlı Başlangıç](03-Hizli-Baslangic.md)
- [API Uç Noktaları](04-API-Uc-Noktalari.md)
- [Kimlik Doğrulama](05-Kimlik-Dogrulama.md)
- [Veri Modelleri](06-Veri-Modelleri.md)
- [Mimari Yapı](06-Mimari-Yapi.md)
- [Veritabanı Seeding Süreci](Seeding-Sureci.md)
- [Yapılandırma](07-Yapilandirma.md)
- [Hata Yönetimi](08-Hata-Yonetimi.md)
- [Performans Optimizasyonu](09-Performans-Optimizasyonu.md)
- [Güvenlik](10-Guvenlik.md)
- [Dağıtım](11-Dagitim.md)
- [İzleme](12-Izleme.md)
- [Sorun Giderme](13-Sorun-Giderme.md)
- [Katkıda Bulunma](14-Katkida-Bulunma.md)
- [Lisans](15-Lisans.md)

---

## Özellikler

- ✅ Uygun HTTP metod semantiği ile RESTful API tasarımı
- ✅ Güvenli API erişimi için JWT kimlik doğrulama
- ✅ Kapsamlı cihaz yönetimi (kayıt, güncelleme, durum)
- ✅ Gerçek zamanlı cihaz veri toplama ve işleme
- ✅ Farklı cihaz gruplarını yönetmek için çok kiracılı mimari
- ✅ Yönetimsel işlemler için rol tabanlı erişim kontrolü
- ✅ Tüm koleksiyon uç noktaları için sayfalama, filtreleme ve sıralama
- ✅ Swagger/OpenAPI dokümantasyonu
- ✅ Loglama ve izleme yetenekleri
- ✅ Docker ile konteynerizasyon desteği
- ✅ Yüksek hacimli veri işleme için performans optimizasyonu

---

## Hızlı Başlangıç

DeviceApi ile başlamak için:

```csharp
// Depoyu klonlayın
git clone https://github.com/isbak/DeviceApi.git

// Proje dizinine gidin
cd DeviceApi

// Uygulamayı derleyin ve çalıştırın
dotnet build
dotnet run
```

API uç noktalarını Swagger UI kullanarak keşfetmek için `https://localhost:5001/swagger` adresine gidin.

Daha fazla bilgi için [Hızlı Başlangıç](03-Hizli-Baslangic.md) kılavuzuna göz atın.

---

## Destek

Herhangi bir sorunuz veya geri bildiriminiz varsa, lütfen GitHub üzerinden bir issue açın veya pull request gönderin.

---

## Lisans

Bu API, MIT Lisansı altında lisanslanmıştır. Detaylar için [LICENSE](../../LICENSE) dosyasına bakınız.

---

© 2025 İSBAK. Tüm hakları saklıdır. 