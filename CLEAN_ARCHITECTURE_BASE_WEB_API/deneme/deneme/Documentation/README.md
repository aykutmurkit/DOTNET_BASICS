# Deneme API Dokümantasyonu

Bu dokümantasyon, Deneme API'nin kullanımı, özellikleri ve teknik detayları hakkında bilgiler içerir.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Başlarken](#başlarken)
- [Doküman İçeriği](#doküman-içeriği)
- [API Sürümleri](#api-sürümleri)
- [İletişim](#i̇letişim)

## Genel Bakış

Deneme API, güvenli ve ölçeklenebilir bir kullanıcı yönetim sistemi sunar. Temel özellikleri:

- **Güçlü Kimlik Doğrulama**: JWT tabanlı yetkilendirme, güvenli oturum yönetimi
- **İki Faktörlü Kimlik Doğrulama (2FA)**: Ek güvenlik katmanı
- **Rol Tabanlı Erişim Kontrolü**: Admin, Developer ve User rolleri
- **Kullanıcı Yönetimi**: Kapsamlı profil ve hesap yönetimi
- **Standart API Yanıtları**: Tutarlı yanıt formatları

## Başlarken

### Gereksimler

- .NET 8.0 veya üzeri
- Microsoft SQL Server veya SQLite
- HTTPS desteği

### Kurulum

1. Projeyi klonlayın:
```
git clone https://github.com/example/deneme-api.git
```

2. Bağımlılıkları yükleyin:
```
dotnet restore
```

3. Veritabanını kurun:
```
dotnet ef database update
```

4. Uygulamayı çalıştırın:
```
dotnet run
```

API şu adreste çalışmaya başlayacaktır: `https://localhost:7052`

## Doküman İçeriği

- [**API Referansı**](./api-reference.md): Tüm endpoint'lerin teknik detayları
- [**Kimlik Doğrulama**](./authentication.md): Kayıt, giriş ve yetkilendirme süreçleri
- [**İki Faktörlü Kimlik Doğrulama**](./two-factor-auth.md): 2FA özelliklerinin detaylı açıklaması
- [**Kullanıcı Yönetimi**](./users.md): Kullanıcı profili ve hesap yönetimi API'leri
- [**Veritabanı Yapılandırması ve Seeding**](./database-configuration.md): Veritabanı yapılandırması ve seed işlemleri

## API Sürümleri

| Sürüm | Durum | Notlar |
|-------|-------|--------|
| v1.0  | Aktif | İlk sürüm - temel kimlik doğrulama ve kullanıcı yönetimi |
| v1.1  | Aktif | İki faktörlü kimlik doğrulama eklendi |
| v1.2  | Geliştirme | Rol tabanlı erişim kontrollerinin genişletilmesi |

## İletişim

API hakkında sorularınız veya önerileriniz için:

- **E-posta**: api-support@example.com
- **GitHub**: [github.com/example/deneme-api](https://github.com/example/deneme-api) 