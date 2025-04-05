# En İyi Uygulamalar

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu belge, DeviceApi ile çalışırken optimal performans, güvenlik ve bakım yapılabilirliği sağlamak için en iyi uygulama kılavuzları sunmaktadır.

## Güvenlik En İyi Uygulamaları

### Kimlik Doğrulama ve Yetkilendirme

- **Token Yönetimi**: Tokenleri güvenli bir şekilde saklayın ve hiçbir zaman istemci tarafı kodunda veya URL'lerde açığa çıkarmayın
- **Token Yenileme**: Süresi dolan erişim tokenlerini ele almak için token yenileme mantığı uygulayın
- **En Az Ayrıcalık**: Yalnızca uygulamanızın ihtiyaç duyduğu izinleri talep edin
- **Rol Ayrımı**: Yöneticiler, operatörler ve okuyucular için ayrı roller oluşturun
- **Her Zaman HTTPS Kullanın**: Kimlik bilgilerini veya tokenleri hiçbir zaman şifrelenmemiş bağlantılar üzerinden göndermeyin

### Veri Koruma

- **Hassas Veriler**: Cihaz meta verilerinde veya özel özelliklerde hassas verileri saklamayın
- **Girdi Doğrulama**: API'ye göndermeden önce tüm kullanıcı girdilerini doğrulayın
- **Çıktı Kodlama**: Kullanıcı tarafından oluşturulan içerik barındıran API yanıtlarını kodlayın
- **Cihaz Kimlik Bilgileri**: Cihaz kimlik bilgilerini periyodik olarak değiştirin
- **Kişisel Veriler**: Herhangi bir kişisel tanımlayıcı bilgiyi (PII) hashleyin veya şifreleyin

### Saldırı Önleme

- **Hız Sınırlama**: Hesap kilitlenmelerini önlemek için istemci tarafı hız sınırlaması uygulayın
- **İstek İmzalama**: Ek güvenlik için API isteklerini imzalamayı düşünün
- **Yeniden Oynatma Koruması**: Hassas işlemlerde zaman damgaları veya nonce'lar ekleyin
- **CSRF Koruması**: Tarayıcı tabanlı uygulamalar için anti-CSRF tokenları kullanın
- **Hata Yönetimi**: Son kullanıcılara asla ayrıntılı hata bilgilerini açıklamayın

## Performans En İyi Uygulamaları

### İstek Optimizasyonu

- **Toplu İşlemler**: Mümkün olduğunda toplu işlemler için batch uç noktalarını kullanın
- **Sayfalandırma**: Koleksiyon uç noktaları için her zaman sayfalandırma kullanın
- **Alan Seçimi**: Yanıt boyutunu sınırlamak için alan seçim parametrelerini kullanın
- **Sıkıştırma**: API istekleri/yanıtları için gzip veya Brotli sıkıştırmasını etkinleştirin
- **Koşullu İstekler**: Veri transferini azaltmak için ETags ve koşullu istekleri kullanın

### Önbellekleme

- **Yanıt Önbellekleme**: API yanıtlarını Cache-Control başlıklarına göre önbelleğe alın
- **Kaynak Önbellekleme**: Cihaz yapılandırmaları gibi sık erişilen kaynakları önbelleğe alın
- **Geçersiz Kılma Stratejisi**: Kaynaklar değiştiğinde uygun önbellek geçersiz kılma mekanizması uygulayın
- **Stale-While-Revalidate**: Arka planda taze veri alırken bayat veri kullanmayı düşünün
- **Önbellek Hiyerarşileri**: Farklı veri türleri için çok seviyeli önbellekleme uygulayın

### Bağlantı Yönetimi

- **Bağlantı Havuzu**: Birden fazla istek yaparken HTTP bağlantılarını yeniden kullanın
- **Kalıcı Bağlantılar**: Kalıcı bağlantılar için keep-alive kullanın
- **İstek Zaman Aşımları**: API istekleri için uygun zaman aşımları ayarlayın
- **Yeniden Deneme Mantığı**: Başarısız istekler için üstel geri çekilme ile yeniden deneme mantığı uygulayın
- **Devre Kesiciler**: Zincirleme arızaları önlemek için devre kesiciler kullanın

## Entegrasyon En İyi Uygulamaları

### API İstemci Tasarımı

