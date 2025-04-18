# Frontend'den Backend'e API Gereksinimleri

Bu doküman, frontend ekibi olarak backend API implementasyonundan beklentilerimizi tanımlamaktadır. Aşağıdaki özellikleri ve standartları içeren bir API'nin frontend uygulamamızla sorunsuz bir şekilde entegre edilebilmesi için kritik öneme sahiptir.

## API Yanıt Standardı

Tüm API yanıtları aşağıdaki şemaya uygun olmalıdır:

```json
{
  "success": true/false,
  "message": "İşlem ile ilgili kullanıcı dostu mesaj",
  "data": {}, // İşleme özgü veri (başarılı yanıtlarda)
  "errors": {}, // Validasyon hataları (başarısız yanıtlarda)
  "statusCode": 200 // HTTP durum kodu
}
```

Bu yapı, frontend'de tutarlı bir hata işleme ve kullanıcı bildirimi mekanizması kurmamızı sağlayacaktır.

## HTTP Durum Kodları

API, aşağıdaki HTTP durum kodlarını uygun şekilde kullanmalıdır:

- **200 OK**: Başarılı GET, PUT, PATCH istekleri
- **201 Created**: Başarılı POST istekleri (yeni kaynak oluşturma)
- **400 Bad Request**: Validasyon hataları, hatalı istek formatı
- **401 Unauthorized**: Kimlik doğrulama hatası, geçersiz token
- **403 Forbidden**: Kimlik doğrulanmış ancak yetkisiz erişim
- **404 Not Found**: Kaynak bulunamadı
- **409 Conflict**: Çakışma durumu (örn. aynı kullanıcı adı/e-posta)
- **429 Too Many Requests**: Rate limit aşıldı
- **500 Server Error**: Sunucu kaynaklı hatalar

## Validasyon Mekanizması

Frontend'deki form validasyonları ile uyumlu backend validasyonu istiyoruz:

1. Tüm validasyon hataları, standart API yanıt formatındaki `errors` alanında ilgili alan adına göre gruplanmalı
2. Her alan için hatalar bir dizi içinde olmalı (birden fazla hata olabilir)
3. Hata mesajları kullanıcı dostu ve Türkçe olmalı

Örnek validasyon hatası yanıtı:

```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "username": ["Kullanıcı adı zorunludur", "Kullanıcı adı en az 3 karakter olmalıdır"],
    "password": ["Şifre en az 8 karakter olmalıdır", "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir"]
  },
  "statusCode": 400
}
```

## Kimlik Doğrulama ve Yetkilendirme

JWT tabanlı bir kimlik doğrulama sistemi bekliyoruz:

1. Access token: 11 saat geçerlilik
2. Refresh token: 7 gün geçerlilik
3. Tüm güvenli endpoint'ler için `Authorization: Bearer {token}` header kontrolü
4. İki faktörlü kimlik doğrulama desteği

## Rate Limiting

Güvenlik ve performans için rate limiting uygulanmalı:

1. Global limite ek olarak kritik endpointler için özel limitler tanımlanmalı:
   - Login endpointleri: 5 dakikada 10 istek
   - Kayıt endpointleri: 10 dakikada 3 istek
   - Şifre sıfırlama: 30 dakikada 3 istek
   - Hassas veri işlemleri: 5 dakikada 5 istek

2. Rate limit aşıldığında frontend'e uygun HTTP 429 yanıtı dönülmeli

## Örnek İstek/Yanıt Formatları

Bu bölümde, beklenen istek ve yanıt formatlarına dair genel örnekler sunulmuştur. Backend geliştiriciler kendi projelerindeki endpoint'leri bu formatlara uygun şekilde tasarlamalıdır.

### Örnek 1: Veri Listeleme Endpoint'i

#### İstek
```
GET /api/items?page=1&pageSize=10&sortBy=createdAt&sortDirection=desc
```

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Kayıtlar başarıyla getirildi",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Örnek Öğe 1",
        "description": "Açıklama metni",
        "category": "Kategori A",
        "price": 149.99,
        "imageUrl": "https://example.com/images/item1.jpg",
        "createdAt": "2023-04-30T12:00:00Z"
      },
      {
        "id": 2,
        "name": "Örnek Öğe 2",
        "description": "Açıklama metni 2",
        "category": "Kategori B",
        "price": 89.99,
        "imageUrl": "https://example.com/images/item2.jpg",
        "createdAt": "2023-04-29T15:30:00Z"
      }
      // ... diğer öğeler
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalItems": 42,
      "totalPages": 5,
      "hasNext": true,
      "hasPrevious": false
    }
  },
  "statusCode": 200
}
```

### Örnek 2: Tekil Kayıt Getirme Endpoint'i

#### İstek
```
GET /api/items/1
```

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Kayıt başarıyla getirildi",
  "data": {
    "id": 1,
    "name": "Örnek Öğe 1",
    "description": "Detaylı açıklama metni",
    "category": "Kategori A",
    "price": 149.99,
    "stock": 25,
    "imageUrl": "https://example.com/images/item1.jpg",
    "images": [
      "https://example.com/images/item1_1.jpg",
      "https://example.com/images/item1_2.jpg",
      "https://example.com/images/item1_3.jpg"
    ],
    "specifications": {
      "weight": "1.2 kg",
      "dimensions": "15 x 10 x 5 cm",
      "color": "Siyah"
    },
    "createdAt": "2023-04-30T12:00:00Z",
    "updatedAt": "2023-05-01T09:15:00Z"
  },
  "statusCode": 200
}
```

