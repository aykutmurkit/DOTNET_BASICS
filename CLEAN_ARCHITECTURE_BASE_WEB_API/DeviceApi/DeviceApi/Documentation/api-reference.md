# API Referansı

Bu bölüm, DeviceApi'nin tüm endpoint'lerini, parametrelerini ve yanıt formatlarını açıklar.

## İçindekiler

- [Temel URL](#temel-url)
- [Kimlik Doğrulama ve Yetkilendirme](#kimlik-doğrulama-ve-yetkilendirme)
- [Yanıt Formatı](#yanıt-formatı)
- [Rate Limiting](#rate-limiting)
- [Cihaz API](#cihaz-api)
  - [Tüm Cihazları Listele](#tüm-cihazları-listele)
  - [Cihaz Detayı](#cihaz-detayı)
  - [Cihaz Oluştur](#cihaz-oluştur)
  - [Cihaz Güncelle](#cihaz-güncelle)
  - [Cihaz Sil](#cihaz-sil)
- [Platform API](#platform-api)
  - [Tüm Platformları Listele](#tüm-platformları-listele)
  - [Platform Detayı](#platform-detayı)
  - [Platform Oluştur](#platform-oluştur)
  - [Platform Güncelle](#platform-güncelle)
  - [Platform Sil](#platform-sil)
- [İstasyon API](#i̇stasyon-api)
  - [Tüm İstasyonları Listele](#tüm-i̇stasyonları-listele)
  - [İstasyon Detayı](#i̇stasyon-detayı)
  - [İstasyon Oluştur](#i̇stasyon-oluştur)
  - [İstasyon Güncelle](#i̇stasyon-güncelle)
  - [İstasyon Sil](#i̇stasyon-sil)
- [Hata Kodları](#hata-kodları)

## Temel URL

API'nin temel URL'si:

```
https://localhost:7052/api
```

## Kimlik Doğrulama ve Yetkilendirme

DeviceApi, JWT tabanlı kimlik doğrulama kullanır. Kimlik doğrulaması, harici bir kimlik sağlayıcı tarafından gerçekleştirilir.

API'ye erişim için istek başlığında JWT token'ı şu şekilde gönderilmelidir:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

DeviceApi'deki endpoint'ler, rol tabanlı erişim kontrolüne göre korunur:

- **Halka Açık Endpoint'ler**: Herhangi bir kimlik doğrulama gerektirmeyen endpoint'ler (ör. dokümantasyon, sağlık kontrolü)
- **Kimlik Doğrulamalı Endpoint'ler**: Geçerli bir JWT token gerektiren, ancak özel bir rol gerektirmeyen endpoint'ler
- **Rol Tabanlı Endpoint'ler**: Belirli rollere (Admin, DeviceManager, vb.) sahip kullanıcılara özel endpoint'ler

Token doğrulama ve yetkilendirme işlemleri hakkında daha detaylı bilgi için [JWT Token Doğrulama Dokümantasyonu](jwt_authentication.md) sayfasına bakabilirsiniz.

## Yanıt Formatı

Tüm API yanıtları standart bir format kullanır:

**Başarılı Yanıt**:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": { ... },
  "message": "İşlem açıklaması"
}
```

**Hata Yanıtı**:

```json
{
  "statusCode": 400,
  "isSuccess": false,
  "errors": { 
    "property1": ["Hata mesajı 1", "Hata mesajı 2"],
    "property2": ["Hata mesajı"]
  },
  "message": "Hata açıklaması"
}
```

## Rate Limiting

API, istemcilerin yapabilecekleri istek sayısını sınırlamak için rate limiting uygular. Rate limit aşıldığında, API 429 (Too Many Requests) hatası döndürür ve "Retry-After" başlığında yeni bir istek yapmadan önce beklenecek saniye sayısını belirtir.

## Cihaz API

### Tüm Cihazları Listele

Sistemdeki tüm cihazları listeler.

**Endpoint**: `GET /Devices`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Kamera 1",
      "ip": "192.168.1.100",
      "port": 8080,
      "latitude": 41.0082,
      "longitude": 28.9784,
      "platform": {
        "id": 1,
        "name": "Güvenlik Platformu"
      }
    },
    {
      "id": 2,
      "name": "Sensör 1",
      "ip": "192.168.1.101",
      "port": 8081,
      "latitude": 41.0089,
      "longitude": 28.9789,
      "platform": {
        "id": 2,
        "name": "Çevre İzleme Platformu"
      }
    }
  ],
  "message": "Cihazlar başarıyla listelendi"
}
```

### Cihaz Detayı

Belirli bir cihazın detaylarını getirir.

**Endpoint**: `GET /Devices/{id}`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "Kamera 1",
    "ip": "192.168.1.100",
    "port": 8080,
    "latitude": 41.0082,
    "longitude": 28.9784,
    "platform": {
      "id": 1,
      "name": "Güvenlik Platformu"
    }
  },
  "message": "Cihaz başarıyla getirildi"
}
```

### Cihaz Oluştur

Yeni bir cihaz oluşturur.

**Endpoint**: `POST /Devices`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "Kamera 2",
  "ip": "192.168.1.102",
  "port": 8082,
  "latitude": 41.0090,
  "longitude": 28.9790,
  "platformId": 1
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Kamera 2",
    "ip": "192.168.1.102",
    "port": 8082,
    "latitude": 41.0090,
    "longitude": 28.9790,
    "platform": {
      "id": 1,
      "name": "Güvenlik Platformu"
    }
  },
  "message": "Cihaz başarıyla oluşturuldu"
}
```

### Cihaz Güncelle

Var olan bir cihazı günceller.

**Endpoint**: `PUT /Devices/{id}`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "Kamera 2 - Güncellendi",
  "ip": "192.168.1.102",
  "port": 8082,
  "latitude": 41.0091,
  "longitude": 28.9791,
  "platformId": 1
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Kamera 2 - Güncellendi",
    "ip": "192.168.1.102",
    "port": 8082,
    "latitude": 41.0091,
    "longitude": 28.9791,
    "platform": {
      "id": 1,
      "name": "Güvenlik Platformu"
    }
  },
  "message": "Cihaz başarıyla güncellendi"
}
```

### Cihaz Sil

Bir cihazı sistemden siler.

**Endpoint**: `DELETE /Devices/{id}`

**Yetki**: Admin

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": null,
  "message": "Cihaz başarıyla silindi"
}
```

## Platform API

### Tüm Platformları Listele

Sistemdeki tüm platformları listeler.

**Endpoint**: `GET /Platforms`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "Güvenlik Platformu",
      "description": "Güvenlik kameraları ve sensörlerini içeren platform"
    },
    {
      "id": 2,
      "name": "Çevre İzleme Platformu",
      "description": "Çevre sensörlerini içeren platform"
    }
  ],
  "message": "Platformlar başarıyla listelendi"
}
```

### Platform Detayı

Belirli bir platformun detaylarını getirir.

**Endpoint**: `GET /Platforms/{id}`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "Güvenlik Platformu",
    "description": "Güvenlik kameraları ve sensörlerini içeren platform",
    "devices": [
      {
        "id": 1,
        "name": "Kamera 1",
        "ip": "192.168.1.100",
        "port": 8080
      },
      {
        "id": 3,
        "name": "Kamera 2 - Güncellendi",
        "ip": "192.168.1.102",
        "port": 8082
      }
    ]
  },
  "message": "Platform başarıyla getirildi"
}
```

### Platform Oluştur

Yeni bir platform oluşturur.

**Endpoint**: `POST /Platforms`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "Trafik İzleme Platformu",
  "description": "Trafik kameraları ve sensörlerini içeren platform"
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Trafik İzleme Platformu",
    "description": "Trafik kameraları ve sensörlerini içeren platform"
  },
  "message": "Platform başarıyla oluşturuldu"
}
```

### Platform Güncelle

Var olan bir platformu günceller.

**Endpoint**: `PUT /Platforms/{id}`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "Trafik İzleme Platformu - Güncellendi",
  "description": "Trafik kameraları ve sensörlerini içeren güncellenmiş platform"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "Trafik İzleme Platformu - Güncellendi",
    "description": "Trafik kameraları ve sensörlerini içeren güncellenmiş platform"
  },
  "message": "Platform başarıyla güncellendi"
}
```

### Platform Sil

Bir platformu sistemden siler.

**Endpoint**: `DELETE /Platforms/{id}`

**Yetki**: Admin

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": null,
  "message": "Platform başarıyla silindi"
}
```

## İstasyon API

### Tüm İstasyonları Listele

Sistemdeki tüm istasyonları listeler.

**Endpoint**: `GET /Stations`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "name": "İstasyon 1",
      "description": "Ana kontrol istasyonu",
      "latitude": 41.0082,
      "longitude": 28.9784
    },
    {
      "id": 2,
      "name": "İstasyon 2",
      "description": "Yedek kontrol istasyonu",
      "latitude": 40.9896,
      "longitude": 29.0233
    }
  ],
  "message": "İstasyonlar başarıyla listelendi"
}
```

### İstasyon Detayı

Belirli bir istasyonun detaylarını getirir.

**Endpoint**: `GET /Stations/{id}`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "name": "İstasyon 1",
    "description": "Ana kontrol istasyonu",
    "latitude": 41.0082,
    "longitude": 28.9784
  },
  "message": "İstasyon başarıyla getirildi"
}
```

### İstasyon Oluştur

Yeni bir istasyon oluşturur.

**Endpoint**: `POST /Stations`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "İstasyon 3",
  "description": "Mobil kontrol istasyonu",
  "latitude": 41.0522,
  "longitude": 28.9913
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "İstasyon 3",
    "description": "Mobil kontrol istasyonu",
    "latitude": 41.0522,
    "longitude": 28.9913
  },
  "message": "İstasyon başarıyla oluşturuldu"
}
```

### İstasyon Güncelle

Var olan bir istasyonu günceller.

**Endpoint**: `PUT /Stations/{id}`

**Yetki**: Admin

**İstek**:

```json
{
  "name": "İstasyon 3 - Güncellendi",
  "description": "Mobil kontrol istasyonu - Güncellendi",
  "latitude": 41.0523,
  "longitude": 28.9914
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 3,
    "name": "İstasyon 3 - Güncellendi",
    "description": "Mobil kontrol istasyonu - Güncellendi",
    "latitude": 41.0523,
    "longitude": 28.9914
  },
  "message": "İstasyon başarıyla güncellendi"
}
```

### İstasyon Sil

Bir istasyonu sistemden siler.

**Endpoint**: `DELETE /Stations/{id}`

**Yetki**: Admin

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": null,
  "message": "İstasyon başarıyla silindi"
}
```

## Hata Kodları

| HTTP Kodu | Açıklama |
|-----------|----------|
| 400 | Bad Request - İstek formatı geçersiz |
| 401 | Unauthorized - Kimlik doğrulama gerekiyor |
| 403 | Forbidden - Yetkisiz erişim |
| 404 | Not Found - Kaynak bulunamadı |
| 409 | Conflict - Çakışma (örn. aynı isimde cihaz zaten var) |
| 422 | Unprocessable Entity - Geçersiz veri |
| 429 | Too Many Requests - Rate limit aşıldı |
| 500 | Internal Server Error - Sunucu hatası | 