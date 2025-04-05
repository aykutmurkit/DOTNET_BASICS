# Versiyonlama

**Sürüm:** 1.0.0  
**Yazar:** Arge Mühendisi Aykut Mürkit  
**Şirket:** İSBAK 2025

---

Bu doküman, DeviceApi için sürüm stratejisini, öngörülebilir güncellemeler ve geriye dönük uyumluluk sağlamak amacıyla ana hatlarıyla belirtir.

## Sürüm Politikası

DeviceApi, sürüm numaraları ve anlamları için net bir yapı sağlayan Anlamsal Sürümlendirme (SemVer) 2.0.0'ı takip eder.

### Sürüm Formatı

Tüm sürümler şu formatı takip eder: **MAJOR.MINOR.PATCH**

- **MAJOR** (ANA) sürüm, uyumsuz API değişiklikleri için artar
- **MINOR** (ALT) sürüm, geriye dönük uyumlu işlevsellik eklemeleri için artar
- **PATCH** (YAMA) sürüm, geriye dönük uyumlu hata düzeltmeleri için artar

### Ek Etiketler

Yayın öncesi sürümler için, bir kısa çizgi ve nokta ile ayrılmış tanımlayıcılar serisi ekleyebiliriz:

- Alfa sürümleri: `1.0.0-alpha.1`, `1.0.0-alpha.2`, vb.
- Beta sürümleri: `1.0.0-beta.1`, `1.0.0-beta.2`, vb.
- Sürüm adayları: `1.0.0-rc.1`, `1.0.0-rc.2`, vb.

## API Sürümlendirme

DeviceApi, istemcilere esneklik sağlamak için birden fazla API sürümlendirme stratejisi uygular.

### URL Sürümlendirme

Birincil sürümlendirme mekanizması URL yolu üzerinden gerçekleşir:

```
https://api.example.com/v1/devices
https://api.example.com/v2/devices
```

### Başlık Tabanlı Sürümlendirme

İstemciler ayrıca API sürümünü özel bir HTTP başlığı ile belirtebilirler:

```
X-API-Version: 1.0
```

### Accept Başlığı Sürümlendirmesi

Accept başlığı kullanarak içerik müzakeresi desteklenir:

```
Accept: application/json;version=1.0
```

### Varsayılan Sürüm

Eğer bir sürüm belirtilmezse, API varsayılan olarak en son kararlı sürüme döner.

## Sürüm Yaşam Döngüsü

Her API sürümü aşağıdaki aşamalardan geçer:

1. **Önizleme**: Erken test ve geri bildirim için ilk sürüm
2. **Aktif**: Düzenli güncellemeler ve iyileştirmelerle tam desteklenen
3. **Kullanımdan Kaldırılmış**: Hala çalışıyor ancak kaldırılmak üzere planlanmış
4. **Gün Batımı**: Artık desteklenmiyor veya kullanılamıyor

### Kullanımdan Kaldırma Politikası

Bir API sürümü veya özelliği kullanımdan kaldırıldığında:

1. Gün batımından en az 6 ay önce ön bildirim sağlanacaktır
2. Dokümantasyon, kullanımdan kaldırılan özellikleri açıkça belirtecektir
3. API yanıtları, başlıklarda kullanımdan kaldırma uyarıları içerecektir
4. Değişiklik günlüğü, kullanımdan kaldırılan tüm özellikleri listeleyecektir

## Sürüm Geçmişi

### 1.0.0 (Güncel) - 2025-01-15

- DeviceApi'nin ilk kararlı sürümü
- Eksiksiz cihaz yönetim yetenekleri
- Kimlik doğrulama ve yetkilendirme sistemi
- Gerçek zamanlı veri toplama ve işleme

### 0.9.0 (Beta) - 2024-11-01

- Ortakların test etmesi için beta sürümü
- Sayfalama, filtreleme ve sıralama eklendi
- Geliştirilmiş hata yönetimi ve dokümantasyon

### 0.5.0 (Alpha) - 2024-08-15

- İç test için alfa sürümü
- Temel cihaz kaydı ve yönetimi
- İlk API yapısı ve veri modelleri

## Yükseltme Kılavuzu

Sürümler arasında yükseltme yaparken, aşağıdakileri göz önünde bulundurun:

### 0.9.x'ten 1.0.0'a Yükseltme

- Kimlik doğrulama tokenleri artık 1 saat sonra sona erer (önceden 24 saat)
- Cihaz veri uç noktası yapısı `/devices/{id}/data` olarak standardize edildi
- Cihaz kaydı için gereken alanlar eklendi: `macAddress` ve `firmwareVersion`

### 0.5.x'ten 0.9.0'a Yükseltme