#### Kayıt Bulunamadı Yanıtı (404 Not Found)
```json
{
  "success": false,
  "message": "Kayıt bulunamadı",
  "statusCode": 404
}
```

### Örnek 3: Kayıt Ekleme Endpoint'i

#### İstek
```
POST /api/items
```

```json
{
  "name": "Yeni Öğe",
  "description": "Yeni öğe açıklaması",
  "category": "Kategori C",
  "price": 199.99,
  "stock": 10,
  "specifications": {
    "weight": "0.8 kg",
    "dimensions": "12 x 8 x 4 cm",
    "color": "Mavi"
  }
}
```

#### Başarılı Yanıt (201 Created)
```json
{
  "success": true,
  "message": "Kayıt başarıyla oluşturuldu",
  "data": {
    "id": 43,
    "name": "Yeni Öğe",
    "description": "Yeni öğe açıklaması",
    "category": "Kategori C",
    "price": 199.99,
    "stock": 10,
    "imageUrl": null,
    "specifications": {
      "weight": "0.8 kg",
      "dimensions": "12 x 8 x 4 cm",
      "color": "Mavi"
    },
    "createdAt": "2023-05-15T14:22:00Z",
    "updatedAt": "2023-05-15T14:22:00Z"
  },
  "statusCode": 201
}
```

#### Validasyon Hatası Yanıtı (400 Bad Request)
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "name": ["İsim alanı zorunludur"],
    "price": ["Fiyat 0'dan büyük olmalıdır"],
    "category": ["Geçersiz kategori değeri"]
  },
  "statusCode": 400
}
```

### Örnek 4: Kayıt Güncelleme Endpoint'i

#### İstek
```
PUT /api/items/43
```

```json
{
  "name": "Güncellenmiş Öğe",
  "description": "Güncellenmiş açıklama",
  "price": 179.99,
  "stock": 15
}
```

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Kayıt başarıyla güncellendi",
  "data": {
    "id": 43,
    "name": "Güncellenmiş Öğe",
    "description": "Güncellenmiş açıklama",
    "category": "Kategori C",
    "price": 179.99,
    "stock": 15,
    "imageUrl": null,
    "specifications": {
      "weight": "0.8 kg",
      "dimensions": "12 x 8 x 4 cm",
      "color": "Mavi"
    },
    "createdAt": "2023-05-15T14:22:00Z",
    "updatedAt": "2023-05-15T15:05:00Z"
  },
  "statusCode": 200
}
```

### Örnek 5: Kayıt Silme Endpoint'i

#### İstek
```
DELETE /api/items/43
```

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Kayıt başarıyla silindi",
  "statusCode": 200
}
```

### Örnek 6: Dosya Yükleme Endpoint'i

#### İstek
```
POST /api/items/43/images
```
Multipart form data ile resim dosyası gönderimi

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Resim başarıyla yüklendi",
  "data": {
    "imageUrl": "https://example.com/images/item43_1.jpg",
    "thumbnailUrl": "https://example.com/images/thumbnails/item43_1.jpg"
  },
  "statusCode": 200
}
```

#### Geçersiz Dosya Hatası (400 Bad Request)
```json
{
  "success": false,
  "message": "Geçersiz dosya formatı",
  "errors": {
    "image": ["Sadece JPG, PNG ve GIF formatında dosyalar kabul edilir", "Dosya boyutu 5MB'dan küçük olmalıdır"]
  },
  "statusCode": 400
}
```

### Örnek 7: Filtreleme ve Arama Endpoint'i

