# Kurulum

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölümde, JWTVerifyLibrary'nin kurulum adımlarını ve projenize entegre etme sürecini bulacaksınız.

## Önkoşullar

JWTVerifyLibrary'yi kullanmadan önce aşağıdaki gereksinimlerin karşılandığından emin olun:

- **.NET SDK 8.0** veya daha yeni bir sürüm
- **ASP.NET Core 8.0** veya daha yeni bir sürüm

## NuGet Paketi Kurulumu

JWTVerifyLibrary, NuGet üzerinden kolayca kurulabilir.

### Package Manager Console Kullanarak

```powershell
Install-Package JWTVerifyLibrary
```

### .NET CLI Kullanarak

```bash
dotnet add package JWTVerifyLibrary
```

### Visual Studio Kullanarak

1. Solution Explorer'da projenize sağ tıklayın
2. "Manage NuGet Packages..." seçeneğini seçin
3. "Browse" sekmesine tıklayın
4. "JWTVerifyLibrary" yazarak aratın
5. Paketi seçin ve "Install" düğmesine tıklayın

## Proje Referansı Olarak Ekleme

Eğer NuGet kullanmıyorsanız veya kaynak kodu üzerinde değişiklik yapmak istiyorsanız, JWTVerifyLibrary'yi bir proje referansı olarak ekleyebilirsiniz.

1. JWTVerifyLibrary projesini indirin ya da klonlayın
2. Solution'ınıza sağ tıklayın ve "Add > Existing Project..." seçeneğini seçin
3. JWTVerifyLibrary.csproj dosyasını bulun ve seçin
4. Ana projenize sağ tıklayın ve "Add > Project Reference..." seçeneğini seçin
5. JWTVerifyLibrary projesini işaretleyin ve "OK" düğmesine tıklayın

## Yapılandırma

JWTVerifyLibrary'nin düzgün çalışması için yapılandırma dosyanızda bazı ayarların yapılması gerekmektedir.

### appsettings.json Dosyasına Ayarları Ekleme

Projenizin appsettings.json dosyasına aşağıdaki JWT ayarlarını ekleyin:

```json
{
  "JwtSettings": {
    "Secret": "SizinGizliAnahtariniz12345678901234567890",
    "Issuer": "UygulamaninAdi",
    "Audience": "HedefKitle",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### JWTVerifyLibrarySettings.json Dosyası Oluşturma

Alternatif olarak, özel bir JWTVerifyLibrarySettings.json dosyası oluşturabilirsiniz. Bu yaklaşım, JWT yapılandırma ayarlarını ana yapılandırma dosyasından ayırmanıza olanak tanır.

1. Projenizin kök dizininde "JWTVerifyLibrarySettings.json" adında bir dosya oluşturun
2. Aşağıdaki içeriği ekleyin:

```json
{
  "JwtSettings": {
    "Secret": "SizinGizliAnahtariniz12345678901234567890",
    "Issuer": "UygulamaninAdi",
    "Audience": "HedefKitle",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

3. Dosyanın özellikleri penceresinden "Copy to Output Directory" seçeneğini "Copy always" olarak ayarlayın:

```xml
<ItemGroup>
  <None Update="JWTVerifyLibrarySettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Güvenlik Tavsiyesi

Güvenlik açısından aşağıdaki tavsiyelere dikkat etmeniz önemlidir:

- **Secret Key:** En az 32 karakter uzunluğunda, karmaşık ve rastgele oluşturulmuş bir Secret kullanın
- **Ortam Değişkenleri:** Production ortamında Secret key'i appsettings.json dosyasında değil, ortam değişkenlerinde saklayın
- **HTTPS:** JWT kullanımında her zaman HTTPS kullanın
- **Key Rotation:** Secret key'lerinizi düzenli olarak değiştirin

## Sorun Giderme

Kurulum sırasında yaşanan yaygın sorunlar ve çözümleri:

1. **Bağımlılık Çatışmaları:** 
   
   Çözüm: Projenizin diğer bileşenlerinin, JWTVerifyLibrary'nin bağımlı olduğu paketlerle uyumlu sürümleri kullandığından emin olun.

2. **Yapılandırma Dosyası Bulunamadı:** 
   
   Çözüm: JWTVerifyLibrarySettings.json dosyasının doğru konumda olduğunu ve çıktı dizinine kopyalandığını kontrol edin.

3. **.NET Sürüm Uyumsuzluğu:** 
   
   Çözüm: Projenizin .NET 8.0 veya üzeri bir sürüm kullandığından emin olun.

---

[◀ Giriş](01-Giris.md) | [Ana Sayfa](README.md) | [İleri: Hızlı Başlangıç ▶](03-Hizli-Baslangic.md) 