- Kimlik doğrulama mekanizması Temel Kimlik Doğrulama'dan JWT'ye değiştirildi
- Yanıt formatı tüm uç noktalarda standardize edildi
- Sayfalama parametreleri `limit`/`offset`'ten `pageSize`/`page`'e değiştirildi

## Geriye Dönük Uyumluluk Sözü

DeviceApi aşağıdaki uyumluluk sözlerini verir:

### Bir Minor/Patch Sürümünde Neyi Değiştirmeyeceğiz

- Mevcut uç nokta URL'leri
- Gerekli istek parametreleri
- Yanıt alanı anlamları (ancak yeni alanlar ekleyebiliriz)
- Kimlik doğrulama mekanizmaları
- Mevcut durumlar için hata kodları

### Bir Minor Sürümünde Neler Değişebilir

- Yeni uç noktaların eklenmesi
- İsteğe bağlı parametrelerin eklenmesi
- Yanıt alanlarının eklenmesi
- Genişletilmiş enum değerleri
- Performans iyileştirmeleri

### Bir Major Sürümünde Neler Değişebilir

- Uç noktaların kaldırılması
- Parametre gereksinimlerindeki değişiklikler
- Yanıt yapısındaki değişiklikler
- Kimlik doğrulama mekanizması değişiklikleri
- Hata kodu ayarlamaları

## Sürüm Destek Politikası

DeviceApi öngörülebilir bir destek programı izler:

- Her ana sürüm en az 24 ay boyunca desteklenir
- Herhangi bir anda en az iki ana sürüm desteklenir
- Güvenlik güncellemeleri tüm desteklenen sürümler için sağlanır
- Hata düzeltmeleri en son ana sürüm için önceliklendirilir

### Kullanım Ömrü Sonu Programı

| Sürüm | Yayın Tarihi | Aktif Destek Sonu | Kullanım Ömrü Sonu |
|-------|--------------|-------------------|-------------------|
| 1.0.x | 2025-01-15   | 2026-01-15        | 2027-01-15        |
| 0.9.x | 2024-11-01   | 2025-01-15        | 2025-07-15        |
| 0.5.x | 2024-08-15   | 2024-11-01        | 2025-01-15        |

## İstemciler için Sürümlendirme En İyi Uygulamaları

DeviceApi ile sorunsuz bir deneyim sağlamak için aşağıdakileri öneririz:

1. **Her zaman bir sürüm belirtin**: Değişebilecek varsayılan sürüme güvenmeyin
2. **Sürüm bildirimlerine abone olun**: Yaklaşan değişiklikler hakkında bilgi alın
3. **Yeni sürümleri erken test edin**: Önizleme sürümlerinden yararlanın
4. **Geçişleri planlayın**: Eski sürümler kullanımdan kalkmadan önce yeni sürümlere geçiş için zaman ayırın
5. **Uyumluluk testlerini otomatikleştirin**: API'ye karşı doğrulama yapan test paketleri oluşturun

## İç Sürümlendirme Stratejisi

DeviceApi'ye katkıda bulunan geliştiriciler için:

### Veritabanı Şema Migrasyonları

Veritabanı değişiklikleri Entity Framework Core migrasyonları kullanılarak izlenir. Her migrasyon, bir zaman damgası ve açıklayıcı bir isimle sürümlendirilir:

```csharp
public partial class CihazKonumTakibiEkle_20250110 : Migration
{
    // migrasyon kodu
}
```

### Kütüphane Bağımlılıkları

Paket bağımlılıkları, kesin sürümler veya uygun sürüm aralıklarıyla belirtilir:

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
```

### Kaynak Kontrolü

Her sürüm için Git etiketleri oluşturulur ve SemVer şemasını takip eder:

```bash
git tag -a v1.0.0 -m "Sürüm 1.0.0"
git push origin v1.0.0
```

## API Değişiklik Yönetim Süreci

Yeni API değişiklikleri bu süreci izler:

1. **Öneri**: Yeni özellikler belgelenir ve gözden geçirilir
2. **Uygulama**: Değişiklikler bir özellik dalında geliştirilir
3. **İnceleme**: API değişiklikleri teknik ve tasarım incelemesinden geçer
4. **Test**: Otomatik testler geriye dönük uyumluluğu doğrular
5. **Dokümantasyon**: API değişiklikleri tamamen belgelenir
6. **Sürüm Notları**: Değişiklikler sürüm notlarında özetlenir
7. **Dağıtım**: Yeni sürüm dağıtılır ve izlenir

---

[◀ Yapılandırma](07-Yapilandirma.md) | [Ana Sayfa](README.md) | [İleri: En İyi Uygulamalar ▶](09-En-Iyi-Uygulamalar.md) 