#### İstek
```
GET /api/items/search?query=örnek&category=A,B&minPrice=50&maxPrice=200&page=1&pageSize=20
```

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Arama sonuçları başarıyla getirildi",
  "data": {
    "items": [
      // Arama sonuçları
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 7,
      "totalPages": 1,
      "hasNext": false,
      "hasPrevious": false
    },
    "filters": {
      "appliedFilters": {
        "query": "örnek",
        "category": ["A", "B"],
        "minPrice": 50,
        "maxPrice": 200
      },
      "availableCategories": [
        {"id": "A", "name": "Kategori A", "count": 5},
        {"id": "B", "name": "Kategori B", "count": 2},
        {"id": "C", "name": "Kategori C", "count": 0}
      ],
      "priceRange": {
        "min": 49.99,
        "max": 199.99
      }
    }
  },
  "statusCode": 200
}
```

## Hata İşleme Beklentileri

1. **Giriş hatası (401):**
```json
{
  "success": false,
  "message": "Kullanıcı adı veya şifre hatalı",
  "statusCode": 401
}
```

2. **Aynı kullanıcı adı/e-posta kullanımı (409):**
```json
{
  "success": false,
  "message": "Bu kullanıcı adı zaten kullanılıyor",
  "statusCode": 409
}
```

3. **Şifre politikası hatası (400):**
```json
{
  "success": false,
  "message": "Lütfen form alanlarını kontrol ediniz",
  "errors": {
    "password": ["Şifre en az 8 karakter olmalıdır", 
                "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir"]
  },
  "statusCode": 400
}
```

4. **Geçersiz dosya hatası (400):**
```json
{
  "success": false,
  "message": "Desteklenmeyen dosya formatı. Lütfen JPG, PNG veya GIF formatında resim yükleyin",
  "statusCode": 400
}
```

5. **Rate limiting hatası (429):**
```json
{
  "success": false,
  "message": "Çok fazla istek gönderdiniz. Lütfen X saniye sonra tekrar deneyin.",
  "data": {
    "retryAfterSeconds": 30
  },
  "statusCode": 429
}
```

## Güvenlik Gereksinimleri

1. HTTPS zorunluluğu
2. Şifre politikası: minimum 8 karakter, en az 1 büyük harf, 1 küçük harf, 1 rakam, 1 özel karakter
3. CORS yapılandırması (frontend URL'lerinin izin verilen origins listesinde olması)
4. Rate limiting uygulanması
5. Token rotasyonu (her refresh token kullanımında yeni bir refresh token üretilmesi)
6. Hassas bilgiler için veri maskeleme (loglar, hata mesajları)

## Performans Beklentileri

1. API yanıt süresi: 300ms'den az (95. yüzdelik dilim)
3. Ağır işlemler için asenkron işleme
4. Büyük veri kümeleri için sayfalama desteği

## Test ve Dokümantasyon Gereksinimleri

1. Swagger/OpenAPI arayüzü ile API dokümantasyonu
2. Her endpoint için örnek istek ve yanıt bilgisi
3. Test ortamı için dummy hesaplar ve test verileri

## Profil Resmi Yükleme Gereksinimleri

### Profil Resmi Yükleme Endpoint'i

#### İstek
```
POST /api/Users/profile-picture
```
Multipart form data ile resim dosyası gönderimi

#### Başarılı Yanıt (200 OK)
```json
{
  "success": true,
  "message": "Profil resmi başarıyla yüklendi",
  "data": {
    "profilePictureUrl": "/api/Users/{userId}/profile-picture"
  },
  "statusCode": 200
}
```

#### Validasyon Hatası (400 Bad Request)
```json
{
  "success": false,
  "message": "Geçersiz profil resmi",
  "errors": {
    "file": [
      "Profil resmi kare formatta olmalıdır (örn. 200x200)",
      "Profil resmi maksimum 1000x1000 piksel boyutunda olmalıdır",
      "Dosya boyutu maksimum 5MB olmalıdır",
      "Sadece JPG, PNG ve GIF formatları desteklenir"
    ]
  },
  "statusCode": 400
}
```

### Profil Resmi Görüntüleme Endpoint'i

#### İstek
```
GET /api/Users/{userId}/profile-picture
```

#### Başarılı Yanıt
- Content-Type: image/png, image/jpeg veya image/gif
- Binary resim verisi

### Profil Resmi Teknik Gereksinimleri

1. **Resim Formatı ve Boyut**
   - Desteklenen formatlar: JPG, PNG, GIF
   - Maksimum dosya boyutu: 5MB
   - Resim boyutları: Kare format (genişlik = yükseklik)
   - Maksimum boyut: 1000x1000 piksel
   - Önerilen boyut: 200x200 piksel

2. **Resim İşleme**
   - Yüklenen resimler otomatik olarak optimize edilmeli
   - Büyük resimler önerilen boyuta küçültülmeli
   - Resim kalitesi korunmalı (JPEG için minimum %80 kalite)
   - EXIF bilgileri temizlenmeli

3. **Depolama ve CDN**
   - Resimler CDN üzerinde saklanmalı
   - CDN URL yapısı: `https://cdn.example.com/profiles/{userId}.{ext}`
   - CDN'de önbellekleme süresi: 1 saat
   - Resim güncellendiğinde CDN önbelleği temizlenmeli
   - Varsayılan profil resmi için ayrı bir CDN yolu: `https://cdn.example.com/profiles/default.png`

4. **Güvenlik**
   - Dosya içeriği ve MIME type kontrolü
   - Zararlı yazılım taraması
   - Maksimum yükleme boyutu sınırı
   - Yalnızca kimlik doğrulaması yapılmış kullanıcılar resim yükleyebilmeli
   - Rate limiting: 5 dakikada maksimum 3 resim yükleme

5. **Performans**
   - Resim sıkıştırma ve optimizasyon
   - Progressive loading desteği
   - WebP format desteği (tarayıcı uyumluluğuna göre)
   - Responsive image desteği (farklı boyutlar için)

6. **Hata İşleme**
   - Geçersiz dosya formatı
   - Boyut sınırı aşımı
   - Resim boyutu uyumsuzluğu
   - Depolama/CDN hataları
   - Rate limit aşımı

---

