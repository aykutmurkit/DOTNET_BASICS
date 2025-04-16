# 05 - API Endpoints

Bu bölüm, AuthApi'nin sunduğu API endpoint'lerini detaylı olarak açıklamaktadır.

## Authentication Endpoints

Kimlik doğrulama ve yetkilendirme ile ilgili endpoint'ler `/api/Auth` rotası altında tanımlanmıştır.

### Kullanıcı Girişi

Kullanıcı adı ve şifre ile giriş yapmak için kullanılır.

**Endpoint:** `POST /api/Auth/login`

**Örnek İstek:**
```json
{
  "username": "johndoe",
  "password": "password123"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Giriş başarılı",
  "statusCode": 200,
  "data": {
    "user": {
      "id": 42,
      "username": "johndoe",
      "email": "john.doe@example.com",
      "fullName": "John Doe",
      "profilePictureUrl": "/api/Users/42/profile-picture",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-04-17T12:30:45Z"
    },
    "refreshToken": {
      "token": "cw1EjzZVrqL3fJ6ZnYmtP7...",
      "expiresAt": "2025-04-24T12:30:45Z"
    }
  }
}
```

**2FA Gerektiğinde Yanıt:**
```json
{
  "success": true,
  "message": "İki faktörlü kimlik doğrulama gerekli",
  "statusCode": 200,
  "data": {
    "userId": 42,
    "email": "j******@example.com",
    "username": "johndoe",
    "requiresTwoFactor": true,
    "message": "İki faktörlü kimlik doğrulama gerekli. E-posta adresinize gönderilen kodu giriniz."
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Kullanıcı adı veya şifre hatalı",
  "statusCode": 401,
  "errors": null
}
```

### İki Faktörlü Kimlik Doğrulama

2FA gerektiğinde, kullanıcının e-postasına gönderilen kodu doğrulamak için kullanılır.

**Endpoint:** `POST /api/Auth/verify-2fa`

**Örnek İstek:**
```json
{
  "userId": 42,
  "code": "123456"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "İki faktörlü kimlik doğrulama başarılı",
  "statusCode": 200,
  "data": {
    "user": {
      "id": 42,
      "username": "johndoe",
      "email": "john.doe@example.com",
      "fullName": "John Doe",
      "profilePictureUrl": "/api/Users/42/profile-picture",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-04-17T12:30:45Z"
    },
    "refreshToken": {
      "token": "cw1EjzZVrqL3fJ6ZnYmtP7...",
      "expiresAt": "2025-04-24T12:30:45Z"
    }
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Doğrulama kodu geçersiz veya süresi dolmuş",
  "statusCode": 400,
  "errors": null
}
```

### Kullanıcı Kaydı

Yeni bir kullanıcı hesabı oluşturmak için kullanılır.

**Endpoint:** `POST /api/Auth/register`

**Örnek İstek:**
```json
{
  "username": "janedoe",
  "email": "jane.doe@example.com",
  "password": "securePassword123",
  "confirmPassword": "securePassword123",
  "fullName": "Jane Doe"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla kaydedildi",
  "statusCode": 201,
  "data": {
    "user": {
      "id": 43,
      "username": "janedoe",
      "email": "jane.doe@example.com",
      "fullName": "Jane Doe",
      "profilePictureUrl": null,
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-04-17T12:30:45Z"
    },
    "refreshToken": {
      "token": "cw1EjzZVrqL3fJ6ZnYmtP7...",
      "expiresAt": "2025-04-24T12:30:45Z"
    }
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Bu kullanıcı adı veya e-posta adresi zaten kullanılıyor",
  "statusCode": 409,
  "errors": null
}
```

### Token Yenileme

Access token'ın süresi dolduğunda, refresh token kullanarak yeni token çifti almak için kullanılır.

**Endpoint:** `POST /api/Auth/refresh-token`

**Örnek İstek:**
```json
{
  "refreshToken": "cw1EjzZVrqL3fJ6ZnYmtP7..."
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Token başarıyla yenilendi",
  "statusCode": 200,
  "data": {
    "user": {
      "id": 42,
      "username": "johndoe",
      "email": "john.doe@example.com",
      "fullName": "John Doe",
      "profilePictureUrl": "/api/Users/42/profile-picture",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "expiresAt": "2025-04-18T12:30:45Z"
    },
    "refreshToken": {
      "token": "xyz123EjzZVrqL3fJ6ZnYmtP7...",
      "expiresAt": "2025-04-25T12:30:45Z"
    }
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Geçersiz veya süresi dolmuş refresh token",
  "statusCode": 401,
  "errors": null
}
```

### 2FA Durumu Sorgulama

Kullanıcının 2FA durumunu öğrenmek için kullanılır.

**Endpoint:** `GET /api/Auth/2fa-status`

