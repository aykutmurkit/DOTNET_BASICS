# LogLibrary Sürüm Notları

## Versiyon 1.0.0 (Ekim 2023)

**Yazar:** AR-GE Mühendisi Aykut Mürkit, İsbak

### İlk Sürüm Özellikleri

- **Çoklu Hedef Günlük Kaydı**: Eşzamanlı olarak konsol, dosya ve MongoDB'ye günlük kaydı yapma yeteneği.
- **JObject ve JArray için Özel Serileştirme**: Veritabanında daha temiz bir depolama formatı için Newtonsoft JSON nesnelerinin tip bilgisi olmadan serileştirilmesi.
- **Yapılandırılmış Günlük Kaydı**: Karmaşık günlük verilerini destekler, ayrıştırma ve analiz için JSON formatında depolama.
- **Esnek Yapılandırma**: Programatik olarak veya appsettings.json aracılığıyla yapılandırma desteği.
- **Asenkron İşlemler**: Uygulama performansını etkilemeden günlük kaydı için tam asenkron API.
- **HTTP İstek Günlüğü**: ASP.NET Core uygulamalarında istek günlüğü kaydı için yerleşik destek.
- **MongoDB Performans Optimizasyonları**: Üretim ortamları için optimize edilmiş serileştirme ve depolama stratejileri.

### Eklentiler

- MongoDB günlükleri için TTL (Time-To-Live) indeksleri desteği eklendi.
- İstek izleme ve korelasyon kimlikleri için destek eklendi.
- Kullanıcı bağlamı günlüğü için yardımcı yöntemler eklendi.

### İyileştirmeler

- Tüm günlük kaydı işlemleri için daha iyi hata yönetimi ve geri düşme stratejileri.
- Standart .NET Core günlük sağlayıcılarıyla daha iyi entegrasyon.
- Kapsamlı API belgeleri ve Türkçe kullanım kılavuzu eklendi.

### Hata Düzeltmeleri

- MongoDB bağlantı hatalarının daha iyi ele alınması.
- Uzun günlük metinlerinde serileştirme sorunları düzeltildi.
- JArray serileştirmesindeki metin kodlama sorunları düzeltildi.

---

© İsbak, 2023. Tüm hakları saklıdır. 