- **İstemci Kütüphaneleri**: Yaygın konuları ele alan istemci kütüphanelerini kullanın veya oluşturun
- **Loglama**: Sorun giderme için tüm API etkileşimlerini loglayın
- **Metrik Toplama**: API çağrı performansını ve başarı oranlarını izleyin
- **Hata Yönetimi**: Tüm API çağrıları için kapsamlı hata yönetimi uygulayın
- **Serileştirme**: JSON işleme için uygun serileştirme kütüphanelerini kullanın

### Webhook İşleme

- **İmza Doğrulama**: Her zaman webhook imzalarını doğrulayın
- **İdempotens**: Olası kopyaları ele almak için webhook'ları idempotent şekilde işleyin
- **Asenkron İşleme**: Webhook yüklerini asenkron olarak işleyin
- **Kalıcılık**: İşlemeden önce webhook yüklerini saklayın
- **Kuyruk Yönetimi**: Webhook işleme yükünü yönetmek için kuyruklar kullanın

### Dağıtım Hususları

- **API Sürümlendirme**: İsteklerinizde API sürümlerini açıkça belirtin
- **Ortam Ayrımı**: Geliştirme ve üretim için ayrı API kimlik bilgilerini kullanın
- **İzleme**: API kullanılabilirliği ve performansı için izleme uygulayın
- **Uyarı Sistemi**: API hataları ve performans düşüşleri için uyarılar ayarlayın
- **Dokümantasyon**: Dahili dokümantasyonu API değişiklikleriyle güncel tutun

## Cihaz Yönetimi En İyi Uygulamaları

### Cihaz Kaydı

- **Benzersiz Tanımlayıcılar**: Tüm cihazlar için global olarak benzersiz tanımlayıcılar kullanın
- **Cihaz Meta Verileri**: Cihaz kaydı sırasında kapsamlı meta veriler ekleyin
- **Gruplama Stratejisi**: İlgili cihazları gruplamak için tutarlı bir strateji geliştirin
- **Yaşam Döngüsü Yönetimi**: Net cihaz yaşam döngüsü aşamaları tanımlayın (provizyon, aktif, hizmet dışı)
- **Toplu Kayıt**: Birden fazla cihaz dağıtırken toplu kayıt kullanın

### Veri Toplama

- **Örnekleme Hızı**: Kullanım durumunuz için uygun veri örnekleme hızlarını seçin
- **Veri Toplama**: Mümkün olduğunda yük boyutunu azaltmak için cihazda veri toplamayı gerçekleştirin
- **Zaman Damgası Hassasiyeti**: Tutarlı zaman damgası formatları ve saat dilimleri kullanın
- **Toplu İşlem**: Birden fazla okuma gönderirken veri noktalarını gruplandırın
- **Önceliklendirme**: Bant genişliği kısıtlı ortamlarda kritik verilere öncelik verin

### Donanım Yazılımı ve Güncellemeler

- **Sürüm İzleme**: Cihaz donanım yazılımı sürümlerini takip edin
- **Aşamalı Dağıtımlar**: Donanım yazılımı güncellemeleri için aşamalı dağıtımlar uygulayın
- **Geri Alma Yeteneği**: Her zaman önceki sürümlere geri dönme yeteneğini koruyun
- **Güncelleme Doğrulama**: Kurulumdan önce ve sonra güncellemelerin bütünlüğünü doğrulayın
- **Test**: Güncellemeleri üretim ortamına dağıtmadan önce bir hazırlık ortamında kapsamlı bir şekilde test edin

## En İyi Uygulamalar Kontrol Listesi

DeviceApi entegrasyonunuzda en iyi uygulamaları izlediğinizden emin olmak için bu kontrol listesini kullanın:

### Güvenlik Kontrol Listesi

- [ ] Tüm API iletişimleri için HTTPS kullanılır
- [ ] Tokenler güvenli bir şekilde saklanır ve hiçbir zaman istemci tarafı kodunda açığa çıkarılmaz
- [ ] Tüm kullanıcı girdileri için girdi doğrulama uygulanır
- [ ] Token yenileme mekanizması mevcuttur
- [ ] Hassas veriler uygun şekilde şifrelenir veya hashlenir

### Performans Kontrol Listesi