**Kimlik Doğrulama:** Bearer Token

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "2FA durumu",
  "statusCode": 200,
  "data": {
    "enabled": true,
    "isGloballyRequired": false,
    "message": "İki faktörlü kimlik doğrulama hesabınız için etkinleştirilmiştir."
  }
}
```

### 2FA Ayarlarını Güncelleme

2FA özelliğini etkinleştirmek veya devre dışı bırakmak için kullanılır.

**Endpoint:** `POST /api/Auth/setup-2fa`

**Kimlik Doğrulama:** Bearer Token

**Örnek İstek:**
```json
{
  "enabled": true,
  "currentPassword": "securePassword123"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "2FA ayarları güncellendi",
  "statusCode": 200,
  "data": {
    "enabled": true,
    "isGloballyRequired": false,
    "message": "İki faktörlü kimlik doğrulama başarıyla etkinleştirildi."
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Mevcut şifre hatalı",
  "statusCode": 400,
  "errors": null
}
```

### Şifre Değiştirme

Kullanıcı şifresini değiştirmek için kullanılır.

**Endpoint:** `POST /api/Auth/change-password`

**Kimlik Doğrulama:** Bearer Token

**Örnek İstek:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newSecurePassword123",
  "confirmNewPassword": "newSecurePassword123"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Şifre başarıyla değiştirildi",
  "statusCode": 200,
  "data": null
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Mevcut şifre hatalı",
  "statusCode": 400,
  "errors": null
}
```

### Şifremi Unuttum

Şifre sıfırlama işlemi başlatmak için kullanılır.

**Endpoint:** `POST /api/Auth/forgot-password`

**Örnek İstek:**
```json
{
  "email": "john.doe@example.com"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Şifre sıfırlama talimatları e-posta adresinize gönderildi",
  "statusCode": 200,
  "data": null
}
```

### Şifre Sıfırlama

E-postaya gönderilen kod ile şifre sıfırlama işlemini tamamlamak için kullanılır.

**Endpoint:** `POST /api/Auth/reset-password`

**Örnek İstek:**
```json
{
  "email": "john.doe@example.com",
  "resetCode": "ABC123XYZ",
  "newPassword": "newSecurePassword123",
  "confirmNewPassword": "newSecurePassword123"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Şifreniz başarıyla sıfırlandı",
  "statusCode": 200,
  "data": null
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "Geçersiz veya süresi dolmuş sıfırlama kodu",
  "statusCode": 400,
  "errors": null
}
```

## User Endpoints

Kullanıcı yönetimi ile ilgili endpoint'ler `/api/Users` rotası altında tanımlanmıştır.

### Tüm Kullanıcıları Listeleme

Sistemdeki tüm kullanıcıları listelemek için kullanılır (sadece Admin rolü).

**Endpoint:** `GET /api/Users`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcılar başarıyla getirildi",
  "statusCode": 200,
  "data": [
    {
      "id": 1,
      "username": "admin",
      "email": "admin@example.com",
      "fullName": "System Admin",
      "profilePictureUrl": "/api/Users/1/profile-picture",
      "roles": ["Admin"]
    },
    {
      "id": 42,
      "username": "johndoe",
      "email": "john.doe@example.com",
      "fullName": "John Doe",
      "profilePictureUrl": "/api/Users/42/profile-picture",
      "roles": ["User"]
    }
  ]
}
```

### Kullanıcı Detayları

Belirli bir kullanıcının detaylarını görmek için kullanılır.

**Endpoint:** `GET /api/Users/{id}`

**Kimlik Doğrulama:** Bearer Token (Admin veya Developer rolü, ya da kendi profili)

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla getirildi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["User"],
    "twoFactorEnabled": true,
    "lastLogin": "2025-04-16T10:30:45Z",
    "createdAt": "2025-01-15T08:20:30Z"
  }
}
```

**Hata Yanıtı:**
```json
{
  "success": false,
  "message": "ID: 999 olan kullanıcı bulunamadı",
  "statusCode": 404,
  "errors": null
}
```

### Profil Bilgileri

Giriş yapmış kullanıcının kendi profil bilgilerini görmek için kullanılır.

**Endpoint:** `GET /api/Users/profile`

