# Kimlik Doğrulama

Bu bölüm, Deneme API'nin kimlik doğrulama ve yetkilendirme süreçlerini açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Kullanıcı Kaydı](#kullanıcı-kaydı)
- [Kullanıcı Girişi](#kullanıcı-girişi)
- [Token Yönetimi](#token-yönetimi)
- [Şifre Yönetimi](#şifre-yönetimi)
- [Güvenlik Önlemleri](#güvenlik-önlemleri)

## Genel Bakış

Deneme API, JWT (JSON Web Token) tabanlı kimlik doğrulama kullanır. Oturum yönetimi için hem access token hem de refresh token yaklaşımını benimser.

### Token Tipleri

1. **Access Token**: Kısa süreli, API kaynaklarına erişim için kullanılır.
2. **Refresh Token**: Uzun süreli, access token'in süresi dolduğunda yenilemek için kullanılır.

### Yetkilendirme Başlığı

Tüm kimlik doğrulama gerektiren istekler, aşağıdaki formatta bir yetkilendirme başlığı içermelidir:

```
Authorization: Bearer {access_token}
```

## Kullanıcı Kaydı

### Genel Bilgiler

- Kullanıcı kaydı, minimum gereksinimler: kullanıcı adı, e-posta ve şifre
- Şifre gereksinimleri: en az 8 karakter, en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter
- E-posta adresi ve kullanıcı adı benzersiz olmalı
- Kayıt sonrası, JWT ve kullanıcı bilgileri döndürülür

### Kayıt Endpoint'i

**Endpoint**: `POST /api/Auth/register`

**İstek Örneği**:

```json
{
  "username": "ornek_kullanici",
  "email": "ornek@mail.com",
  "password": "Guclu_Sifre123!",
  "confirmPassword": "Guclu_Sifre123!"
}
```

**Yanıt Örneği**:

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

## Kullanıcı Girişi

### Genel Bilgiler

- Giriş, kullanıcı adı ve şifre ile yapılır
- Başarılı kimlik doğrulaması, JWT ve kullanıcı bilgileriyle sonuçlanır
- İki faktörlü kimlik doğrulama (2FA) etkinse, bu adımdan sonra 2FA kodu girişi gerekir

### Giriş Endpoint'i

**Endpoint**: `POST /api/Auth/login`

**İstek Örneği**:

```json
{
  "username": "ornek_kullanici",
  "password": "Guclu_Sifre123!"
}
```

**Yanıt Örneği (Normal Giriş)**:

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

**Yanıt Örneği (2FA Gerekli)**:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "userId": 5,
    "email": "ornek@mail.com",
    "username": "ornek_kullanici",
    "requiresTwoFactor": true,
    "message": "İki faktörlü kimlik doğrulama gerekli."
  },
  "message": "İki faktörlü kimlik doğrulama gerekli"
}
```

## Token Yönetimi

### Access Token Yenileme

Access token süresi dolduğunda, refresh token kullanılarak yeni bir access token alınabilir.

**Endpoint**: `POST /api/Auth/refresh-token`

**İstek Örneği**:

```json
{
  "refreshToken": "RZgGVhCO7bqf6zVHRG3fJWMXWJUHd2T3mMdUjx2hHko="
}
```

**Yanıt Örneği**:

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

### Token Yaşam Süreleri

- **Access Token**: 15 dakika
- **Refresh Token**: 7 gün

## Şifre Yönetimi

### Şifre Sıfırlama

Kullanıcılar şifrelerini unuttuklarında, e-posta ile şifre sıfırlama bağlantısı alabilirler.

#### Şifre Sıfırlama İsteği

**Endpoint**: `POST /api/Auth/forgot-password`

**İstek Örneği**:

```json
{
  "email": "ornek@mail.com"
}
```

**Yanıt Örneği**:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi"
}
```

#### Şifre Sıfırlama İşlemi

**Endpoint**: `POST /api/Auth/reset-password`

**İstek Örneği**:

```json
{
  "token": "sifre-sifirlama-token",
  "email": "ornek@mail.com",
  "newPassword": "Yeni_Guclu_Sifre123!",
  "confirmNewPassword": "Yeni_Guclu_Sifre123!"
}
```

**Yanıt Örneği**:

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Şifreniz başarıyla değiştirildi"
}
```

## Güvenlik Önlemleri

Deneme API, aşağıdaki güvenlik önlemlerini içerir:

### Şifre Güvenliği

- Şifreler her zaman hash'lenir ve salt eklenerek saklanır
- Password derivation için PBKDF2 algoritması kullanılır
- Minimum şifre gereksinimleri: 8 karakter, karma karakter tipleri

### Oturum Güvenliği

- Access token'lar kısa süreli tutulur (15 dakika)
- Token'lar, IP adresi gibi ek doğrulama bilgileri içerir
- Refresh token geçersiz kılındığında tüm cihazlarda oturum sonlandırılabilir

### Rate Limiting

- Belirli bir IP adresinden gelen çok sayıda başarısız giriş denemesi, geçici olarak engellenecektir
- Şifre sıfırlama istekleri, spam önleme amacıyla sınırlandırılmıştır
- 2FA doğrulama kodu çok sayıda yanlış denendiğinde kilitlenir

---

Kimlik doğrulama hakkında daha fazla bilgi için [İki Faktörlü Kimlik Doğrulama](./two-factor-auth.md) bölümüne bakabilirsiniz. 