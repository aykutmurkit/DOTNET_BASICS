# 01 - AuthApi: Giriş ve Genel Bakış

## Proje Hakkında

AuthApi, JWT tabanlı kimlik doğrulama, iki faktörlü doğrulama (2FA), rol tabanlı erişim kontrolü ve MongoDB tabanlı loglama sistemi içeren kapsamlı bir .NET Core Web API projesidir. Proje, güvenli kimlik doğrulama, kullanıcı yönetimi ve sistem loglama özelliklerini N-Tier mimarisi ile sunmaktadır.

## Özellikleri

- **JWT Tabanlı Kimlik Doğrulama**: Access ve refresh token yönetimi
- **İki Faktörlü Kimlik Doğrulama (2FA)**: E-posta tabanlı doğrulama kodu sistemi
- **Rol Tabanlı Erişim Kontrolü**: Admin, Developer ve User rolleri
- **Rate Limiting**: IP ve endpoint bazlı istek sınırlama
- **Loglama Sistemi**: MongoDB tabanlı kapsamlı sistem
- **Veri Maskeleme**: Hassas verilerin otomatik gizlenmesi
- **Kullanıcı Yönetimi**: Profil oluşturma, güncelleme, şifre sıfırlama
- **E-posta Entegrasyonu**: Doğrulama, şifre sıfırlama ve bildirimler için

## Teknoloji Stack

### Backend
- **.NET Core 6+**
- **Entity Framework Core**: Veritabanı erişimi için ORM
- **Identity Framework**: Kimlik doğrulama ve yetkilendirme
- **MongoDB Driver**: Loglama veritabanı erişimi
- **JWT Authentication**: Token tabanlı kimlik doğrulama

### Veritabanı
- **SQL Server**: Ana veritabanı olarak
- **MongoDB**: Loglama sistemi için

### Yardımcı Kütüphaneler
- **AutoMapper**: Nesneler arası eşleme için
- **FluentValidation**: Giriş verisi doğrulama için
- **Swagger/OpenAPI**: API dokümantasyonu için
- **Serilog**: Konsol ve dosya loglama için
- **MailKit**: E-posta gönderimi için

## Gereksinimler

- .NET Core 6.0+ SDK
- SQL Server veya başka bir uyumlu ilişkisel veritabanı
- MongoDB (loglama sistemi için)
- SMTP sunucusu erişimi (e-posta gönderimi için)

## Genel Dizin Yapısı

```
AuthApi/
├── API/
│   ├── Controllers/          # API endpoint kontrolcüleri
│   └── Extensions/           # Uygulama yapılandırma uzantıları
│
├── Business/
│   ├── Services/             # İş mantığı servisleri
│   │   ├── Interfaces/       # Servis sözleşmeleri
│   │   └── Concrete/         # Servis implementasyonları
│   └── Extensions/           # Servis kayıt uzantıları
│
├── Core/
│   ├── Security/             # Güvenlik özellikleri
│   ├── Utilities/            # Yardımcı sınıflar
│   ├── Extensions/           # Uzantı metodları
│   └── Enums/                # Enum tanımları
│
├── DataAccess/
│   ├── Context/              # Veritabanı bağlam sınıfları
│   ├── Seeding/              # Başlangıç verisi ekleyen sınıflar
│   ├── Repositories/         # Veritabanı erişim sınıfları
│   └── Configurations/       # Entity yapılandırmaları
│
├── Entities/
│   ├── Concrete/             # Veritabanı model sınıfları
│   └── Dtos/                 # Transfer nesneleri
│
├── wwwroot/                  # Statik dosyalar
│
└── Properties/               # Proje ayarları
``` 