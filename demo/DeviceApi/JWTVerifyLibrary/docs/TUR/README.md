# JWTVerifyLibrary

## Genel Bakış

JWTVerifyLibrary, ASP.NET Core uygulamalarında JWT (JSON Web Token) doğrulamasını kolaylaştırmak için geliştirilmiş güçlü, hafif ve entegrasyonu kolay bir kütüphanedir. Bu kütüphane, güvenlik ve performans odaklı tasarlanmış olup, çeşitli projelere kolayca entegre edilebilir.

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## İçindekiler

- [Giriş](01-Giris.md)
- [Kurulum](02-Kurulum.md)
- [Hızlı Başlangıç](03-Hizli-Baslangic.md)
- [Temel Kullanım](04-Temel-Kullanim.md)
- [Gelişmiş Özellikler](05-Gelismis-Ozellikler.md)
- [Mimari Yapı](06-Mimari-Yapi.md)
- [API Referansı](07-API-Referansi.md)
- [Versiyonlama ve Yol Haritası](08-Versiyonlama.md)
- [En İyi Uygulama Örnekleri](09-Best-Practices.md)
- [Sorun Giderme](10-Sorun-Giderme.md)
- [Katkıda Bulunma](11-Katkida-Bulunma.md)
- [Lisans](12-Lisans.md)

---

## Özellikler

- ✅ JWT token doğrulama ve güvenlik kontrolü
- ✅ ASP.NET Core middleware entegrasyonu
- ✅ .NET 8.0 üzerinde çalışır
- ✅ Harici yapılandırma desteği
- ✅ Kolay entegrasyon için extension metodları
- ✅ Token içindeki iddiaları (claims) çıkarma
- ✅ Kullanıcı kimliği atama
- ✅ Varsayılan güvenlik ayarları
- ✅ Tam test kapsamı
- ✅ Kapsamlı dokümantasyon

---

## Hızlı Başlangıç

```csharp
// Program.cs dosyasına JWT doğrulamasını ekleyin
using JWTVerifyLibrary.Extensions;

// Servis kaydı
builder.Services.AddJwtVerification(builder.Configuration);

// Middleware kaydı
app.UseJwtVerification();

// Artık [Authorize] özniteliğini kullanabilirsiniz!
```

Daha fazla bilgi için [Hızlı Başlangıç](03-Hizli-Baslangic.md) kılavuzuna göz atın.

---

## Destek

Herhangi bir sorunuz veya geri bildiriminiz varsa, lütfen GitHub üzerinden issue açın veya pull request gönderin.

---

## Lisans

Bu kütüphane MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](../../LICENSE) dosyasına bakınız.

---

© 2025 İSBAK. Tüm hakları saklıdır. 