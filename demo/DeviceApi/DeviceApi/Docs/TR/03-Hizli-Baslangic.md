# Hızlı Başlangıç

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu kılavuz, DeviceApi ile hızlıca başlamanıza yardımcı olacak, yaygın işlemleri nasıl gerçekleştireceğinizi ve API ile nasıl etkileşime geçeceğinizi gösterecektir.

## Önkoşullar

Başlamadan önce, aşağıdakilere sahip olduğunuzdan emin olun:

- [Kurulum](02-Kurulum.md) adımlarını tamamlamış olmalısınız
- Kod düzenlemek için bir metin düzenleyici veya IDE
- Postman, cURL veya benzeri bir REST istemcisi
- REST API'leri ve HTTP metotları hakkında temel bilgi

## API'yi Çalıştırma

1. API sunucusunu başlatın:

   ```bash
   cd DeviceApi
   dotnet run
   ```

2. Sunucu varsayılan olarak `https://localhost:5001` adresinde başlayacaktır

3. Bir tarayıcı açın ve kullanılabilir tüm uç noktalarla Swagger UI'ı görmek için `https://localhost:5001/swagger` adresine gidin

## Kimlik Doğrulama

Çoğu uç nokta kimlik doğrulama gerektirir. Bir kimlik doğrulama tokeni almakla başlayalım:

### Adım 1: Yeni Bir Kullanıcı Kaydetme

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "demo_user",
  "email": "demo@example.com",
  "password": "SecureP@ssw0rd123",
  "firstName": "Demo",
  "lastName": "User"
}
```

### Adım 2: JWT Token Almak için Giriş Yapma

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "demo@example.com",
  "password": "SecureP@ssw0rd123"
}
```

Yanıt, JWT tokeninizi içerecektir:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "expiresIn": 3600
}
```

### Adım 3: Tokeni Sonraki İsteklerde Kullanma

Tokeni, Bearer token olarak Authorization başlığına dahil edin:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Cihazlarla Çalışma

### Yeni Bir Cihaz Kaydetme

```http
POST /api/devices
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Sıcaklık Sensörü 1",
  "deviceType": "TEMPERATURE_SENSOR",
  "serialNumber": "TS-2025-001",
  "firmwareVersion": "1.0.0",
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "location": {
    "latitude": 41.0082,
    "longitude": 28.9784,
    "address": "İstanbul, Türkiye",
    "floor": 3,
    "room": "Sunucu Odası"
  },
  "properties": {
    "maxTemperature": 85,
    "minTemperature": -40,
    "accuracyLevel": "high"
  }
}
```

### Tüm Cihazları Getirme

```http
GET /api/devices?page=1&pageSize=10
Authorization: Bearer your-token-here
```

Yanıt:

```json
{
  "totalItems": 1,
  "items": [
    {
      "id": "d290f1ee-6c54-4b01-90e6-d701748f0851",
      "name": "Sıcaklık Sensörü 1",
      "deviceType": "TEMPERATURE_SENSOR",
      "serialNumber": "TS-2025-001",
      "status": "ACTIVE",
      "createdAt": "2025-04-05T08:15:30Z",
      "lastConnectedAt": "2025-04-05T08:15:30Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### Cihaz Detaylarını Getirme

```http
GET /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Authorization: Bearer your-token-here
```

### Cihaz Güncelleme

```http
PUT /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Sıcaklık Sensörü 1 - Güncellenmiş",
  "firmwareVersion": "1.0.1",
  "status": "MAINTENANCE",
  "properties": {
    "maxTemperature": 90,
    "minTemperature": -40,
    "accuracyLevel": "high"
  }
}
```

### Cihaz Silme

```http
DELETE /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Authorization: Bearer your-token-here
```

## Cihaz Verileriyle Çalışma

### Bir Cihazdan Veri Gönderme

```http
POST /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851/data
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "timestamp": "2025-04-05T10:15:30Z",
  "readings": {
    "temperature": 24.5,
    "humidity": 45.2,
    "batteryLevel": 78
  },
  "alerts": ["LOW_BATTERY"]
}
```

### Cihaz Veri Geçmişini Getirme

```http
GET /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851/data?startDate=2025-04-01T00:00:00Z&endDate=2025-04-05T23:59:59Z&page=1&pageSize=100
Authorization: Bearer your-token-here
```

## Filtreleme, Sıralama ve Sayfalandırma Kullanımı

API, koleksiyon uç noktaları için filtreleme, sıralama ve sayfalandırmayı destekler:

### Filtreleme

```http
GET /api/devices?deviceType=TEMPERATURE_SENSOR&status=ACTIVE
Authorization: Bearer your-token-here
```

### Sıralama

```http
GET /api/devices?sortBy=name&sortOrder=asc
Authorization: Bearer your-token-here
```

### Sayfalandırma

```http
GET /api/devices?page=2&pageSize=25
Authorization: Bearer your-token-here
```

## Cihaz Gruplarıyla Çalışma

### Cihaz Grubu Oluşturma

```http
POST /api/groups
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Sunucu Odası Sensörleri",
  "description": "Sunucu odasındaki tüm sensörler",
  "devices": [
    "d290f1ee-6c54-4b01-90e6-d701748f0851"
  ]
}
```

### Bir Gruba Cihaz Ekleme

```http
POST /api/groups/f47ac10b-58cc-4372-a567-0e02b2c3d479/devices
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "deviceIds": [
    "7bba9078-8d1f-462d-81ef-24962acfe9b5",
    "32c3e9b5-0b5e-4f2d-8b9d-15a1f901aa3c"
  ]
}
```

## Hata Yönetimi

API, açıklayıcı hata mesajlarıyla birlikte standart HTTP durum kodları döndürür:

```json
{
  "status": 400,
  "message": "Geçersiz istek parametreleri",
  "errors": [
    {
      "field": "name",
      "message": "İsim gereklidir"
    }
  ],
  "timestamp": "2025-04-05T10:30:45Z",
  "path": "/api/devices"
}
```

## Sonraki Adımlar

Artık DeviceApi'nin temel işlemlerine aşina olduğunuza göre:

1. Tam [API Uç Noktaları](04-API-Uc-Noktalari.md) dokümantasyonunu keşfedin
2. [Kimlik Doğrulama](05-Kimlik-Dogrulama.md) hakkında detaylı bilgi edinin
3. [Veri Modelleri](06-Veri-Modelleri.md)'ni anlayın
4. [Yapılandırma](07-Yapilandirma.md) seçeneklerini kontrol edin

İleri düzey konular için, bakınız:

- [Performans Optimizasyonu](09-Performans-Optimizasyonu.md)
- [Güvenlik](10-Guvenlik.md)
- [Dağıtım](11-Dagitim.md)

---

[◀ Kurulum](02-Kurulum.md) | [Ana Sayfa](README.md) | [İleri: API Uç Noktaları ▶](04-API-Uc-Noktalari.md) 