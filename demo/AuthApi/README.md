<!-- METADATA CARD -->
<div align="center">
  <h1 align="left">Auth API</h1>
  <table>
    <tr>
      <td><strong>Author:</strong></td>
      <td>Aykut Mürkit</td>
    </tr>
    <tr>
      <td><strong>Title:</strong></td>
      <td>R&D Engineer</td>
    </tr>
    <tr>
      <td><strong>Unit:</strong></td>
      <td>Smart City and Geographic Information Systems Department</td>
    </tr>
    <tr>
      <td><strong>Unit Chief:</strong></td>
      <td>Mahmut Bulut</td>
    </tr>
    <tr>
      <td><strong>Department:</strong></td>
      <td>R&D Directorate</td>
    </tr>
    <tr>
      <td><strong>Company:</strong></td>
      <td>ISBAK Inc. (Istanbul Metropolitan Municipality)</td>
    </tr>
    <tr>
      <td><strong>Email:</strong></td>
      <td>aykutmurkit@outlook.com / amurkit@isbak.com.tr</td>
    </tr>
    <tr>
      <td><strong>Created:</strong></td>
      <td>2025-04-16</td>
    </tr>
    <tr>
      <td><strong>Last Modified:</strong></td>
      <td>2025-04-16</td>
    </tr>
  </table>
</div>


# AuthApi Dokümantasyonu

Bu dizin, AuthApi projesi ile ilgili kapsamlı dokümantasyonu içermektedir. Her bir bölüm, projenin farklı bir yönünü detaylandırmaktadır.

## İçindekiler

1. [Giriş ve Genel Bakış](01-Introduction.md) - Proje hakkında genel bilgi, özellikler ve teknoloji stack
2. [Mimari Yapı](02-Architecture.md) - Projenin mimari tasarımı ve katmanlı yapısı
3. [Kimlik Doğrulama ve Yetkilendirme](03-Authentication.md) - JWT, 2FA ve rol tabanlı erişim kontrolü
4. [Loglama Sistemi](04-Logging.md) - MongoDB tabanlı loglama sistemi ve veri maskeleme
5. [API Endpoints](05-API-Endpoints.md) - API endpoint'lerinin detayları ve örnek kullanımlar
6. [Veritabanı Seeding](06-Database-Seeding.md) - Başlangıç verilerinin yüklenmesi
7. [Hata Yönetimi](07-Error-Handling.md) - Hata işleme ve standart API yanıtları

## Dokümantasyon Kullanımı

- Her bölüm kendi içinde tam bir referans olarak çalışmak üzere tasarlanmıştır
- Kod örnekleri proje kaynak kodundan alınmıştır ve güncel durumu yansıtmaktadır
- Dokümantasyonu en verimli şekilde kullanmak için markdown dosyalarını markdown görüntüleyici ile açınız

## Başlarken

Yeni başlayanlar için tavsiye edilen okuma sırası:

1. [Giriş ve Genel Bakış](01-Introduction.md) - Proje hakkında genel bir anlayış edinmek için
2. [Mimari Yapı](02-Architecture.md) - Projenin genel mimarisini anlamak için
3. [API Endpoints](05-API-Endpoints.md) - API'nin nasıl kullanılacağını öğrenmek için

Ardından diğer bölümleri ihtiyaca göre inceleyebilirsiniz.

## Örnek Senaryolar

### Kimlik Doğrulama İşlemi

1. Kullanıcı `/api/Auth/login` endpoint'ine kimlik bilgilerini gönderir
2. 2FA gerekiyorsa, kullanıcı e-postasına gönderilen kodu kullanarak `/api/Auth/verify-2fa` endpoint'ine istek gönderir
3. Başarılı kimlik doğrulama sonrası, JWT token ile API'yi kullanmaya başlar

### Kullanıcı Yönetimi

1. Admin kullanıcısı `/api/Users` endpoint'ini kullanarak tüm kullanıcıları listeleyebilir
2. Yeni bir kullanıcı `/api/Users` endpoint'ine POST isteği ile oluşturulabilir
3. Kullanıcı profili `/api/Users/profile` endpoint'i üzerinden güncellenebilir

### Loglama ve Hata Yönetimi

1. Tüm API istekleri ve yanıtları otomatik olarak loglanır
2. Hatalar, HTTP durum kodları ve açıklayıcı mesajlarla birlikte tutarlı bir formatta döndürülür

## API Testi

API'yi test etmek için [Swagger](https://swagger.io/) veya [Postman](https://www.postman.com/) gibi araçlar kullanabilirsiniz. Development ortamında Swagger UI otomatik olarak `/swagger` endpoint'inde sunulmaktadır.

## Daha Fazla Bilgi

Projenin diğer yönleri hakkında daha fazla bilgi için lütfen ilgili dokümantasyon bölümlerini inceleyiniz. 