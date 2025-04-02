# JWT Tabanlı Kimlik Doğrulama

Bu dokümanda, DeviceApi'nin JWT (JSON Web Token) tabanlı kimlik doğrulama sistemi açıklanmaktadır.

## JWT Yapılandırması

JWT yapılandırması, `appsettings.json` dosyasında aşağıdaki şekilde tanımlanmıştır:

```json
"JwtSettings": {
  "Secret": "VerySecureSecretKey12345678901234567890",
  "Issuer": "DenemeApi",
  "Audience": "DenemeApiClient",
  "AccessTokenExpirationInMinutes": 60,
  "RefreshTokenExpirationInDays": 7
}
```

## JWT Token İçeriği

Sistemde oluşturulan JWT token'ları aşağıdaki bilgileri içerir:

```json
{
  "nameid": "4",         // Kullanıcı ID'si
  "unique_name": "aykut", // Kullanıcı adı
  "email": "aykutmurkit.dev@gmail.com", // E-posta adresi
  "role": "Admin",       // Kullanıcı rolü
  "nbf": 1743598183,     // Not Before (geçerlilik başlangıç zamanı)
  "exp": 1743601783,     // Expiration (son geçerlilik zamanı)
  "iat": 1743598183,     // Issued At (oluşturulma zamanı)
  "iss": "DenemeApi",    // Issuer (token'ı veren)
  "aud": "DenemeApiClient" // Audience (hedef kitle)
}
```

## Kimlik Doğrulama Servisi

Kimlik doğrulama işlemleri, `AuthService` sınıfı tarafından gerçekleştirilir. Bu servis aşağıdaki işlevleri sağlar:

1. **Access Token Oluşturma**: `GenerateAccessTokenAsync` metodu ile kullanıcı bilgilerine göre kısa süreli erişim tokeni oluşturulur.
2. **Refresh Token Oluşturma**: `GenerateRefreshTokenAsync` metodu ile access token yenilemek için kullanılan uzun süreli token oluşturulur.
3. **Token Doğrulama**: `GetPrincipalFromExpiredToken` metodu ile mevcut token'ın içeriği doğrulanır ve claims bilgileri çıkarılır.
4. **Kullanıcı Doğrulama**: `ValidateUserAsync` metodu ile kullanıcı adı ve şifre doğrulanır.
5. **Kullanıcı Bilgileri Getirme**: `GetUserInfoAsync` metodu ile kullanıcı detayları alınır.

## Auth Controller

Auth Controller, aşağıdaki endpoint'leri içerir:

1. **POST /api/Auth/login**: Kullanıcı giriş işlemi. Başarılı giriş sonrası access ve refresh token döner.
2. **POST /api/Auth/refresh-token**: Süresi dolmuş access token'ı yenilemek için kullanılır.
3. **GET /api/Auth/me**: Mevcut oturum açmış kullanıcının bilgilerini getirir.
4. **GET /api/Auth/admin-only**: Sadece Admin rolündeki kullanıcıların erişebileceği örnek endpoint.
5. **GET /api/Auth/role-based-content**: Kullanıcı rolüne göre farklı içerik sunan örnek endpoint.

## Rol Tabanlı Yetkilendirme

Rol tabanlı yetkilendirme, Controller veya Action seviyesinde `[Authorize(Roles = "Admin,Developer")]` şeklinde tanımlanabilir. 

Örnek olarak:
- Tüm cihaz işlemleri için en az kullanıcı girişi gereklidir: `[Authorize]`
- Cihaz oluşturma, güncelleme ve silme işlemleri için Admin veya Developer rolü gereklidir: `[Authorize(Roles = "Admin,Developer")]`

## Middleware ve Loglama

Sistem, JWT claim'lerini loglamak için özel bir middleware (`JwtClaimsLoggingMiddleware`) kullanır. Bu middleware, kimlik doğrulaması yapılmış her istek için kullanıcı bilgilerini ve erişilen endpoint'i loglar.

Ayrıca, her controller action'ında da kullanıcı ID'si ve rolü loglanır, böylece kullanıcıların sistem üzerindeki tüm aktiviteleri kayıt altına alınır.

## Swagger Entegrasyonu

Swagger, JWT kimlik doğrulaması ile entegre edilmiştir. API'yi test ederken "Authorize" butonu ile JWT token'ınızı girebilir ve yetkilendirmeli endpoint'leri test edebilirsiniz.

## Güvenlik Önlemleri

1. Token süreleri sınırlıdır (Access token: 60 dakika, Refresh token: 7 gün)
2. Her token için issuer ve audience doğrulaması yapılır
3. Admin işlemleri için özel rol gereksinimleri tanımlanmıştır
4. Token içeriği şifrelenir ve doğrulanır 