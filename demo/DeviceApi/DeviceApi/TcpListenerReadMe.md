# TCP Listener Servisi Dokümantasyonu

## İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Mimari Yapı](#mimari-yapı)
3. [Bileşenler](#bileşenler)
4. [Güvenlik Özellikleri](#güvenlik-özellikleri)
5. [Mesaj İşleme](#mesaj-işleme)
6. [Yapılandırma Seçenekleri](#yapılandırma-seçenekleri)
7. [Performans Özellikleri](#performans-özellikleri)
8. [Kullanım Örnekleri](#kullanım-örnekleri)
9. [Sorun Giderme](#sorun-giderme)

## Genel Bakış

DeviceApi.TCPListener, cihazlardan gelen TCP bağlantılarını yönetmek, cihazları doğrulamak ve mesajları işlemek için tasarlanmış bir servistir. Bu servis, IoT cihazları, sensörler veya diğer ağ cihazlarından TCP üzerinden veri alabilir, onların kimliklerini doğrulayabilir ve iletilen verileri işleyebilir.

Temel özellikleri:
- TCP bağlantı yönetimi
- IMEI numarası ile cihaz doğrulama
- Kara liste mekanizması
- Hız sınırlama (rate limiting)
- Asenkron mesaj işleme
- Detaylı log ve istatistik takibi

## Mimari Yapı

TCP Listener servisi, .NET Core'un BackgroundService yapısını kullanarak uzun süreli çalışan bir servis olarak tasarlanmıştır. Servis, TCP soketleri üzerinden istemci bağlantılarını kabul eder, her bağlantıyı ayrı bir thread'de işler ve gerekli işlemler tamamlandıktan sonra kaynakları temizler.

```
+----------------+      +------------------+      +------------------+
|   TCP Server   | ---> | Device Verifier  | ---> | Approved Devices |
+----------------+      +------------------+      +------------------+
        |                       |                         |
        v                       v                         v
+----------------+      +------------------+      +------------------+
| Message Handler| <--- | Blacklist Control| <--- |Unapproved Devices|
+----------------+      +------------------+      +------------------+
```

## Bileşenler

### TcpServer (Connection/TcpServer.cs)

Ana TCP sunucu bileşenidir. BackgroundService'ten türetilerek uzun süreli çalışan bir servis olarak davranır. Temel sorumlulukları:

- TCP soketini açıp belirtilen port ve IP adresinde dinleme yapmak
- Gelen istemci bağlantılarını kabul etmek
- Her istemci bağlantısını ayrı bir thread'de işlemek
- Bağlantı sayısı limitlerini kontrol etmek
- Bağlantı istatistiklerini toplamak ve raporlamak
- İstemcilerden gelen mesajları işlemek için MessageHandler'a iletmek

```csharp
public class TcpServer : BackgroundService, ITcpServer
{
    // ... Kodlar ...
}
```

### DeviceVerifier (Security/DeviceVerifier.cs)

Cihaz kimlik doğrulama servisini sağlar. IMEI numarası kullanarak cihazların onaylı olup olmadığını kontrol eder ve onaysız cihazların davranışlarını yönetir. Temel sorumlulukları:

- IMEI formatının geçerliliğini kontrol etmek
- Onaylı cihazların listesini yönetmek
- Onaysız cihazları kaydetmek ve izlemek
- Kara liste mekanizmasını yönetmek
- Hız sınırlamasını yönetmek
- Süresi dolmuş kara liste ve hız sınırlaması kayıtlarını temizlemek

```csharp
public class DeviceVerifier : IDeviceVerifier
{
    // ... Kodlar ...
}
```

### MessageHandler (Messaging/MessageHandler.cs)

İstemcilerden gelen mesajları işleyen ve yanıt üreten servistir. Temel sorumlulukları:

- Gelen mesajları doğrulamak ve ayrıştırmak
- Gerektiğinde cihaz doğrulaması yapmak
- Mesajlara uygun yanıtlar üretmek
- Mesaj istatistiklerini tutmak
- Gerektiğinde performans için log kısıtlaması yapmak

```csharp
public class MessageHandler : IMessageHandler
{
    // ... Kodlar ...
}
```

### Konfigürasyon (Configuration/TcpListenerSettings.cs)

TCP Listener servisinin tüm yapılandırma ayarlarını içerir. Bu ayarlar, appsettings.json dosyasından yüklenir ve Options pattern kullanılarak servislere enjekte edilir.

```csharp
public class TcpListenerSettings
{
    public int Port { get; set; } = 3456;
    public string IpAddress { get; set; } = "0.0.0.0";
    // ... Diğer ayarlar ...
}
```

## Güvenlik Özellikleri

TCP Listener, çeşitli güvenlik özellikleri sağlar:

### Cihaz Doğrulama

- IMEI numarası ile cihaz kimliği doğrulanır
- Yalnızca onaylı cihazlar veri gönderebilir
- Onaylı olmayan cihazlar otomatik olarak kaydedilir ve izlenir
- IMEI format kontrolü yapılır (15-16 haneli rakam)

### Kara Liste (Blacklist)

- Çok fazla başarısız bağlantı deneyen cihazlar kara listeye alınır
- Kara listedeki cihazların bağlantı istekleri otomatik reddedilir
- Kara liste süresi yapılandırılabilir (varsayılan: 60 dakika)
- Süresi dolan kara liste kayıtları otomatik temizlenir

### Hız Sınırlama (Rate Limiting)

- Cihazların dakikada yapabileceği istek sayısı sınırlandırılır
- Limiti aşan cihazlar geçici olarak hız sınırlamasına tabi tutulur
- Süresi dolan hız sınırlaması kayıtları otomatik temizlenir

### Bağlantı Limitleri

- Maksimum eşzamanlı bağlantı sayısı sınırlandırılabilir
- Bağlantı zaman aşımı süresi (timeout) ayarlanabilir

## Mesaj İşleme

TCP Listener, gelen mesajları işlemek için esnek bir yapı sağlar:

### Mesaj Formatı

Örnek mesaj formatı: `^TİP+DEĞER1+DEĞER2...~`

- `^`: Mesaj başlangıç karakteri
- `+`: Parametre ayırıcı karakteri
- `~`: Mesaj bitiş karakteri

### Handshake (El Sıkışma) Süreci

1. Cihaz IMEI ve iletişim tipi bilgilerini içeren bir handshake mesajı gönderir
2. Sunucu IMEI'yi doğrular
3. Onaylı ise kabul yanıtı, değilse ret yanıtı döndürülür
4. Onaylı cihazlar tam veri göndermeye başlayabilir

### Asenkron İşleme

- Her mesaj asenkron olarak işlenir (`HandleMessageAsync` metodu)
- İşlemler performans için ayrı thread'lerde yapılır
- Uzun işlemler ana sunucu thread'ini bloklamaz

## Yapılandırma Seçenekleri

TCP Listener, çeşitli yapılandırma seçenekleri sunar:

```json
{
  "TcpListenerSettings": {
    "Port": 3456,
    "IpAddress": "0.0.0.0",
    "MaxConnections": 100,
    "ConnectionTimeout": 30000,
    "ClientTimeoutMilliseconds": 60000,
    "BufferSize": 1024,
    "StartChar": "^",
    "DelimiterChar": "+",
    "EndChar": "~",
    "RateLimitPerMinute": 60,
    "MaxUnapprovedConnectionAttempts": 5,
    "BlacklistDurationMinutes": 60
  }
}
```

### Önemli Yapılandırma Seçenekleri

- **Port**: TCP sunucusunun dinleyeceği port numarası (varsayılan: 3456)
- **IpAddress/ListenAddress**: TCP sunucusunun dinleyeceği IP adresi (varsayılan: 0.0.0.0)
- **MaxConnections**: Maksimum eşzamanlı bağlantı sayısı (varsayılan: 100)
- **ConnectionTimeout**: Bağlantı zaman aşımı süresi (milisaniye) (varsayılan: 30000)
- **ClientTimeoutMilliseconds**: İstemci zaman aşımı süresi (milisaniye) (varsayılan: 60000)
- **BufferSize**: Okuma/yazma buffer boyutu (varsayılan: 1024 byte)
- **RateLimitPerMinute**: Dakika başına izin verilen maksimum istek sayısı (varsayılan: 60)
- **MaxUnapprovedConnectionAttempts**: Onaysız bir cihazın kara listeye alınmadan önceki maksimum bağlantı denemesi sayısı (varsayılan: 5)
- **BlacklistDurationMinutes**: Kara listedeki cihazların listede kalma süresi (dakika) (varsayılan: 60)

## Performans Özellikleri

TCP Listener, yüksek performans için tasarlanmıştır:

### Concurrent Veri Yapıları

- `ConcurrentDictionary` kullanılarak thread-safe işlemler sağlanır
- Thread kilitleme (locking) mekanizmaları en aza indirilmiştir
- Asenkron I/O operasyonları (async/await) kullanılır

### Bağlantı Yönetimi

- Bağlantı havuzu otomatik yönetilir
- Kapatılan veya zaman aşımına uğrayan bağlantılar otomatik temizlenir
- CPU kullanımını azaltmak için uygun thread bekleme süreleri

### Bellek Yönetimi

- Sabit boyutlu buffer'lar kullanılır
- Eski kayıtlar düzenli olarak temizlenir
- Task tabanlı asenkron programlama ile hafıza optimizasyonu

### Log Kısıtlama

- Aynı kaynaktan gelen tekrarlayan mesajlar için log kısıtlaması
- Yüksek trafikli ortamlarda log boyutunu kontrol altında tutar

## Kullanım Örnekleri

### Servisi Başlatma

```csharp
// Program.cs veya Startup.cs içinde
services.Configure<TcpListenerSettings>(Configuration.GetSection("TcpListenerSettings"));
services.AddSingleton<IDeviceVerifier, DeviceVerifier>();
services.AddSingleton<IMessageHandler, MessageHandler>();
services.AddHostedService<TcpServer>();
```

### Cihazları Doğrulama

```csharp
// Örnek kullanım
bool isApproved = _deviceVerifier.VerifyDeviceByImei("123456789012345");
```

### Onaylı Cihazları Listeleme

```csharp
// Örnek kullanım
var approvedDevices = _deviceVerifier.GetApprovedDevicesWithDetails();
foreach (var device in approvedDevices)
{
    Console.WriteLine($"IMEI: {device.Imei}, İsim: {device.Name}");
}
```

### Sunucu İstatistiklerini Alma

```csharp
// Örnek kullanım
var stats = _tcpServer.GetStatistics();
Console.WriteLine($"Aktif bağlantılar: {stats.ActiveConnections}");
Console.WriteLine($"Toplam bağlantılar: {stats.TotalConnectionsReceived}");
```

## Sorun Giderme

### Bağlantı Sorunları

1. **Port meşgul hatası**: TCP sunucusu başlatılırken "Port already in use" hatası alırsanız:
   - Belirtilen port numarasının başka bir uygulama tarafından kullanılmadığından emin olun
   - `netstat -ano | findstr PORT_NUMBER` komutunu çalıştırın
   - Farklı bir port numarası yapılandırın

2. **Bağlantı reddedildi**: İstemci bağlantısı reddediliyorsa:
   - Firewall ayarlarınızı kontrol edin
   - İstemci IMEI'sinin onaylı olduğundan emin olun
   - Cihazın kara listede olmadığından emin olun
   - Maksimum bağlantı sayısını kontrol edin

### Performans Sorunları

1. **Yüksek CPU kullanımı**:
   - Buffer boyutunu artırın
   - Eşzamanlı bağlantı sayısını sınırlayın
   - Log seviyesini azaltın (Debug yerine Information veya Warning)

2. **Bellek sızıntıları**:
   - Kapatılan bağlantıların düzgün temizlendiğinden emin olun
   - Uzun süreli çalışan task'ları kontrol edin
   - GC.Collect() çağrısı yaparak belleği manuel temizleyin (yalnızca test amaçlı)

### Log İnceleme

Log dosyalarında aşağıdaki bilgileri arayın:

- "TCP sunucu başlatıldı" - Sunucunun başarıyla başladığını gösterir
- "Yeni bağlantı:" - Yeni bir istemci bağlantısı kurulduğunu gösterir
- "Kara listedeki cihaz bağlantı girişimi:" - Kara listedeki bir cihazın bağlanmaya çalıştığını gösterir
- "Bağlantı kapandı/sonlandırıldı:" - Bir bağlantının kapandığını gösterir
- "Hata:" - Servis çalışırken oluşan hataları gösterir

---

Bu dokümantasyon, TCP Listener servisinin tüm yönlerini kapsamaktadır. Servisi kullanmadan veya değiştirmeden önce bu dokümantasyonu dikkatlice okuyun. Herhangi bir sorunla karşılaşırsanız, lütfen yukarıdaki sorun giderme adımlarını uygulayın veya destek ekibiyle iletişime geçin. 