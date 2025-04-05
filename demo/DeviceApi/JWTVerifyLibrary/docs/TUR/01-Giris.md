# Giriş

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## JWT Nedir?

JSON Web Token (JWT), bilgileri güvenli bir şekilde iletmek için açık standart bir yöntemdir. Bilgiler, dijital olarak imzalanmış bir JSON nesnesi içinde taşınır. Bu, gönderilen bilgilerin doğruluğunu ve bütünlüğünü kontrol etmeyi sağlar.

JWT'ler genellikle kimlik doğrulama ve yetkilendirme için kullanılır. Bir kullanıcı sisteme giriş yaptığında, sunucu bir JWT oluşturur ve bu token, kullanıcının her istekte kimliğini doğrulamak için kullanılır.

## JWTVerifyLibrary Neden Oluşturuldu?

Web uygulamalarında güvenliğin önemi giderek artmaktadır. JWT doğrulama, modern web uygulamalarının önemli bir güvenlik bileşenidir, ancak doğru şekilde uygulamak zor olabilir. JWTVerifyLibrary, bu zorluğa çözüm olarak geliştirilmiştir.

Bu kütüphane:
- JWT doğrulama sürecini **basitleştirmek**
- Güvenlik risklerini **azaltmak**
- Geliştirme süresini **kısaltmak**
- Kod tekrarını **önlemek**
- **Standartlaştırılmış** bir yaklaşım sunmak

amacıyla oluşturulmuştur.

## Temel Özellikler

JWTVerifyLibrary, aşağıdaki temel özellikleri sunar:

1. **Token Doğrulama:** JWT'lerin doğruluğunu, bütünlüğünü ve geçerliliğini kontrol eder.
2. **Middleware Entegrasyonu:** ASP.NET Core pipeline'ına kolayca entegre olur.
3. **Kolay Yapılandırma:** appsettings.json dosyası üzerinden basit yapılandırma.
4. **Claims Çıkarma:** Token'dan kullanıcı bilgilerini çıkarma.
5. **Güvenlik Kontrolleri:** Süresi dolmuş token'ları, geçersiz imzaları veya yanlış alıcıları tespit etme.

## Kullanım Senaryoları

JWTVerifyLibrary, aşağıdaki kullanım senaryolarında idealdir:

- **API Güvenliği:** RESTful API'lerinizi JWT tabanlı kimlik doğrulama ile koruma.
- **Mikroservis Mimarisi:** Servisler arası güvenli iletişim sağlama.
- **SPA Uygulamaları:** Single Page Application'lar için backend güvenliği.
- **Mobil API'ler:** Mobil uygulamalara güvenli API erişimi sağlama.
- **B2B Entegrasyonları:** İş ortakları ile güvenli veri alışverişi.

## Kütüphane Felsefesi

JWTVerifyLibrary'nin tasarımında aşağıdaki ilkeler gözetilmiştir:

- **Basitlik:** Karmaşık yapılandırma gerektirmeden kullanılabilir olmalı.
- **Güvenlik:** Güvenlik en iyi uygulamaları ve standartları takip etmeli.
- **Esneklik:** Farklı projelerde ve senaryolarda kullanılabilir olmalı.
- **Performans:** Sistem performansını etkilemeyecek şekilde optimize edilmeli.
- **Bakım Kolaylığı:** Kod kalitesi ve belgelendirme standartlarını takip etmeli.

## Desteklenen Platformlar

JWTVerifyLibrary, aşağıdaki platformları destekler:

- .NET 8.0 ve üzeri
- ASP.NET Core 8.0 ve üzeri

---

[◀ Ana Sayfa](README.md) | [İleri: Kurulum ▶](02-Kurulum.md) 