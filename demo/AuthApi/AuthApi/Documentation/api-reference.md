# API Referansı

Bu bölüm, Deneme API'nin tüm endpoint'lerini, parametrelerini ve yanıt formatlarını açıklar.

## İçindekiler

- [Temel URL](#temel-url)
- [Yetkilendirme](#yetkilendirme)
- [Yanıt Formatı](#yanıt-formatı)
- [Rate Limiting](#rate-limiting)
- [Kimlik Doğrulama API](#kimlik-doğrulama-api)
  - [Kullanıcı Kaydı](#kullanıcı-kaydı)
  - [Kullanıcı Girişi](#kullanıcı-girişi)
  - [Token Yenileme](#token-yenileme)
  - [2FA Durumu](#2fa-durumu)
  - [2FA Ayarları](#2fa-ayarları)
  - [2FA Doğrulama](#2fa-doğrulama)
  - [Şifre Sıfırlama İsteği](#şifre-sıfırlama-i̇steği)
  - [Şifre Sıfırlama](#şifre-sıfırlama)
- [Kullanıcı API](#kullanıcı-api)
  - [Tüm Kullanıcıları Listele](#tüm-kullanıcıları-listele)
  - [Kullanıcı Detayı](#kullanıcı-detayı)
  - [Kullanıcı Profili](#kullanıcı-profili)
  - [Kullanıcı Oluştur](#kullanıcı-oluştur)
  - [Kullanıcı Güncelle](#kullanıcı-güncelle)
  - [Profil Güncelle](#profil-güncelle)
  - [Kullanıcı Sil](#kullanıcı-sil)
  - [Profil Fotoğrafı Yükle](#profil-fotoğrafı-yükle)
  - [Profil Fotoğrafı Görüntüle](#profil-fotoğrafı-görüntüle)
- [Hata Kodları](#hata-kodları)

## Temel URL

API'nin temel URL'si:

```
https://localhost:7052/api
```

## Yetkilendirme

Çoğu endpoint, JWT tabanlı yetkilendirme gerektirir. Yetkilendirme, Authorization başlığında Bearer token olarak sağlanmalıdır:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

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

API, istemcilerin yapabilecekleri istek sayısını sınırlamak için rate limiting uygular. Bu, API'nin istikrarını korumak, hizmet reddi (DoS) saldırılarını önlemek ve adil kullanımı sağlamak için tasarlanmıştır.

### Genel API Limitleri

- **Genel Limit**: Her IP adresi için dakikada maksimum 200 istek
- **IP Bazlı Limit**: Her IP adresi için dakikada maksimum 100 istek

### Endpoint Spesifik Limitler

| Endpoint | Limit | Periyot | Açıklama |
|----------|-------|---------|----------|
| `/api/Auth/login` | 10 | 5 dakika | Maksimum 5 eş zamanlı istek |
| `/api/Auth/register` | 3 | 10 dakika | Yeni hesap oluşturma limiti |
| `/api/Auth/forgot-password` | 3 | 30 dakika | Şifre sıfırlama limiti |
| `/api/Auth/verify-2fa` | 5 | 5 dakika | 2FA doğrulama limiti |
| `/api/Users/profile-picture` | 10 | 1 dakika | Profil fotoğrafı yükleme limiti |

### Rate Limit Aşıldığında

Rate limit aşıldığında, API aşağıdaki yanıtı döndürür:

- **HTTP Durum Kodu**: 429 (Too Many Requests)
- **Yanıt Gövdesi**:

```json
{
  "statusCode": 429,
  "isSuccess": false,
  "message": "İstek limiti aşıldı. Lütfen daha sonra tekrar deneyin.",
  "retryAfter": 60
}
```

### Rate Limiting Başlıkları

API, rate limit durumu hakkında bilgi veren HTTP başlıkları içerir:

- **Retry-After**: Yeni bir istek yapmadan önce beklenecek saniye sayısı

### Rate Limiting En İyi Uygulamalar

- İstek sayısını azaltmak için uygun önbelleğe alma stratejileri kullanın
- Bir token alırken eksponansiyel geri çekilme uygulamak için `Retry-After` başlığını kullanın
- Rate limit'e takılma olasılığı yüksek endpoint'ler için iş mantığınızda yeniden deneme mekanizmaları ekleyin

## Kimlik Doğrulama API

### Kullanıcı Kaydı

Yeni bir kullanıcı hesabı oluşturur.

**Endpoint**: `POST /Auth/register`

**Yetki**: Herkese açık

**Rate Limit**: 10 dakikada 3 istek

**İstek**:

```json
{
  "username": "ornek_kullanici",
  "email": "ornek@mail.com",
  "password": "Guclu_Sifre123!",
  "confirmPassword": "Guclu_Sifre123!"
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-03-30T08:30:00Z"
    },
    "refreshToken": {
      "token": "RZgGVhCO7bqf6zVHRG3fJWMXWJUHd2T3mMdUjx2hHko=",
      "expiresAt": "2025-04-06T08:30:00Z"
    },
    "user": {
      "id": 5,
      "username": "ornek_kullanici",
      "email": "ornek@mail.com",
      "role": {
        "id": 1,
        "name": "User"
      }
    }
  },
  "message": "Kullanıcı başarıyla kaydedildi"
}
```

### Kullanıcı Girişi

Mevcut kullanıcı kimlik bilgileri ile giriş yapar.

**Endpoint**: `POST /Auth/login`

**Yetki**: Herkese açık

**Rate Limit**: 5 dakikada 10 istek

**İstek**:

```json
{
  "username": "ornek_kullanici",
  "password": "Guclu_Sifre123!"
}
```

**Yanıt** (200 OK - Normal Giriş):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-03-30T08:30:00Z"
    },
    "refreshToken": {
      "token": "RZgGVhCO7bqf6zVHRG3fJWMXWJUHd2T3mMdUjx2hHko=",
      "expiresAt": "2025-04-06T08:30:00Z"
    },
    "user": {
      "id": 5,
      "username": "ornek_kullanici",
      "email": "ornek@mail.com",
      "role": {
        "id": 1,
        "name": "User"
      }
    }
  },
  "message": "Giriş başarılı"
}
```

**Yanıt** (200 OK - 2FA Gerekli):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "userId": 5,
    "email": "ornek@mail.com",
    "username": "ornek_kullanici",
    "requiresTwoFactor": true,
    "message": "İki faktörlü kimlik doğrulama gerekli. E-posta adresinize gönderilen kodu giriniz."
  },
  "message": "İki faktörlü kimlik doğrulama gerekli"
}
```

### Token Yenileme

Refresh token kullanarak yeni bir access token alır.

**Endpoint**: `POST /Auth/refresh-token`

**Yetki**: Herkese açık

**İstek**:

```json
{
  "refreshToken": "RZgGVhCO7bqf6zVHRG3fJWMXWJUHd2T3mMdUjx2hHko="
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-03-30T08:30:00Z"
    },
    "refreshToken": {
      "token": "YtPR8vF4J9bTwZ3mHsA7qKxDgE5nL1cXoV2iW6yUzNp=",
      "expiresAt": "2025-04-06T08:30:00Z"
    },
    "user": {
      "id": 5,
      "username": "ornek_kullanici",
      "email": "ornek@mail.com",
      "role": {
        "id": 1,
        "name": "User"
      }
    }
  },
  "message": "Token başarıyla yenilendi"
}
```

### 2FA Durumu

Kullanıcının iki faktörlü kimlik doğrulama durumunu getirir.

**Endpoint**: `GET /Auth/2fa-status`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "enabled": true,
    "isGloballyRequired": false,
    "message": "İki faktörlü kimlik doğrulama etkin."
  },
  "message": "2FA durumu"
}
```

### 2FA Ayarları

Kullanıcının iki faktörlü kimlik doğrulama ayarlarını günceller.

**Endpoint**: `POST /Auth/setup-2fa`

**Yetki**: Kimliği doğrulanmış kullanıcı

**İstek**:

```json
{
  "enabled": true,
  "currentPassword": "Kullanici_Sifresi123!"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "enabled": true,
    "isGloballyRequired": false,
    "message": "İki faktörlü kimlik doğrulama başarıyla etkinleştirildi."
  },
  "message": "2FA ayarları güncellendi"
}
```

### 2FA Doğrulama

İki faktörlü kimlik doğrulama kodunu doğrular ve giriş işlemini tamamlar.

**Endpoint**: `POST /Auth/verify-2fa`

**Yetki**: Herkese açık

**Rate Limit**: 5 dakikada 5 istek

**İstek**:

```json
{
  "userId": 5,
  "code": "123456"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-03-30T08:30:00Z"
    },
    "refreshToken": {
      "token": "RZgGVhCO7bqf6zVHRG3fJWMXWJUHd2T3mMdUjx2hHko=",
      "expiresAt": "2025-04-06T08:30:00Z"
    },
    "user": {
      "id": 5,
      "username": "ornek_kullanici",
      "email": "ornek@mail.com",
      "role": {
        "id": 1,
        "name": "User"
      }
    }
  },
  "message": "İki faktörlü kimlik doğrulama başarılı"
}
```

### Şifre Sıfırlama İsteği

Şifre sıfırlama e-postası gönderir.

**Endpoint**: `POST /Auth/forgot-password`

**Yetki**: Herkese açık

**Rate Limit**: 30 dakikada 3 istek

**İstek**:

```json
{
  "email": "ornek@mail.com"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi"
}
```

### Şifre Sıfırlama

Şifre sıfırlama işlemini tamamlar.

**Endpoint**: `POST /Auth/reset-password`

**Yetki**: Herkese açık

**İstek**:

```json
{
  "token": "sifre-sifirlama-token",
  "email": "ornek@mail.com",
  "newPassword": "Yeni_Guclu_Sifre123!",
  "confirmNewPassword": "Yeni_Guclu_Sifre123!"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Şifreniz başarıyla değiştirildi"
}
```

## Kullanıcı API

### Tüm Kullanıcıları Listele

Tüm kullanıcıları listeler.

**Endpoint**: `GET /Users`

**Yetki**: Admin

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "username": "admin",
      "email": "admin@example.com",
      "role": {
        "id": 3,
        "name": "Admin"
      },
      "createdDate": "2025-03-30T01:41:29.0298257",
      "twoFactor": {
        "enabled": false,
        "required": false
      },
      "profilePicture": {
        "hasProfilePicture": false
      }
    },
    // ... diğer kullanıcılar
  ],
  "message": "Kullanıcılar başarıyla getirildi"
}
```

### Kullanıcı Detayı

Belirli bir kullanıcının detaylarını getirir.

**Endpoint**: `GET /Users/{id}`

**Yetki**: Admin, Developer, veya kendi profilini görüntüleyen kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 1,
    "username": "admin",
    "email": "admin@example.com",
    "role": {
      "id": 3,
      "name": "Admin"
    },
    "createdDate": "2025-03-30T01:41:29.0298257",
    "lastLoginDate": "2025-03-30T01:42:10.1234567",
    "twoFactor": {
      "enabled": false,
      "required": false
    },
    "profilePicture": {
      "hasProfilePicture": true,
      "url": "/api/Users/1/profile-picture",
      "picture": "base64_encoded_image_data"
    }
  },
  "message": "Kullanıcı başarıyla getirildi"
}
```

### Kullanıcı Profili

Giriş yapmış kullanıcının profilini getirir.

**Endpoint**: `GET /Users/profile`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    // Kullanıcı detayları (Kullanıcı Detayı endpoint'i ile aynı format)
  },
  "message": "Profil başarıyla getirildi"
}
```

### Kullanıcı Oluştur

Yeni bir kullanıcı oluşturur.

**Endpoint**: `POST /Users`

**Yetki**: Admin

**İstek**:

```json
{
  "username": "yeni_kullanici",
  "email": "yeni@example.com",
  "password": "Guclu_Sifre123!",
  "roleId": 1
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "id": 5,
    "username": "yeni_kullanici",
    "email": "yeni@example.com",
    "role": {
      "id": 1,
      "name": "User"
    },
    "createdDate": "2025-03-30T02:15:10.1234567",
    "twoFactor": {
      "enabled": false,
      "required": false
    },
    "profilePicture": {
      "hasProfilePicture": true,
      "url": "/api/Users/5/profile-picture",
      "picture": "base64_encoded_image_data"
    }
  },
  "message": "Kullanıcı başarıyla oluşturuldu"
}
```

### Random Şifre ile Kullanıcı Oluşturma

Rastgele güçlü bir şifre ile yeni bir kullanıcı hesabı oluşturur ve şifre bilgisini kullanıcının e-posta adresine gönderir.

**Endpoint**: `POST /Users/random-password`

**Yetki**: Admin

**İstek**:

```json
{
  "username": "yeni_kullanici",
  "email": "yeni@example.com",
  "roleId": 1
}
```

**Yanıt** (201 Created):

```json
{
  "statusCode": 201,
  "isSuccess": true,
  "data": {
    "id": 5,
    "username": "yeni_kullanici",
    "email": "yeni@example.com",
    "role": {
      "id": 1,
      "name": "User"
    },
    "createdDate": "2025-03-30T02:15:10.1234567",
    "twoFactor": {
      "enabled": false,
      "required": false
    },
    "profilePicture": {
      "hasProfilePicture": false
    }
  },
  "message": "Kullanıcı otomatik şifre ile başarıyla oluşturuldu ve e-posta gönderildi"
}
```

### Kullanıcı Güncelle

Belirli bir kullanıcıyı günceller.

**Endpoint**: `PUT /Users/{id}`

**Yetki**: Admin

**İstek**:

```json
{
  "username": "guncel_kullanici",
  "email": "guncel@example.com",
  "roleId": 2
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 5,
    "username": "guncel_kullanici",
    "email": "guncel@example.com",
    "role": {
      "id": 2,
      "name": "Developer"
    },
    // ... diğer kullanıcı özellikleri
  },
  "message": "Kullanıcı başarıyla güncellendi"
}
```

### Kullanıcı Rolünü Güncelleme

Belirli bir kullanıcının yalnızca rol bilgisini günceller.

**Endpoint**: `PATCH /Users/{id}/role`

**Yetki**: Admin

**İstek**:

```json
{
  "roleId": 2
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 5,
    "username": "ornek_kullanici",
    "email": "ornek@example.com",
    "role": {
      "id": 2,
      "name": "Developer"
    },
    // ... diğer kullanıcı özellikleri
  },
  "message": "Kullanıcı rolü başarıyla güncellendi"
}
```

### Kullanıcı E-posta Adresini Güncelleme

Belirli bir kullanıcının yalnızca e-posta adresini günceller.

**Endpoint**: `PATCH /Users/{id}/email`

**Yetki**: Admin

**İstek**:

```json
{
  "email": "yeni_email@example.com"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "id": 5,
    "username": "ornek_kullanici",
    "email": "yeni_email@example.com",
    "role": {
      "id": 1,
      "name": "User"
    },
    // ... diğer kullanıcı özellikleri
  },
  "message": "Kullanıcı e-posta adresi başarıyla güncellendi"
}
```

### Profil Güncelle

Giriş yapmış kullanıcının profilini günceller.

**Endpoint**: `PUT /Users/profile`

**Yetki**: Kimliği doğrulanmış kullanıcı

**İstek**:

```json
{
  "username": "kullanici_yeni_ad",
  "email": "yeni_email@example.com"
}
```

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    // Güncellenmiş kullanıcı bilgileri
  },
  "message": "Profil başarıyla güncellendi"
}
```

### Kullanıcı Sil

Belirli bir kullanıcıyı siler.

**Endpoint**: `DELETE /Users/{id}`

**Yetki**: Admin

**Yanıt** (204 No Content):

```json
{
  "statusCode": 204,
  "isSuccess": true,
  "message": "Kullanıcı başarıyla silindi"
}
```

### Profil Fotoğrafı Yükle

Kullanıcı profil fotoğrafı yükler.

**Endpoint**: `POST /Users/profile-picture`

**Yetki**: Kimliği doğrulanmış kullanıcı

**Rate Limit**: 1 dakikada 10 istek

**İstek**: `multipart/form-data` formatında dosya

| Alan Adı | Tür | Açıklama |
|----------|-----|----------|
| ProfilePicture | File | Kare formatta (ör. 200x200), maksimum 1000x1000 piksel boyutunda resim dosyası |

**Yanıt** (200 OK):

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    // Kullanıcının güncel profil bilgileri
  },
  "message": "Profil fotoğrafı başarıyla yüklendi"
}
```

### Profil Fotoğrafı Görüntüle

Kullanıcının profil fotoğrafını resim formatında döndürür.

**Endpoint**: `GET /Users/{id}/profile-picture`

**Yetki**: Herkese açık

**Yanıt**: `image/png` formatında resim dosyası

## Hata Kodları

| HTTP Kodu | Açıklama | Olası Sebepler |
|-----------|---------|----------------|
| 400 | Bad Request | Geçersiz istek formatı, eksik veya hatalı parametreler |
| 401 | Unauthorized | Geçersiz veya süresi dolmuş token, 2FA doğrulama gerekli |
| 403 | Forbidden | Yetkisiz erişim girişimi, rol yetersizliği |
| 404 | Not Found | Kaynak bulunamadı (kullanıcı, profil fotoğrafı, vb.) |
| 409 | Conflict | Aynı kullanıcı adı veya e-posta zaten kullanımda |
| 422 | Unprocessable Entity | Doğrulama hatası (şifre kriterleri, e-posta formatı, vb.) |
| 429 | Too Many Requests | İstek limiti aşıldı, rate limiting |
| 500 | Internal Server Error | Sunucu taraflı hata |

---

Bu API referansı, Deneme API'nin tüm endpoint'lerini ve kullanım detaylarını içerir. İlave yardım veya örnekler için [Kullanım Örnekleri](./examples.md) bölümüne bakabilirsiniz. 