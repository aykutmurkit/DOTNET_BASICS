# Frontend için Backend Gereksinimleri / Backend Requirements for Frontend

## Türkçe / Turkish

### API Endpoint Gereksinimleri

#### Kimlik Doğrulama Endpointleri
- `POST /api/Auth/login`: Kullanıcı giriş işlemi
- `POST /api/Auth/verify-2fa`: İki faktörlü doğrulama
- `POST /api/Auth/register`: Kullanıcı kaydı
- `POST /api/Auth/refresh-token`: Token yenileme
- `POST /api/Auth/forgot-password`: Şifre sıfırlama talebi
- `POST /api/Auth/reset-password`: Şifre sıfırlama
- `POST /api/Auth/change-password`: Şifre değiştirme
- `GET /api/Auth/2fa-status`: 2FA durumu sorgulama
- `POST /api/Auth/setup-2fa`: 2FA ayarları güncelleme

#### Kullanıcı Endpointleri
- `GET /api/Users/profile`: Kullanıcı profil bilgilerini getirme
- `PUT /api/Users/profile`: Kullanıcı profil bilgilerini güncelleme
- `POST /api/Users/profile-picture`: Profil resmi yükleme

### API Yanıt Formatı

Tüm API yanıtları aşağıdaki standart yapıda olmalı:

```json
{
  "success": true/false,
  "message": "İşlem mesajı",
  "data": {}, // İşleme özgü veri
  "errors": {}, // Validasyon hataları (opsiyonel)
  "statusCode": 200 // HTTP durum kodu
}
```

### Kimlik Doğrulama Gereksinimleri

- JWT tabanlı token sistemi
- Access token: 11 saat (660 dakika) geçerlilik
- Refresh token: 7 gün geçerlilik
- Tüm authorized endpointler için `Authorization: Bearer {token}` header desteği
- İki faktörlü kimlik doğrulama (2FA) desteği (E-posta tabanlı)

### Hata İşleme

- Her hata durumu için uygun HTTP durum kodu (400, 401, 403, 404, 409, 429, 500)
- Validasyon hataları için detaylı hata açıklamaları
- Kullanıcı dostu hata mesajları

### Rate Limiting

- Kritik endpointler için rate limiting desteği:
  - `/api/Auth/login`: 5 dakikada 10 istek
  - `/api/Auth/register`: 10 dakikada 3 istek
  - `/api/Auth/forgot-password`: 30 dakikada 3 istek
  - `/api/Auth/verify-2fa`: 5 dakikada 5 istek
  - `/api/Users/profile-picture`: 1 dakikada 10 istek

### Dış Servis Entegrasyonları

- E-posta gönderimi için güvenilir bir servis
- Profil resmi depolama için CDN/Blob desteği

### Güvenlik Gereksinimleri

- HTTPS zorunluluğu
- Şifre politikası (minimum 8 karakter, en az 1 büyük harf, 1 küçük harf, 1 rakam, 1 özel karakter)
- Şifrelerin bcrypt veya benzer algoritmalarla hashli saklanması
- Refresh token rotasyonu (her kullanımda yenilenmesi)
- CORS yapılandırması
- API isteklerinde uygun güvenlik başlıkları

### Performans Beklentileri

- API yanıt süresi: 300ms'den az (95. yüzdelik)
- Yüksek trafik durumlarında ölçeklenebilirlik (1000+ eşzamanlı kullanıcı)
- Uzun süren işlemler için asenkron işleme

### Dokümantasyon

- Swagger/OpenAPI entegrasyonu
- Tüm endpoint, istek ve yanıt parametrelerinin dokümantasyonu
- API değişiklik geçmişi ve sürüm bilgisi

### Test Veri Ortamı

- Geliştirme/test için seed veri
- Çeşitli kullanıcı senaryoları için test hesapları 

---

## English / İngilizce

### API Endpoint Requirements

#### Authentication Endpoints
- `POST /api/Auth/login`: User login process
- `POST /api/Auth/verify-2fa`: Two-factor verification
- `POST /api/Auth/register`: User registration
- `POST /api/Auth/refresh-token`: Token refresh
- `POST /api/Auth/forgot-password`: Password reset request
- `POST /api/Auth/reset-password`: Password reset
- `POST /api/Auth/change-password`: Password change
- `GET /api/Auth/2fa-status`: Query 2FA status
- `POST /api/Auth/setup-2fa`: Update 2FA settings

#### User Endpoints
- `GET /api/Users/profile`: Retrieve user profile information
- `PUT /api/Users/profile`: Update user profile information
- `POST /api/Users/profile-picture`: Upload profile picture

### API Response Format

All API responses should follow this standard structure:

```json
{
  "success": true/false,
  "message": "Operation message",
  "data": {}, // Operation-specific data
  "errors": {}, // Validation errors (optional)
  "statusCode": 200 // HTTP status code
}
```

### Authentication Requirements

- JWT-based token system
- Access token: 11 hours (660 minutes) validity
- Refresh token: 7 days validity
- `Authorization: Bearer {token}` header support for all authorized endpoints
- Two-factor authentication (2FA) support (Email-based)

### Error Handling

- Appropriate HTTP status code for each error condition (400, 401, 403, 404, 409, 429, 500)
- Detailed error descriptions for validation errors
- User-friendly error messages

### Rate Limiting

- Rate limiting support for critical endpoints:
  - `/api/Auth/login`: 10 requests per 5 minutes
  - `/api/Auth/register`: 3 requests per 10 minutes
  - `/api/Auth/forgot-password`: 3 requests per 30 minutes
  - `/api/Auth/verify-2fa`: 5 requests per 5 minutes
  - `/api/Users/profile-picture`: 10 requests per minute

### External Service Integrations

- Reliable service for email delivery
- CDN/Blob support for profile picture storage

### Security Requirements

- HTTPS enforcement
- Password policy (minimum 8 characters, at least 1 uppercase letter, 1 lowercase letter, 1 number, 1 special character)
- Passwords stored with bcrypt or similar hashing algorithms
- Refresh token rotation (renewal after each use)
- CORS configuration
- Appropriate security headers in API requests

### Performance Expectations

- API response time: less than 300ms (95th percentile)
- Scalability in high traffic situations (1000+ concurrent users)
- Asynchronous processing for long-running operations

### Documentation

- Swagger/OpenAPI integration
- Documentation of all endpoints, request and response parameters
- API change history and version information

### Test Data Environment

- Seed data for development/testing
- Test accounts for various user scenarios 