- [ ] Tüm koleksiyon uç noktaları için sayfalandırma kullanılır
- [ ] Yanıt önbellekleme Cache-Control başlıklarına göre uygulanır
- [ ] Uygun olduğunda toplu işlemler kullanılır
- [ ] Bağlantı havuzu ve kalıcı bağlantılar kullanılır
- [ ] Uygun istek zaman aşımları ve yeniden deneme mantığı uygulanır

### Entegrasyon Kontrol Listesi

- [ ] İsteklerde API sürümü açıkça belirtilir
- [ ] Kapsamlı hata yönetimi uygulanır
- [ ] API etkileşimleri için loglama ve izleme mevcuttur
- [ ] Webhook işleyicileri imzaları doğrular ve yükleri idempotent bir şekilde işler
- [ ] Farklı ortamlar için ayrı kimlik bilgileri kullanılır

### Cihaz Yönetimi Kontrol Listesi

- [ ] Cihazların global olarak benzersiz tanımlayıcıları vardır
- [ ] Cihaz kaydı sırasında kapsamlı meta veriler eklenir
- [ ] Tutarlı bir gruplama stratejisi tanımlanmıştır
- [ ] Veri örnekleme hızları kullanım durumuna uygundur
- [ ] Donanım yazılımı sürüm izleme ve güncelleme prosedürleri mevcuttur

## Kaçınılması Gereken Anti-Kalıplar

### Güvenlik Anti-Kalıpları

- **Sabit Kodlu Kimlik Bilgileri**: Kimlik bilgilerini veya tokenleri hiçbir zaman kod tabanınıza sabit kodlamayın
- **Aşırı İzinler**: Gerekenden fazla izin talep etmekten veya vermekten kaçının
- **Sertifika Doğrulamasını Göz Ardı Etme**: Hiçbir zaman SSL/TLS sertifika doğrulamasını devre dışı bırakmayın
- **Güvensiz Depolama**: Tokenleri veya hassas verileri yerel depolamada veya çerezlerde koruma olmadan saklamayın
- **Belirsizlikle Güvenlik**: Ana güvenlik mekanizmanız olarak obfuscation'a güvenmeyin

### Performans Anti-Kalıpları

- **N+1 Sorgular**: Tek bir toplu istek yeterli olduğunda birden fazla istek yapmaktan kaçının
- **Polling Suistimali**: Webhook'lar veya uzun polling mevcut olduğunda aşırı polling yapmayın
- **Sayfalandırmayı Görmezden Gelme**: Hiçbir zaman sayfalandırma olmadan tüm kaynakları almaya çalışmayın
- **Gereksiz İstekler**: Önbellekleme olmadan aynı verileri tekrar tekrar istemekten kaçının
- **Senkron İşleme**: Ana iş parçacığını senkron API çağrılarıyla bloke etmeyin

### Entegrasyon Anti-Kalıpları

- **Sürüm Sabitleme**: Bir geçiş planı olmadan belirli API sürümlerine katı bir şekilde sabitlemekten kaçının
- **Durum Kodlarını Görmezden Gelme**: Her zaman HTTP durum kodlarını uygun şekilde kontrol edin ve işleyin
- **Sessiz Hatalar**: API hatalarını sessizce görmezden gelmeyin
- **Doğrudan Veritabanı Erişimi**: API'yi atlayıp doğrudan veritabanına erişmeyin
- **Sıkı Bağlı Kod**: İş mantığınızı API istemcisine sıkı bir şekilde bağlamaktan kaçının

### Cihaz Yönetimi Anti-Kalıpları

- **Tutarsız Adlandırma**: Tutarsız cihaz adlandırma veya tanımlama şemalarından kaçının
- **Yapılandırılmamış Meta Veriler**: Cihaz meta verilerinde yapılandırılmamış veya keyfi veriler saklamayın
- **Özel Özelliklerin Aşırı Yüklenmesi**: Temel cihaz öznitelikleri için özel özellikleri kullanmaktan kaçının
- **Aşırı Veri İletimi**: Gerekenden daha sık veri göndermeyin
- **Cihaz Yaşam Döngüsünü İhmal Etme**: Eksiksiz cihaz yaşam döngüsünü yönetmeyi ihmal etmeyin

---

[◀ Versiyonlama](08-Versiyonlama.md) | [Ana Sayfa](README.md) | [İleri: Hata Yönetimi ▶](10-Hata-Yonetimi.md) 