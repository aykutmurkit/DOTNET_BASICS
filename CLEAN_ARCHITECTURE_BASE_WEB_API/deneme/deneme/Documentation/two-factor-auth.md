# İki Faktörlü Kimlik Doğrulama (2FA)

Bu bölüm, Deneme API'nin iki faktörlü kimlik doğrulama (2FA) özelliklerini ve kullanımını açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Nasıl Çalışır](#nasıl-çalışır)
- [API Endpoints](#api-endpoints)
- [2FA Durumları](#2fa-durumları)
- [Zorunlu 2FA Politikası](#zorunlu-2fa-politikası)
- [Geliştirici Notları](#geliştirici-notları)
- [Sık Sorulan Sorular](#sık-sorulan-sorular)

## Genel Bakış

İki faktörlü kimlik doğrulama (2FA), kullanıcı hesaplarına ek bir güvenlik katmanı ekleyen bir özelliktir. Bu sistem, kullanıcı giriş işlemi sırasında kullanıcı adı ve şifrenin yanı sıra ikinci bir doğrulama faktörü gerektirir. Deneme API'de bu ikinci faktör, kullanıcının e-posta adresine gönderilen bir doğrulama kodudur.

## Nasıl Çalışır

2FA etkinleştirildiğinde, kullanıcı girişi şu adımlarla gerçekleşir:

1. Kullanıcı, kullanıcı adı ve şifre ile giriş yapar
2. Sistem, bu bilgileri doğrular ve 2FA etkinse veya zorunluysa, bir doğrulama kodu gönderir
3. Kullanıcının e-posta adresine 6 haneli sayısal bir kod gönderilir
4. Kullanıcı, bu kodu giriş ekranına girer
5. Kod doğruysa, kullanıcı başarıyla giriş yapar ve access token alır

Bu süreç, kullanıcı hesabını yetkisiz erişimlere karşı korumaya yardımcı olur. Saldırgan, kullanıcının şifresini ele geçirse bile, e-posta adresine erişimi olmadığı sürece hesaba giremez.

## API Endpoints

### 2FA Durumunu Kontrol Et

```
GET /api/Auth/2fa-status
```

**Yetki**: Kimliği doğrulanmış kullanıcı

**Yanıt**:
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

### 2FA Ayarlarını Güncelle

```
POST /api/Auth/setup-2fa
```

**Yetki**: Kimliği doğrulanmış kullanıcı

**İstek gövdesi**:
```json
{
  "enabled": true,
  "currentPassword": "Kullanici_Sifresi123!"
}
```

**Yanıt**:
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

### 2FA Kodu ile Giriş Tamamlama

```
POST /api/Auth/verify-2fa
```

**İstek gövdesi**:
```json
{
  "userId": 5,
  "code": "123456"
}
```

**Yanıt**:
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

## 2FA Durumları

İki faktörlü kimlik doğrulama, üç temel durumda olabilir:

1. **Devre Dışı**: 2FA sistemi devre dışı, kullanıcı normal giriş yapabilir
2. **Kullanıcı Tarafından Etkinleştirilmiş**: Kullanıcı kendi hesabı için 2FA'yı etkinleştirmiş
3. **Sistem Tarafından Zorunlu Kılınmış**: Sistemde global olarak 2FA zorunlu hale getirilmiş

2FA durumu, kullanıcının profil bilgilerinde şu şekilde görünür:

```json
"twoFactor": {
  "enabled": true,    // Kullanıcının 2FA'yı etkinleştirip etkinleştirmediği
  "required": false   // Sistemin 2FA'yı zorunlu kılıp kılmadığı
}
```

## Zorunlu 2FA Politikası

Sistem yöneticileri, tüm kullanıcılar için 2FA'yı zorunlu hale getirebilir. Bu durumda:

1. Kullanıcılar, 2FA'yı devre dışı bırakamaz
2. Tüm giriş işlemleri iki aşamalı olur
3. Yeni kullanıcılar, ilk girişlerinde 2FA kurulumunu tamamlamak zorundadır

Zorunlu 2FA politikası, `appsettings.json` dosyasındaki şu ayarlar ile kontrol edilir:

```json
"TwoFactorSettings": {
  "SystemEnabled": true,       // 2FA sisteminin genel olarak etkin olup olmadığı
  "RequiredForAllUsers": true, // 2FA'nın tüm kullanıcılar için zorunlu olup olmadığı
  "CodeLength": 6,             // Doğrulama kodu uzunluğu
  "ExpirationMinutes": 10      // Kodun geçerlilik süresi (dakika)
}
```

## Geliştirici Notları

### 2FA Akışı Entegrasyonu

Frontend uygulamalarının 2FA akışını entegre etmesi için önerilen adımlar:

1. Kullanıcı giriş formunda kullanıcı adı ve şifre alın
2. `/api/Auth/login` endpoint'ine bu bilgileri gönderin
3. Yanıtı kontrol edin:
   - Eğer `data.accessToken` varsa, normal giriş
   - Eğer `data.requiresTwoFactor` true ise, 2FA gerekiyor
4. 2FA gerekiyorsa, kullanıcıdan e-postasına gelen kodu isteyin
5. `/api/Auth/verify-2fa` endpoint'ine kod ve userId gönderin
6. Başarılı doğrulama sonrası access token alın ve oturumu başlatın

### Kod Doğrulama Detayları

- Kod uzunluğu: 6 hane
- Geçerlilik süresi: 10 dakika
- Format: Sayısal

### Güvenlik Önlemleri

- Arka arkaya maksimum 5 başarısız deneme yapılabilir
- Brute force saldırıları için rate limiting uygulanır
- Kod geçerlilik süresi dolduğunda yeni kod istenmesi gerekir

## Sık Sorulan Sorular

### Kullanıcı 2FA'yı etkinleştirdikten sonra devre dışı bırakabilir mi?

Evet, sistem zorunlu kılmadığı sürece kullanıcılar 2FA'yı devre dışı bırakabilir. Sistem zorunlu kıldığında ise devre dışı bırakılamaz.

### Kullanıcı e-posta adresini değiştirirse 2FA nasıl etkilenir?

E-posta değişikliği durumunda:
1. Kullanıcının mevcut şifresi doğrulanır
2. Yeni e-posta adresine doğrulama kodu gönderilir
3. Doğrulama başarılı olduğunda, 2FA ayarları yeni e-posta adresine aktarılır

### Kullanıcı doğrulama kodunu almazsa ne yapmalı?

1. Spam/gereksiz klasörlerini kontrol etmeli
2. Tekrar giriş denemeli
3. Yönetici ile iletişime geçmeli

---

İki faktörlü kimlik doğrulama, hesap güvenliğini artırmak için önerilen bir uygulamadır. Tüm kullanıcıların, kendi hesapları için bu özelliği etkinleştirmesi tavsiye edilir. 