# Loglama Sistemi

Bu bölüm, Deneme API'nin loglama sistemi hakkında bilgiler içerir.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Log Tipleri](#log-tipleri)
- [MongoDB Entegrasyonu](#mongodb-entegrasyonu)
- [API Endpoints](#api-endpoints)
- [Yapılandırma](#yapılandırma)
- [Güvenlik Önlemleri](#güvenlik-önlemleri)
- [Best Practices](#best-practices)

## Genel Bakış

Deneme API, kullanıcı aktivitelerini ve API işlemlerini kapsamlı bir şekilde izlemek için MongoDB tabanlı bir loglama sistemi kullanır. Bu sistem, gelen istekleri, yanıtları ve sistem olaylarını kaydeder ve analiz amacıyla saklar.

Loglama sistemi şu özellikleri sunar:

- İstek/yanıt loglaması
- API olay loglaması (bilgi, uyarı, hata)
- Hassas verilerin otomatik gizlenmesi
- Belirli endpoint'leri loglama dışında bırakma
- Loglar için TTL (Time-To-Live) ile otomatik silme
- Admin kullanıcıları için log görüntüleme API'leri

## Log Tipleri

### RequestResponseLog

İstek ve yanıt detaylarını içeren log tipidir.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | ObjectId | Benzersiz log ID'si |
| TraceId | string | İsteği tanımlamak için benzersiz iz ID'si |
| Path | string | İstek yapılan endpoint path'i |
| HttpMethod | string | HTTP metodu (GET, POST, vb.) |
| QueryString | string | URL sorgu parametreleri |
| RequestBody | string | İstek gövdesi (hassas veriler gizlenir) |
| ResponseBody | string | Yanıt gövdesi (hassas veriler gizlenir) |
| StatusCode | int | HTTP durum kodu |
| UserId | string | İsteği yapan kullanıcı ID'si |
| Username | string | İsteği yapan kullanıcı adı |
| UserIp | string | İsteği yapan kullanıcının IP adresi |
| RequestSize | long | İstek boyutu (byte) |
| ResponseSize | long | Yanıt boyutu (byte) |
| ExecutionTime | long | İsteğin işlenme süresi (ms) |
| Timestamp | DateTime | Log oluşturulma zamanı (UTC) |

### ApiLog

Genel API olaylarını içeren log tipidir.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | ObjectId | Benzersiz log ID'si |
| TraceId | string | İlgili iz ID'si |
| Level | string | Log seviyesi (Info, Warning, Error) |
| Message | string | Log mesajı |
| Exception | string | Hata detayı (varsa) |
| UserId | string | İlgili kullanıcı ID'si |
| Username | string | İlgili kullanıcı adı |
| Path | string | İlgili endpoint path'i |
| Timestamp | DateTime | Log oluşturulma zamanı (UTC) |

## MongoDB Entegrasyonu

Loglama sistemi, verileri saklamak için MongoDB kullanır. Sistem, aşağıdaki koleksiyonları oluşturur:

- **RequestResponseLogs**: İstek/yanıt logları
- **ApiLogs**: API olay logları

Her iki koleksiyon da, `Timestamp` alanı üzerinde TTL (Time-To-Live) indeksine sahiptir. Bu sayede loglar, belirli bir süre sonra otomatik olarak silinir.

## API Endpoints

### RequestResponse Logları

```
GET /api/logs/requests?pageNumber=1&pageSize=20&search=kullanıcı_adı
```

- **Yetki**: Admin
- **Parametreler**:
  - pageNumber: Sayfa numarası (varsayılan: 1)
  - pageSize: Sayfa başına kayıt sayısı (varsayılan: 20, maksimum: 100)
  - search: İsteğe bağlı arama terimi (kullanıcı adı, path, request/response gövdesinde arama yapar)

### API Logları

```
GET /api/logs/api?pageNumber=1&pageSize=20&level=Error&search=hata
```

- **Yetki**: Admin
- **Parametreler**:
  - pageNumber: Sayfa numarası (varsayılan: 1)
  - pageSize: Sayfa başına kayıt sayısı (varsayılan: 20, maksimum: 100)
  - level: Log seviyesi filtresi (Info, Warning, Error)
  - search: İsteğe bağlı arama terimi (mesaj, kullanıcı adı, path üzerinde arama yapar)

### Log Temizleme Endpoints

#### İstek/Yanıt Loglarını Temizle

```
DELETE /api/logs/requests?olderThanDays=30&path=/api/users
```

- **Yetki**: Admin
- **Parametreler**:
  - olderThanDays: Kaç günden eski logların silineceği (isteğe bağlı)
  - path: Belirli bir path içeren logları silmek için (isteğe bağlı)

#### API Loglarını Temizle

```
DELETE /api/logs/api?olderThanDays=30&level=Error
```

- **Yetki**: Admin
- **Parametreler**:
  - olderThanDays: Kaç günden eski logların silineceği (isteğe bağlı)
  - level: Belirli bir seviyedeki logları silmek için (isteğe bağlı)

#### Tüm Logları Temizle

```
DELETE /api/logs/all
```

- **Yetki**: Admin
- **Açıklama**: Tüm log kayıtlarını siler

### Test Endpoint'leri

```
GET /api/logs/test
GET /api/logs/test/warning
GET /api/logs/test/error
```

- **Yetki**: Herhangi bir yetkilendirilmiş kullanıcı
- **Açıklama**: Test amaçlı log kayıtları oluşturur (Info, Warning, Error)

## Yapılandırma

appsettings.json dosyasında loglama sistemi için aşağıdaki ayarlar kullanılabilir:

```json
"LogSettings": {
  "DatabaseName": "DenemeApiLogs",
  "ExpireAfterDays": 30,
  "ExcludedPaths": [
    "/api/logs",
    "/swagger",
    "/health"
  ]
}
```

- **DatabaseName**: MongoDB veritabanı adı
- **ExpireAfterDays**: Logların silinmeden önce saklanacağı gün sayısı
- **ExcludedPaths**: Loglanmayacak endpoint'lerin listesi

Ayrıca, MongoDB bağlantı dizesi için:

```json
"ConnectionStrings": {
  "MongoDb": "mongodb://localhost:27017"
}
```

## Güvenlik Önlemleri

Loglama sistemi, güvenlik açısından aşağıdaki önlemleri içerir:

1. **Hassas Verilerin Gizlenmesi**: Şifre, token, refreshToken gibi hassas bilgiler otomatik olarak gizlenir ve "***REDACTED***" ile değiştirilir.

2. **Endpoint Filtreleme**: Belirli endpoint'ler (örn. /api/logs, /swagger) loglama dışında bırakılabilir.

3. **Yetkilendirme**: Log erişim API'leri yalnızca Admin rolüne sahip kullanıcılar tarafından kullanılabilir.

4. **TTL ile Otomatik Silme**: Loglar, belirli bir süre sonra otomatik olarak silinir.

## Best Practices

### Loglama Sistemi Kullanımı

1. **Doğru Log Seviyesi**: Log mesajları için doğru seviye kullanılmalıdır:
   - **Info**: Normal işlem bilgileri
   - **Warning**: Potansiyel sorunlar
   - **Error**: Gerçek hatalar ve istisnalar

2. **Anlamlı Mesajlar**: Log mesajları anlamlı ve açıklayıcı olmalıdır.

3. **Exception Detayları**: Hata loglarında, exception bilgileri de kaydedilmelidir.

### Log Erişimi ve Analizi

1. **Düzenli İzleme**: Sistem hataları için error loglar düzenli olarak kontrol edilmelidir.

2. **Şüpheli Aktivite**: Başarısız kimlik doğrulama denemeleri gibi şüpheli aktiviteler için loglar incelenmelidir.

3. **Performans Analizi**: İstek/yanıt loglarındaki ExecutionTime değeri, performans sorunlarını tespit etmek için kullanılabilir.

---

Loglama sistemi, Deneme API'nin güvenliğini, hata ayıklamasını ve izlenmesini kolaylaştırmak için tasarlanmıştır. Sistem, geliştiricilere ve yöneticilere, API kullanımı hakkında değerli bilgiler sağlar. 