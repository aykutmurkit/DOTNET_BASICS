# Giriş

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## DeviceApi Nedir?

DeviceApi, IoT (Nesnelerin İnterneti) cihazlarının ve bunlarla ilişkili verilerin yönetimini basitleştirmek için tasarlanmış modern bir RESTful API'dir. ASP.NET Core üzerine inşa edilmiş olup, bir IoT ekosisteminde cihaz kaydı, izleme, kontrol ve veri toplama için kapsamlı bir çözüm sunar.

Özünde DeviceApi, fiziksel IoT cihazları ile bunlarla etkileşime girmesi gereken uygulamalar arasında bir köprü görevi görür. Cihaz iletişim protokollerinin, veri depolamanın ve güvenlik endişelerinin karmaşıklıklarını soyutlayarak geliştiricilere temiz, tutarlı bir arayüz sağlar.

## Temel Kavramlar

### Cihazlar

DeviceApi bağlamında, "cihaz", internete bağlanabilen ve API ile iletişim kurabilen herhangi bir IoT donanımını temsil eder. Her cihazın şunları vardır:

- Benzersiz bir tanımlayıcı
- Meta veriler (üretici, model, donanım yazılımı sürümü vb.)
- Durum bilgisi (çevrimiçi/çevrimdışı, pil seviyesi vb.)
- Yapılandırma ayarları
- İlişkili veri kayıtları

### Veri Noktaları

Cihazlar, "veri noktaları" olarak yakalanan veriler üretir. Bunlar şunları içerebilir:

- Sensör okumaları (sıcaklık, nem, basınç vb.)
- Durum değişiklikleri (kapı açık/kapalı, hareket algılandı vb.)
- İşletimsel metrikler (çalışma süresi, sinyal gücü vb.)
- Uyarı koşulları

### Gruplar ve Hiyerarşiler

Cihazlar, aşağıdakilere izin veren mantıksal gruplara düzenlenebilir:

- Organizasyonel ayrım (departman, konum, işlev vb. göre)
- Toplu işlemler (bir gruptaki tüm cihazları güncelleme)
- Erişim kontrolü (hangi kullanıcıların hangi grupları görebileceğini sınırlama)
- Toplu raporlama

### Kullanıcılar ve Roller

API, şunlara sahip rol tabanlı erişim kontrolünü destekler:

- Farklı izin seviyeleri (yönetici, operatör, görüntüleyici vb.)
- Çok kiracılı izolasyon
- Kullanıcı eylemlerinin denetim günlüğü

## Mimari Genel Bakış

DeviceApi, temiz, katmanlı bir mimariyi takip eder:

1. **API Katmanı**: İstemci uygulamaları için HTTP uç noktaları
2. **Servis Katmanı**: İş mantığı ve iş akışları
3. **Veri Erişim Katmanı**: Veri işlemleri için repository pattern
4. **Çekirdek Alan**: Varlık tanımları ve iş kuralları

### Teknoloji Yığını

- **Framework**: ASP.NET Core 8.0
- **Kimlik Doğrulama**: Yenileme tokenleri ile JWT tabanlı
- **Veritabanı**: SQL Server ile Entity Framework Core
- **Dokümantasyon**: Swagger/OpenAPI
- **Loglama**: Yapılandırılmış loglama ile Serilog
- **Mesajlaşma**: Azure Service Bus veya RabbitMQ ile opsiyonel entegrasyon
- **Önbellek**: Redis ile dağıtılmış önbellek

## Neden DeviceApi?

### Çözülen Sorunlar

DeviceApi, IoT cihaz yönetimindeki birkaç yaygın zorluğu ele alır:

- **Ölçeklenebilirlik**: Binlerce cihaz ve milyonlarca veri noktasını ele alacak şekilde tasarlanmıştır
- **Güvenlik**: Kapsamlı kimlik doğrulama, yetkilendirme ve veri koruma
- **Birlikte Çalışabilirlik**: Standart REST arayüzü, HTTP konuşan herhangi bir istemci ile çalışır
- **Esneklik**: Özelleştirilebilir cihaz türleri ve veri şemaları
- **Güvenilirlik**: Hata toleransı ve kurtarma düşünülerek inşa edilmiştir

### Kullanım Durumları

DeviceApi şunlar için idealdir:

- **Akıllı Bina Yönetimi**: HVAC, aydınlatma, erişim kontrolünü izleme ve kontrol etme
- **Endüstriyel IoT**: Fabrika ekipmanını takip etme, üretim hatlarını izleme
- **Çevresel İzleme**: Dağıtılmış sensör ağlarından veri toplama
- **Filo Yönetimi**: Araç telemetrisi ve bakım ihtiyaçlarını takip etme
- **Akıllı Şehir Altyapısı**: Sokak aydınlatması, park sensörleri vb. yönetimi

## Başlangıç

DeviceApi'yi anlamanın en hızlı yolu, onu eylem halinde görmektir. [Hızlı Başlangıç](03-Hizli-Baslangic.md) kılavuzu, bir geliştirme ortamı kurma ve ilk API çağrılarınızı yapma konusunda size rehberlik edecektir.

Uç noktaların ve işlevlerinin tam listesi için [API Uç Noktaları](04-API-Uc-Noktalari.md) dokümantasyonuna bakın.

---

[◀ Ana Sayfa](README.md) | [İleri: Kurulum ▶](02-Kurulum.md) 