**Kimlik Doğrulama:** Bearer Token

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Profil başarıyla getirildi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["User"],
    "twoFactorEnabled": true,
    "lastLogin": "2025-04-16T10:30:45Z",
    "createdAt": "2025-01-15T08:20:30Z"
  }
}
```

### Kullanıcı Oluşturma

Yeni bir kullanıcı oluşturmak için Admin rolü gereklidir.

**Endpoint:** `POST /api/Users`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Örnek İstek:**
```json
{
  "username": "newuser",
  "email": "new.user@example.com",
  "password": "securePassword123",
  "fullName": "New User",
  "roleId": 3
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla oluşturuldu",
  "statusCode": 201,
  "data": {
    "id": 44,
    "username": "newuser",
    "email": "new.user@example.com",
    "fullName": "New User",
    "profilePictureUrl": null,
    "roles": ["User"]
  }
}
```

### Rastgele Şifre ile Kullanıcı Oluşturma

Sistem tarafından oluşturulan rastgele şifre ile kullanıcı oluşturmak için kullanılır.

**Endpoint:** `POST /api/Users/random-password`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Örnek İstek:**
```json
{
  "username": "randomuser",
  "email": "random.user@example.com",
  "fullName": "Random User",
  "roleId": 3,
  "sendPasswordByEmail": true
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı rastgele şifre ile oluşturuldu ve şifre e-posta ile gönderildi",
  "statusCode": 201,
  "data": {
    "id": 45,
    "username": "randomuser",
    "email": "random.user@example.com",
    "fullName": "Random User",
    "profilePictureUrl": null,
    "roles": ["User"],
    "password": "X7tQ!p3R*mL9"
  }
}
```

### Kullanıcı Güncelleme

Kullanıcı bilgilerini güncellemek için kullanılır.

**Endpoint:** `PUT /api/Users/{id}`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Örnek İstek:**
```json
{
  "fullName": "Johnny Doe Updated",
  "email": "john.updated@example.com"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla güncellendi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.updated@example.com",
    "fullName": "Johnny Doe Updated",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["User"]
  }
}
```

### Profil Güncelleme

Kullanıcının kendi profilini güncellemesi için kullanılır.

**Endpoint:** `PUT /api/Users/profile`

**Kimlik Doğrulama:** Bearer Token

**Örnek İstek:**
```json
{
  "fullName": "John Doe Jr."
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Profil başarıyla güncellendi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.doe@example.com",
    "fullName": "John Doe Jr.",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["User"]
  }
}
```

### Kullanıcı Silme

Kullanıcıyı sistemden silmek için kullanılır.

**Endpoint:** `DELETE /api/Users/{id}`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla silindi",
  "statusCode": 204,
  "data": null
}
```

### Profil Resmi Yükleme

Kullanıcı profil resmini yüklemek için kullanılır.

**Endpoint:** `POST /api/Users/profile-picture`

**Kimlik Doğrulama:** Bearer Token

**İstek Tipi:** `multipart/form-data`

**Parametre:** `file` (resim dosyası)

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Profil resmi başarıyla yüklendi",
  "statusCode": 200,
  "data": {
    "profilePictureUrl": "/api/Users/42/profile-picture"
  }
}
```

### Profil Resmi Görüntüleme

Kullanıcı profil resmini görüntülemek için kullanılır.

**Endpoint:** `GET /api/Users/{id}/profile-picture`

**Kimlik Doğrulama:** Gerekli değil

**Yanıt:** Resim dosyası (Content-Type: image/jpeg, image/png, vb.)

### Kullanıcı Rolünü Güncelleme

Kullanıcının rolünü değiştirmek için kullanılır.

**Endpoint:** `PATCH /api/Users/{id}/role`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Örnek İstek:**
```json
{
  "roleId": 2
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı rolü başarıyla güncellendi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["Developer"]
  }
}
```

### Kullanıcı E-postasını Güncelleme

Kullanıcının e-posta adresini değiştirmek için kullanılır.

**Endpoint:** `PATCH /api/Users/{id}/email`

**Kimlik Doğrulama:** Bearer Token (Admin rolü gerekli)

**Örnek İstek:**
```json
{
  "email": "john.new.email@example.com"
}
```

**Başarılı Yanıt:**
```json
{
  "success": true,
  "message": "Kullanıcı e-postası başarıyla güncellendi",
  "statusCode": 200,
  "data": {
    "id": 42,
    "username": "johndoe",
    "email": "john.new.email@example.com",
    "fullName": "John Doe",
    "profilePictureUrl": "/api/Users/42/profile-picture",
    "roles": ["User"]
  }
}
```

## API Yanıt Yapısı

Tüm API yanıtları standart bir formatta döndürülür:

```json
{
  "success": true|false,  // İşlemin başarılı olup olmadığını belirtir
  "message": "string",    // Kullanıcı dostu mesaj
  "statusCode": number,   // HTTP durum kodu
  "data": any,            // Başarılı yanıtlar için veri
  "errors": {             // Hata durumunda validasyon hataları
    "propertyName": ["hata mesajı"]
  }
}
```

### Durum Kodları

- **200 OK**: İstek başarılı
- **201 Created**: Yeni kaynak başarıyla oluşturuldu
- **204 No Content**: İstek başarılı, ancak döndürülecek veri yok
- **400 Bad Request**: Geçersiz istek
- **401 Unauthorized**: Kimlik doğrulama gerekli
- **403 Forbidden**: Yetkisiz erişim
- **404 Not Found**: Kaynak bulunamadı
- **409 Conflict**: Çakışma (örneğin, aynı kullanıcı adı)
- **429 Too Many Requests**: Rate limit aşıldı
- **500 Internal Server Error**: Sunucu hatası 