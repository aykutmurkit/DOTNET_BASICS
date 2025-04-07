# LogLibrary Mimari Dokümantasyonu

## Versiyon 1.0.0
**Yazar:** AR-GE Mühendisi Aykut Mürkit, İsbak

## Genel Bakış

LogLibrary, esneklik, genişletilebilirlik ve ilgilerin ayrılması ilkesini sağlamak için katmanlı bir mimari ile tasarlanmıştır. Kütüphane, birkaç anahtar bileşenden oluşur:

```
┌─────────────────────────────────────────────────────┐
│                  İstemci Uygulaması                 │
└───────────────────────┬─────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│                     ILogService                      │
└───────────────────────┬─────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│                    LogService                        │
└──────┬─────────────────┬──────────────────┬─────────┘
       │                 │                  │
       ▼                 ▼                  ▼
┌─────────────┐  ┌─────────────┐  ┌──────────────────┐
│ Konsol Log  │  │  Dosya Log  │  │   MongoDB Log    │
└─────────────┘  └─────────────┘  └────────┬─────────┘
                                           │
                                           ▼
                               ┌──────────────────────┐
                               │   MongoDbContext     │
                               └──────────┬───────────┘
                                          │
                                          ▼
                               ┌──────────────────────┐
                               │   Özel Serileştir.   │
                               │ - JObjectSerializer  │
                               │ - JArraySerializer   │
                               │ - BsonDocumentSer.   │
                               └──────────────────────┘
```

## Bileşenler

### 1. Çekirdek Katmanı (Core Layer)

Alan modellerini ve arayüzleri içerir:

- **LogEntry**: Bir günlük kaydını temsil eden ana model
- **ILogService**: Günlük işlemlerini tanımlayan arayüz
- **ILogRepository**: Veri erişim işlemleri için arayüz

### 2. Servisler Katmanı (Services Layer)

İş mantığının uygulamalarını içerir:

- **LogService**: ILogService arayüzünü uygular, günlük oluşturma ve biçimlendirmeyi yönetir

### 3. Veri Katmanı (Data Layer)

Veri erişimi ve kalıcılığı yönetir:

- **MongoDbContext**: MongoDB bağlantısını ve serileştirmeyi yönetir
- **LogRepository**: MongoDB işlemleri için ILogRepository'yi uygular

### 4. Özel Serileştirme

Karmaşık veri tiplerini işlemek için özel serileştiriciler:

- **JObjectSerializer**: Tip ayırıcıları olmadan JObject serileştirmesini işler
- **JArraySerializer**: Tip ayırıcıları olmadan JArray serileştirmesini işler
- **BsonDocumentSerializer**: Genel nesneler için özel serileştirici

## Sıra Diyagramları

### Temel Günlük Akışı

```
┌───────────┐     ┌────────────┐     ┌──────────────┐    ┌────────────┐
│ Controller│     │ LogService │     │LogRepository │    │MongoDbContext│
└─────┬─────┘     └──────┬─────┘     └──────┬───────┘    └──────┬─────┘
      │                  │                   │                   │
      │  LogInfoAsync    │                   │                   │
      │─────────────────>│                   │                   │
      │                  │                   │                   │
      │                  │  CreateLogEntry   │                   │
      │                  │   (LogEntry       │                   │
      │                  │    oluşturur)     │                   │
      │                  │                   │                   │
      │                  │   SaveLogAsync    │                   │
      │                  │──────────────────>│                   │
      │                  │                   │    InsertOne      │
      │                  │                   │──────────────────>│
      │                  │                   │                   │
      │                  │                   │                   │
      │                  │                   │   Serileştirme    │
      │                  │                   │                   │
      │                  │                   │   VT İşlemi       │
      │                  │                   │                   │
      │                  │                   │<─ ─ ─ ─ ─ ─ ─ ─ ─ │
      │                  │<─ ─ ─ ─ ─ ─ ─ ─ ─│                   │
      │<─ ─ ─ ─ ─ ─ ─ ─ ─│                   │                   │
      │                  │                   │                   │
```

## MongoDB Serileştirme

LogLibrary'deki özel serileştirme sistemi, tip bilgisi eklemeden JObject gibi karmaşık veri tiplerini işlemek için tasarlanmıştır:

### Önce (tip ayırıcıları ile):
```json
{
  "Data": {
    "_t": "Newtonsoft.Json.Linq.JObject",
    "_v": {
      "userId": 123,
      "action": "login"
    }
  }
}
```

### Sonra (özel serileştirme ile):
```json
{
  "Data": {
    "userId": 123,
    "action": "login"
  }
}
```

## Genişleme Noktaları

LogLibrary genişletilebilir şekilde tasarlanmıştır:

1. **Özel Günlük Hedefleri**: Yeni depolama hedefleri için ILogRepository uygulayın
2. **Özel Serileştiriciler**: Ek karmaşık tipler için serileştiriciler ekleyin
3. **Günlük Filtreleme**: LogService'te özel günlük filtreleme mantığı uygulayın

---

© İsbak, 2025. Tüm hakları saklıdır. 