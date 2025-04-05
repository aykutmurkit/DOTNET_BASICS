# JWT Token Doğrulama Dokümantasyonu

Bu doküman, DeviceApi'de JWT (JSON Web Token) token doğrulama sistemini açıklar.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [JWT Token Yapısı](#jwt-token-yapısı)
- [Token Doğrulama Mekanizması](#token-doğrulama-mekanizması)
- [Yetkilendirme](#yetkilendirme)
- [Güvenlik Önlemleri](#güvenlik-önlemleri)

## Genel Bakış

DeviceApi, kullanıcı kimlik doğrulaması için JWT (JSON Web Token) standardını kullanır. API, harici bir kimlik sağlayıcı tarafından üretilen token'ları doğrular ve yetkilendirme kararlarını bu token'lar üzerindeki bilgilere göre verir.

API, authentication (kimlik doğrulama) ve authorization (yetkilendirme) işlemleri için ASP.NET Core'un yerleşik middleware'lerini kullanır.

## JWT Token Yapısı

JWT, üç bölümden oluşur:

1. **Header**: Token türü ve kullanılan algoritma bilgilerini içerir.
2. **Payload**: Token içindeki claims (iddialar) bilgilerini içerir.
3. **Signature**: Token'ın bütünlüğünü doğrulamak için kullanılan imza.

DeviceApi, token'ın payload bölümündeki aşağıdaki claims'leri kullanır:

```json
{
  "sub": "1",                                // Subject - Kullanıcı ID
  "name": "kullanici_adi",                   // Kullanıcı adı
  "email": "kullanici@mail.com",             // E-posta adresi 
  "role": "Admin",                           // Kullanıcı rolü
  "jti": "3f0bd76f-df9d-4885-9730-...",      // Benzersiz token ID
  "nbf": 1647691200,                         // Not Before - Token geçerlilik başlangıç tarihi
  "exp": 1647692100,                         // Expiration - Token son kullanma tarihi
  "iat": 1647691200,                         // Issued At - Token oluşturulma tarihi
  "iss": "TokenIssuer"                       // Issuer - Token'ı oluşturan sistemin adı
}
```

## Token Doğrulama Mekanizması

DeviceApi, JWT token doğrulaması için ASP.NET Core'un `JwtBearer` kimlik doğrulama mekanizmasını kullanır. Bu yapılandırma, `Program.cs` dosyasında şu şekilde tanımlanmıştır:

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["TokenSettings:Issuer"],
            ValidAudience = Configuration["TokenSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["TokenSettings:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });
```

Token doğrulama işlemi şu adımları içerir:

1. API'ye gelen her istek için, `Authorization` başlığında bir Bearer token olup olmadığı kontrol edilir.
2. Token varsa, imzası doğrulanır (token'ın değiştirilmediğinden emin olmak için).
3. Token'ın süresi kontrol edilir (süre dolmuşsa reddedilir).
4. Token'ın issuer ve audience değerleri kontrol edilir (güvenilen bir kaynak tarafından verildiğinden emin olmak için).
5. Token geçerliyse, içindeki claims (iddialar) HTTP context'ine yüklenir ve istek işlenir.

**Not:** Token'lar harici bir kimlik sağlayıcısından alınmalıdır. DeviceApi, token üretme veya kullanıcı kimlik doğrulama işlemlerini kendisi gerçekleştirmez.

## Yetkilendirme

Token doğrulandıktan sonra, API'nin farklı bölümlerine erişim yetkilendirmesi (authorization) yapılır. Bu, genellikle controller veya action seviyesinde `[Authorize]` özniteliği kullanılarak gerçekleştirilir.

Rol tabanlı yetkilendirme için:

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // Sadece Admin rolündeki kullanıcılar erişebilir
}

[Authorize]
public class DevicesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Kimliği doğrulanmış tüm kullanıcılar erişebilir
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Developer")]
    public async Task<IActionResult> Create()
    {
        // Sadece Admin veya Developer rolündeki kullanıcılar erişebilir
    }
}
```

Policy tabanlı yetkilendirme için önce politikaları tanımlayın:

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DeviceManagement", policy => 
        policy.RequireRole("Admin", "DeviceManager"));
});
```

Sonra bu politikaları kullanın:

```csharp
[Authorize(Policy = "DeviceManagement")]
public async Task<IActionResult> UpdateDevice(int id)
{
    // Sadece DeviceManagement politikasına uygun kullanıcılar erişebilir
}
```

## Güvenlik Önlemleri

DeviceAPI'nin token doğrulama sistemi, aşağıdaki güvenlik önlemlerini içerir:

1. **Token Süresi Kontrolü**: Süresi dolmuş token'lar otomatik olarak reddedilir.

2. **Sıfır Saat Farkı (Zero Clock Skew)**: Token süre sonu kontrolünde herhangi bir tolerans uygulanmaz.

3. **İmza Doğrulama**: Her token'ın imzası doğrulanarak, içeriğinin değiştirilmediğinden emin olunur.

4. **Issuer ve Audience Kontrolü**: Token'ların güvenilir bir kaynak tarafından verildiği ve bu API için oluşturulduğu doğrulanır.

5. **HTTPS Zorunluluğu**: Tüm API trafiği HTTPS üzerinden yönlendirilir, böylece token'lar ağ üzerinde güvenli bir şekilde taşınır.

6. **Cache-Control Başlıkları**: API yanıtları önbelleğe alınmaz, böylece hassas bilgiler veya token'lar önbellekte tutulmaz.

7. **Rol ve İzin Bazlı Yetkilendirme**: Token içindeki rol ve yetki iddiaları kullanılarak, API endpoint'lerine erişim kontrol edilir. 