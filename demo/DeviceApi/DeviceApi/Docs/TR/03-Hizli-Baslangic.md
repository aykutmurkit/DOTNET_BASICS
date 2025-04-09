# Hızlı Başlangıç

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

## API'ye Genel Bakış

DeviceApi, IoT cihazlarının yönetimi için geliştirilmiş bir RESTful API'dir. Bu kılavuz, API'yi hızlı bir şekilde kullanmaya başlamanıza yardımcı olacaktır.

## Temel Endpoint'ler

### Kimlik Doğrulama

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

### Cihaz Yönetimi

```http
# Tüm cihazları listele
GET /api/Devices

# Yeni cihaz ekle
POST /api/Devices
Content-Type: application/json

{
  "name": "Test Cihazı",
  "ip": "192.168.1.100",
  "port": 8080,
  "platformId": 1
}

# Cihaz güncelle
PUT /api/Devices/{id}
Content-Type: application/json

{
  "name": "Güncellenmiş Cihaz",
  "ip": "192.168.1.101",
  "port": 8081
}
```

### Mesaj Yönetimi

#### Tam Ekran Mesajları

```http
# Tüm tam ekran mesajlarını listele
GET /api/FullScreenMessages

# Yeni tam ekran mesajı ekle
POST /api/FullScreenMessages
Content-Type: application/json

{
  "turkishMessage": "Merhaba Dünya",
  "englishMessage": "Hello World"
}
```

#### Kaydırma Ekran Mesajları

```http
# Tüm kaydırma ekran mesajlarını listele
GET /api/ScrollingScreenMessages

# Yeni kaydırma ekran mesajı ekle
POST /api/ScrollingScreenMessages
Content-Type: application/json

{
  "turkishLines": ["Satır 1", "Satır 2"],
  "englishLines": ["Line 1", "Line 2"]
}
```

#### Bitmap Ekran Mesajları

```http
# Tüm bitmap ekran mesajlarını listele
GET /api/BitmapScreenMessages

# Yeni bitmap ekran mesajı ekle
POST /api/BitmapScreenMessages
Content-Type: application/json

{
  "turkishBitmap": "base64_encoded_bitmap",
  "englishBitmap": "base64_encoded_bitmap"
}
```

#### Periyodik Mesajlar

```http
# Tüm periyodik mesajları listele
GET /api/PeriodicMessages

# Yeni periyodik mesaj ekle
POST /api/PeriodicMessages
Content-Type: application/json

{
  "message": "Periyodik Mesaj",
  "startTime": "2024-01-01T00:00:00",
  "endTime": "2024-12-31T23:59:59",
  "intervalInMinutes": 60
}
```

## Örnek Kullanım Senaryoları

### Senaryo 1: Cihaz Ekleme ve Mesaj Atama

1. Yeni bir cihaz ekleyin
2. Cihaza tam ekran mesajı atayın
3. Cihazın durumunu kontrol edin

```http
# 1. Cihaz ekle
POST /api/Devices
{
  "name": "Yeni Cihaz",
  "ip": "192.168.1.100",
  "port": 8080,
  "platformId": 1
}

# 2. Tam ekran mesajı oluştur
POST /api/FullScreenMessages
{
  "turkishMessage": "Hoş Geldiniz",
  "englishMessage": "Welcome"
}

# 3. Mesajı cihaza ata
POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "FullScreen"
}

# 4. Cihaz durumunu kontrol et
GET /api/Devices/{deviceId}/status
```

### Senaryo 2: Çoklu Mesaj Yönetimi

1. Farklı türlerde mesajlar oluşturun
2. Mesajları cihazlara atayın
3. Mesaj durumlarını izleyin

```http
# 1. Farklı türlerde mesajlar oluştur
POST /api/FullScreenMessages
{
  "turkishMessage": "Tam Ekran Mesaj",
  "englishMessage": "Full Screen Message"
}

POST /api/ScrollingScreenMessages
{
  "turkishLines": ["Kaydırma 1", "Kaydırma 2"],
  "englishLines": ["Scroll 1", "Scroll 2"]
}

POST /api/BitmapScreenMessages
{
  "turkishBitmap": "base64_encoded_bitmap",
  "englishBitmap": "base64_encoded_bitmap"
}

# 2. Mesajları cihazlara ata
POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "FullScreen"
}

POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "ScrollingScreen"
}

# 3. Mesaj durumlarını kontrol et
GET /api/Devices/{deviceId}/messages
```

## Hata Yönetimi

API, standart HTTP durum kodlarını ve hata mesajlarını kullanır:

- `200 OK`: İşlem başarılı
- `201 Created`: Kaynak başarıyla oluşturuldu
- `400 Bad Request`: Geçersiz istek
- `401 Unauthorized`: Kimlik doğrulama gerekli
- `403 Forbidden`: Yetkisiz erişim
- `404 Not Found`: Kaynak bulunamadı
- `500 Internal Server Error`: Sunucu hatası

Örnek hata yanıtı:

```json
{
  "success": false,
  "message": "Cihaz bulunamadı",
  "errors": [
    {
      "code": "DeviceNotFound",
      "description": "Belirtilen ID'ye sahip cihaz bulunamadı"
    }
  ]
}
```

## Güvenlik

API'ye erişmek için:

1. `/api/Auth/login` endpoint'ini kullanarak JWT token alın
2. Token'ı `Authorization` header'ında gönderin:
   ```
   Authorization: Bearer your_jwt_token
   ```

## Sonraki Adımlar

1. [Mimari Yapı](06-Mimari-Yapi.md) dokümanını inceleyin
2. [Veri Modelleri](05-Veri-Modelleri.md) hakkında bilgi edinin
3. [En İyi Uygulamalar](09-En-Iyi-Uygulamalar.md) kılavuzunu okuyun

---

[◀ Kurulum](02-Kurulum.md) | [İleri: Veri Modelleri ▶](05-Veri-Modelleri.md) 