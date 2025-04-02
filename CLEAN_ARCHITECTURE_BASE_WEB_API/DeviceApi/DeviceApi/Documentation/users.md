# Kullanıcı Yönetimi

Bu bölüm, Deneme API'nin kullanıcı profili ve hesap yönetimi özelliklerini açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Kullanıcı Özellikleri](#kullanıcı-özellikleri)
- [Kullanıcı API Endpoints](#kullanıcı-api-endpoints)
  - [Kullanıcı Listesi](#kullanıcı-listesi)
  - [Kullanıcı Detayı](#kullanıcı-detayı)
  - [Kullanıcı Profili](#kullanıcı-profili)
  - [Kullanıcı Oluşturma](#kullanıcı-oluşturma)
  - [Random Şifre ile Kullanıcı Oluşturma](#random-şifre-ile-kullanıcı-oluşturma)
  - [Kullanıcı Güncelleme](#kullanıcı-güncelleme)
  - [Kullanıcı Rolünü Güncelleme](#kullanıcı-rolünü-güncelleme)
  - [Kullanıcı E-posta Adresini Güncelleme](#kullanıcı-e-posta-adresini-güncelleme)
  - [Profil Güncelleme](#profil-güncelleme)
  - [Kullanıcı Silme](#kullanıcı-silme)
  - [Profil Fotoğrafı Yönetimi](#profil-fotoğrafı-yönetimi)
- [Kullanıcı Rolleri](#kullanıcı-rolleri)
- [Yaygın Kullanım Senaryoları](#yaygın-kullanım-senaryoları)

## Genel Bakış

Deneme API, kapsamlı kullanıcı yönetimi özellikleri sunar. Bu özellikler, kullanıcı hesaplarını oluşturma, görüntüleme, güncelleme ve silme işlemlerini içerir. Ayrıca profil fotoğrafı yönetimi ve rol tabanlı erişim kontrolü de desteklenir.

## Kullanıcı Özellikleri

Her kullanıcı aşağıdaki temel özelliklere sahiptir:

| Özellik | Tür | Açıklama |
|---------|-----|----------|
| id | number | Benzersiz kullanıcı tanımlayıcısı |
| username | string | Kullanıcı adı (benzersiz) |
| email | string | E-posta adresi (benzersiz) |
| role | object | Kullanıcının rolü ve yetkileri |
| createdDate | date | Hesabın oluşturulma tarihi |
| lastLoginDate | date | Son giriş tarihi |
| twoFactor | object | İki faktörlü kimlik doğrulama durumu |
| profilePicture | object | Profil fotoğrafı bilgileri |

## Kullanıcı API Endpoints

### Kullanıcı Listesi

Tüm kullanıcıları listeler. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `GET /api/Users`

**Yetki**: Admin

**Yanıt Örneği**:

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

**Endpoint**: `GET /api/Users/{id}`

**Yetki**: Admin, Developer veya kendi profilini görüntüleyen kullanıcı

**Yanıt Örneği**:

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

Giriş yapmış kullanıcının kendi profilini getirir.

**Endpoint**: `GET /api/Users/profile`

**Yetki**: Kimliği doğrulanmış herhangi bir kullanıcı

**Yanıt Örneği**:

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

### Kullanıcı Oluşturma

Yeni bir kullanıcı hesabı oluşturur. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `POST /api/Users`

**Yetki**: Admin

**İstek Örneği**:

```json
{
  "username": "yeni_kullanici",
  "email": "yeni@example.com",
  "password": "Guclu_Sifre123!",
  "roleId": 1
}
```

**Yanıt Örneği**:

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
  "message": "Kullanıcı başarıyla oluşturuldu"
}
```

### Random Şifre ile Kullanıcı Oluşturma

Rastgele güçlü bir şifre ile yeni bir kullanıcı hesabı oluşturur ve şifre bilgisini kullanıcının e-posta adresine gönderir. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `POST /api/Users/random-password`

**Yetki**: Admin

**İstek Örneği**:

```json
{
  "username": "yeni_kullanici",
  "email": "yeni@example.com",
  "roleId": 1
}
```

**Yanıt Örneği**:

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

**Özellikler**:

- Random şifre, aşağıdaki kriterlere göre otomatik oluşturulur:
  - 12 karakter uzunluğunda
  - En az bir büyük harf
  - En az bir küçük harf
  - En az bir rakam
  - En az bir özel karakter
- Oluşturulan şifre, kullanıcının e-posta adresine HTML formatında gönderilir
- Kullanıcı, giriş yaptıktan sonra şifresini değiştirebilir

### Kullanıcı Güncelleme

Belirli bir kullanıcının bilgilerini günceller. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `PUT /api/Users/{id}`

**Yetki**: Admin

**İstek Örneği**:

```json
{
  "username": "guncel_kullanici",
  "email": "guncel@example.com",
  "roleId": 2
}
```

**Yanıt Örneği**:

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

Belirli bir kullanıcının yalnızca rol bilgisini günceller. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `PATCH /api/Users/{id}/role`

**Yetki**: Admin

**İstek Örneği**:

```json
{
  "roleId": 2
}
```

**Yanıt Örneği**:

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
    "createdDate": "2025-03-30T02:15:10.1234567",
    "lastLoginDate": "2025-03-30T02:16:20.1234567",
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
  "message": "Kullanıcı rolü başarıyla güncellendi"
}
```

### Kullanıcı E-posta Adresini Güncelleme

Belirli bir kullanıcının yalnızca e-posta adresini günceller. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `PATCH /api/Users/{id}/email`

**Yetki**: Admin

**İstek Örneği**:

```json
{
  "email": "yeni_email@example.com"
}
```

**Yanıt Örneği**:

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
    "createdDate": "2025-03-30T02:15:10.1234567",
    "lastLoginDate": "2025-03-30T02:16:20.1234567",
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
  "message": "Kullanıcı e-posta adresi başarıyla güncellendi"
}
```

**Doğrulama**:

- Yeni e-posta adresi, sistemdeki diğer kullanıcılar tarafından kullanılmıyor olmalıdır
- E-posta adresi geçerli bir formatta olmalıdır

### Profil Güncelleme

Giriş yapmış kullanıcının kendi profilini güncellemesine olanak tanır.

**Endpoint**: `PUT /api/Users/profile`

**Yetki**: Kimliği doğrulanmış herhangi bir kullanıcı

**İstek Örneği**:

```json
{
  "username": "kullanici_yeni_ad",
  "email": "yeni_email@example.com"
}
```

**Yanıt Örneği**:

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

### Kullanıcı Silme

Belirli bir kullanıcıyı siler. Bu endpoint yalnızca Admin rolündeki kullanıcılar tarafından erişilebilir.

**Endpoint**: `DELETE /api/Users/{id}`

**Yetki**: Admin

**Yanıt Örneği**:

```json
{
  "statusCode": 204,
  "isSuccess": true,
  "message": "Kullanıcı başarıyla silindi"
}
```

### Profil Fotoğrafı Yönetimi

#### Profil Fotoğrafı Yükleme

Kullanıcının profil fotoğrafını yükler veya günceller.

**Endpoint**: `POST /api/Users/profile-picture`

**Yetki**: Kimliği doğrulanmış herhangi bir kullanıcı

**İstek**: `multipart/form-data` formatında dosya

| Alan Adı | Tür | Açıklama |
|----------|-----|----------|
| ProfilePicture | File | Kare formatta (ör. 200x200), maksimum 1000x1000 piksel boyutunda resim dosyası |

**Profil Fotoğrafı Gereksinimleri**:

- Maksimum dosya boyutu: 1MB
- İzin verilen formatlar: JPG, JPEG, PNG
- Önerilen çözünürlük: 200x200 piksel
- Yüklenen resimler otomatik olarak yeniden boyutlandırılıp optimize edilir

**Yanıt Örneği**:

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

#### Profil Fotoğrafı Görüntüleme

Belirli bir kullanıcının profil fotoğrafını görüntüler.

**Endpoint**: `GET /api/Users/{id}/profile-picture`

**Yetki**: Herkese açık

**Yanıt**: İstemcinin beklediği content-type başlığı ile resim verileri (genellikle `image/png` veya `image/jpeg`).

Profil fotoğrafı mevcut değilse, varsayılan bir avatar görüntüsü döndürülür.

## Kullanıcı Rolleri

Sistem, aşağıdaki kullanıcı rollerini destekler:

| Rol ID | Rol Adı | Yetkiler |
|--------|---------|---------|
| 1 | User | Temel kullanıcı işlemleri, kendi profilini yönetme |
| 2 | Developer | User yetkilerine ek olarak API geliştirici özellikleri |
| 3 | Admin | Tam sistem yönetimi, tüm kullanıcıları ve rolleri yönetme |

## Yaygın Kullanım Senaryoları

### Admin Paneli için Kullanıcı Listesi

Admin kullanıcılar, tüm kullanıcıları görüntüleyebilir ve yönetebilir:

1. `GET /api/Users` endpoint'i ile tüm kullanıcıları listeleyin
2. Her kullanıcının detaylarını görüntülemek için `GET /api/Users/{id}` endpoint'ini kullanın
3. Kullanıcıları güncellemek veya silmek için ilgili endpoint'leri çağırın

### Kullanıcı Profili Yönetimi

Kullanıcılar kendi profillerini şu şekilde yönetebilir:

1. `GET /api/Users/profile` endpoint'i ile mevcut profil bilgilerini alın
2. `PUT /api/Users/profile` endpoint'i ile profil bilgilerini güncelleyin
3. `POST /api/Users/profile-picture` endpoint'i ile profil fotoğrafını yükleyin

### Yeni Kullanıcı Oluşturma (Admin)

Admin kullanıcılar, sistem yönetimi için yeni kullanıcılar oluşturabilir:

1. `POST /api/Users` endpoint'ine kullanıcı bilgilerini gönderin
2. Oluşturulan kullanıcıya uygun rolü atayın
3. Gerekirse, oluşturulan kullanıcı için 2FA gereksinimleri ayarlayın

### Random Şifre ile Kullanıcı Oluşturma (Admin)

Admin kullanıcılar, otomatik şifre atanan yeni kullanıcılar oluşturabilir:

1. `POST /api/Users/random-password` endpoint'ine kullanıcı bilgilerini gönderin
2. Sistem otomatik olarak güçlü bir şifre oluşturur ve kullanıcıya e-posta ile gönderir
3. Kullanıcı, e-postadaki şifre ile giriş yapabilir ve daha sonra şifresini değiştirebilir

Bu yöntem, özellikle çok sayıda kullanıcı eklerken veya güvenli başlangıç şifresi oluşturmak istediğinizde kullanışlıdır.

---

Kullanıcı yönetimi hakkında daha fazla bilgi için, [API Referansı](./api-reference.md) bölümüne bakabilirsiniz. İki faktörlü kimlik doğrulama ile ilgili detaylar için [İki Faktörlü Kimlik Doğrulama](./two-factor-auth.md) bölümünü inceleyebilirsiniz. 