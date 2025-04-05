# Versiyonlama

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu bölüm, JWTVerifyLibrary'nin sürüm politikasını, güncellemelerin nasıl dağıtılacağını ve geriye dönük uyumluluk garantilerini açıklar.

## Sürüm Numaralandırma

JWTVerifyLibrary, [Semantic Versioning (SemVer)](https://semver.org/lang/tr/) şemasını takip eder. Sürüm numaraları aşağıdaki formatta belirlenir:

```
X.Y.Z
```

Burada:
- **X** = Ana Sürüm (Major Version): Geriye dönük uyumsuz değişiklikler yapıldığında
- **Y** = Alt Sürüm (Minor Version): Geriye dönük uyumlu bir şekilde işlevsellik eklendiğinde
- **Z** = Yama (Patch): Geriye dönük uyumlu hata düzeltmeleri yapıldığında

Örnek:
- `1.0.0` - İlk kararlı sürüm
- `1.1.0` - Yeni özellikler eklenmiş, geriye dönük uyumlu
- `1.1.1` - Hata düzeltmeleri yapılmış
- `2.0.0` - Geriye dönük uyumsuz değişiklikler içeren yeni sürüm

## Ön Sürümler ve Derleme Metadatası

Geliştirme öncesi sürümler ve test sürümleri için aşağıdaki formatlar kullanılabilir:

- **Ön-sürüm (Pre-release)**: `X.Y.Z-alpha.N`, `X.Y.Z-beta.N`, `X.Y.Z-rc.N`
- **Derleme metadatası (Build metadata)**: `X.Y.Z+YYYYMMDDHHMMSS`

Örnekler:
- `1.0.0-alpha.1` - İlk alfa sürümü
- `1.0.0-beta.2` - İkinci beta sürümü
- `1.0.0-rc.1` - İlk yayın adayı
- `1.0.0+20250115103045` - 15 Ocak 2025 tarihli derleme

## Sürüm Geçmişi

### 1.0.0 (15 Ocak 2025)

- İlk kararlı sürüm
- JWT token doğrulama temel işlevselliği
- ASP.NET Core entegrasyonu
- Middleware destek
- Extension metodları
- Basit yapılandırma mekanizması

### 0.9.0 (10 Aralık 2024)

- RC (Release Candidate) sürümü
- Public API'ların finalize edilmesi
- Performans iyileştirmeleri
- Dokümantasyon eklenmesi

### 0.5.0 (1 Ekim 2024)

- Beta sürümü
- JWT doğrulama işlevleri
- İlk middleware uygulaması

### 0.1.0 (15 Ağustos 2024)

- Alfa sürümü
- Temel proje yapısı

## Yükseltme Rehberi

Farklı sürüm numaraları için yükseltme rehberi:

### Yama Sürümleri (Patch, Z artışı)

Yama sürümleri yalnızca hata düzeltmeleri içerir ve geriye dönük olarak tamamen uyumludur. Kodunuzu değiştirmeden güncelleyebilirsiniz.

```xml
<PackageReference Include="JWTVerifyLibrary" Version="1.0.1" />
```

### Alt Sürümler (Minor, Y artışı)

Alt sürümler, yeni özellikler ekler ancak mevcut işlevselliği değiştirmez. Genellikle güvenli bir yükseltmedir, ancak yeni davranışları gözden geçirmek iyi olur.

```xml
<PackageReference Include="JWTVerifyLibrary" Version="1.1.0" />
```

Yeni özellikler için dokümantasyonu kontrol edin ve isteğe bağlı olarak bunları kullanın.

### Ana Sürümler (Major, X artışı)

Ana sürümler, geriye dönük uyumsuz değişiklikler içerir. Yükseltme yapmadan önce aşağıdakileri yapmanız gerekir:

1. Sürüm notlarını dikkatlice okuyun
2. Değişiklik günlüğündeki değişiklikleri inceleyin
3. Uygulamanızı test edin
4. Gerekirse kodunuzu yeni API ile uyumlu olacak şekilde güncelleyin

Örneğin, 1.x.x'ten 2.0.0'a geçiş:

```xml
<PackageReference Include="JWTVerifyLibrary" Version="2.0.0" />
```

## Geriye Dönük Uyumluluk Politikası

### Garantiler

- **Yama sürümleri (Patch)**: Tam geriye dönük uyumluluk garantisi verilir.
- **Alt sürümler (Minor)**: Public API'lar ve davranışlar için geriye dönük uyumluluk. Yeni özellikler etkileşime girmediği sürece mevcut kodunuz çalışmaya devam edecektir.
- **Ana sürümler (Major)**: Geriye dönük uyumluluk garanti edilmez. Kodunuzda değişiklikler gerekebilir.

### Değişiklik Kapsamı

API'mızı kullanırken aşağıdaki değişiklikleri bekleyebilirsiniz:

#### Yama Sürümlerinde (Patch):
- Hata düzeltmeleri
- Güvenlik güncellemeleri
- İç uygulamada değişiklikler (public API değişmez)
- Belgeleme iyileştirmeleri

#### Alt Sürümlerde (Minor):
- Yeni özellikler
- Yeni API'lar
- Yeni genişletilebilirlik noktaları
- Kullanımdan kaldırma uyarıları (ama işlevsellik korunur)

#### Ana Sürümlerde (Major):
- API'ların değiştirilmesi veya kaldırılması
- Davranış değişiklikleri
- Desteklenen .NET sürümlerinde değişiklikler
- Önceki sürümlerde kullanımdan kaldırılmış özelliklerin kaldırılması

## Destek Yaşam Döngüsü

JWTVerifyLibrary sürümleri için destek politikamız:

- **Ana sürümler (Major)**: En son ana sürüm ve bir önceki ana sürüm desteklenir.
- **Alt sürümler (Minor)**: Her ana sürüm için yalnızca en son alt sürüm aktif olarak desteklenir.
- **Yama sürümleri (Patch)**: Kritik güvenlik güncellemeleri tüm desteklenen sürümlere uygulanır.

Örneğin, şu anda 2.3.0 sürümü mevcutsa:
- 2.3.0 - Tam destek
- 2.0.0-2.2.x - Yalnızca güvenlik güncellemeleri
- 1.x.x - Son ana sürüm olarak yalnızca kritik güvenlik güncellemeleri (1.5.x)
- 0.x.x - Desteklenmiyor

## LTS (Uzun Süreli Destek) Sürümleri

Bazı sürümler, uzun süreli destek (LTS) olarak belirlenebilir. Bu sürümler, standart destek süresinden daha uzun süre desteklenir.

LTS sürümleri şu şekilde işaretlenir:
- 1.0.0-LTS
- 2.0.0-LTS

LTS sürümleri, kararlılık ve uzun vadeli destek gerektiren kurumsal uygulamalar için idealdir.

## Yükseltme Yardımcı Araçları

JWTVerifyLibrary, ana sürüm yükseltmelerinde yardımcı olmak için bazı araçlar sunar:

- **UpgradeAssistant**: Eski sürümden yeni sürüme otomatik kod dönüşümleri gerçekleştirir.
- **MigrationGuide.md**: Her major sürüm için ayrıntılı yükseltme adımları içeren belge.
- **ApiCompatibilityChecker**: Kodunuzun yeni API ile uyumluluğunu kontrol eden araç.

## Kullanımdan Kaldırma Politikası

Özellikler kullanımdan kaldırılmadan önce:

1. Özellik, alt sürüm güncellemesinde `[Obsolete]` olarak işaretlenir
2. Bir sonraki alt sürümde uyarı mesajı eklenir
3. En az bir ana sürüm boyunca uyarı ile birlikte korunur
4. Sonraki ana sürümde kaldırılabilir

Örnek kullanımdan kaldırma süreci:

```csharp
// 1.5.0 sürümünde
[Obsolete]
public void OldMethod() { ... }

// 1.6.0 sürümünde
[Obsolete("Bu metod 2.0.0 sürümünde kaldırılacak. Bunun yerine NewMethod() kullanın.")]
public void OldMethod() { ... }

// 2.0.0 sürümünde
// OldMethod artık mevcut değil
```

## Sürüm Kontrolü ve Dağıtım

JWTVerifyLibrary sürümleri aşağıdaki şekilde dağıtılır:

1. **NuGet Paketleri**: Tüm resmi sürümler [NuGet.org](https://www.nuget.org)'da yayınlanır
2. **GitHub Releases**: Her sürüm için kaynak kodu [GitHub](https://github.com/isbak/JWTVerifyLibrary/releases) üzerinden etiketlenir
3. **Dağıtım Sıklığı**:
   - Yama sürümleri: Gerektiğinde (hata düzeltmeleri için)
   - Alt sürümler: 2-3 ayda bir
   - Ana sürümler: Yılda 1-2 kez

---

[◀ API Referansı](07-API-Referansi.md) | [Ana Sayfa](README.md) | [İleri: En İyi Uygulamalar ▶](09-En-Iyi-Uygulamalar.md) 