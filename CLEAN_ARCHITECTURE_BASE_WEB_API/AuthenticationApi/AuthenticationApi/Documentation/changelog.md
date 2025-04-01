# Değişiklik Geçmişi (Changelog)

Bu belge, Deneme API'de yapılan tüm önemli güncellemeleri, yeni özellikleri ve hata düzeltmelerini kronolojik sırada listeler.

## v1.4.0 - 2024-06-15

### Eklenen Özellikler

- **Kullanıcı Yönetimi İyileştirmeleri**:
  - **Random Şifre ile Kullanıcı Oluşturma**: Admin kullanıcılar artık rastgele güçlü şifre oluşturarak yeni kullanıcılar ekleyebilir. Oluşturulan şifre otomatik olarak kullanıcının e-posta adresine gönderilir.
  - **Kullanıcı Rolü Güncelleme**: Kullanıcıların sadece rol bilgisini güncellemek için yeni bir PATCH endpoint'i eklendi.
  - **Kullanıcı E-posta Güncelleme**: Kullanıcıların sadece e-posta adresini güncellemek için yeni bir PATCH endpoint'i eklendi.

- **E-posta İyileştirmeleri**:
  - Şık tasarımlı HTML e-posta şablonları eklendi
  - Yeni kullanıcılar için "Hoş Geldiniz" e-posta şablonu eklendi

### İyileştirmeler

- Şifre oluşturma algoritması güçlendirildi
- E-posta gönderim işlemi daha güvenilir hale getirildi
- Kullanıcı dokümantasyonu güncellendi

## v1.3.0 - 2024-05-20

### Eklenen Özellikler

- **Loglama Sistemi**:
  - MongoDB tabanlı kapsamlı loglama sistemi eklendi
  - Kullanıcı aktivitelerini, API işlemlerini ve hataları kaydeder
  - Belirli endpoint'leri loglama dışında bırakma özelliği
  - Hassas verilerin otomatik gizlenmesi
  - Logları görüntüleme ve temizleme için API endpoint'leri

- **API Endpoint Testleri**:
  - Loglama test endpoint'leri eklendi

### İyileştirmeler

- Veritabanı bağlantısı optimizasyonları
- Rate limiting algoritması geliştirildi
- Dokümantasyon güncellemeleri

### Hata Düzeltmeleri

- Refresh token yenilemede yaşanan zamanlama sorunları giderildi
- Bazı endpoint'lerde eksik validasyon kontrolleri eklendi

## v1.2.0 - 2024-04-10

### Eklenen Özellikler

- **Rol Tabanlı Erişim Kontrolü**:
  - Daha detaylı yetkilendirme sistemi
  - Admin, Developer ve User rolleri için özel erişim hakları
  - Rol tabanlı erişim kontrolünü API düzeyinde uygulama

- **Rate Limiting**:
  - IP ve endpoint bazlı rate limiting
  - Güvenlik açısından kritik endpoint'ler için özel limitler
  - DoS koruması için genel API limitleri

### İyileştirmeler

- JWT token güvenliği artırıldı
- API yanıt formatı standardize edildi
- Veritabanı sorgu performansı iyileştirildi

## v1.1.0 - 2024-03-15

### Eklenen Özellikler

- **İki Faktörlü Kimlik Doğrulama (2FA)**:
  - E-posta tabanlı doğrulama kodu sistemi
  - Kullanıcılar için 2FA etkinleştirme/devre dışı bırakma
  - Sistem genelinde 2FA zorunlu kılma seçeneği
  - 2FA kodu doğrulama işlemi

### İyileştirmeler

- Şifre güvenlik gereksinimleri güçlendirildi
- Kullanıcı profil yönetimi genişletildi
- JWT token ömrü optimize edildi

### Hata Düzeltmeleri

- Belirli durumlarda token yenileme sorunu giderildi
- Kullanıcı kaydında doğrulama hataları düzeltildi

## v1.0.0 - 2024-02-01

### İlk Sürüm

- **Temel Kimlik Doğrulama**:
  - JWT tabanlı kimlik doğrulama
  - Kullanıcı kaydı ve girişi
  - Şifre sıfırlama
  - Access ve refresh token yönetimi

- **Kullanıcı Yönetimi**:
  - Kullanıcı profili oluşturma ve güncelleme
  - Profil fotoğrafı yükleme
  - Admin tarafından kullanıcı yönetimi

- **Temel API Özellikleri**:
  - Standardize edilmiş API yanıtları
  - Hata yönetimi
  - Validasyon filtreleri 