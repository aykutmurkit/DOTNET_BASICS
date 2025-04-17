# TCP Listener Bileşeni

Bu modül, TCP bağlantıları dinlemek, cihazlarla iletişim kurmak ve belirli protokol kurallarına göre mesajları işlemek için kullanılır.

## Özellikler

- Konfigüre edilebilir port üzerinden TCP bağlantı dinleme (varsayılan: 3456)
- Asenkron bağlantı yönetimi
- Basit text tabanlı protokol desteği
- Cihaz onaylama mekanizması
- Hız sınırlama (rate limiting) ve kara liste mekanizması
- Kapsamlı loglama ve istatistik toplama
- Ölçeklenebilir mimari

## Protokol

TCP Listener, aşağıdaki formatta simple text-based bir protokol kullanır:

```
^TİP+DEĞER1+DEĞER2...~
```

Burada:
- `^` : Mesaj başlangıç karakteri
- `+` : Parametre ayırıcı karakteri
- `~` : Mesaj bitiş karakteri

### Mesaj Tipleri

| Kod | Tip | Açıklama |
|-----|-----|----------|
| 0 | Unknown | Bilinmeyen mesaj tipi |
| 1 | Handshake | El sıkışma mesajı (Format: `^1+IMEI+ILETISIM_TIPI~`) |
| 2 | OpeningScreen | Açılış ekranı mesajı |
| 3 | Settings | Ayarlar mesajı |
| 4 | Logo | Logo mesajı |
| 5 | ClearScreen | Ekran temizleme mesajı |
| 6 | Information | Bilgilendirme mesajı |
| 7 | Bus | Otobüs mesajı |
| 8 | Tram | Tramvay mesajı |
| 9 | Live | Canlı mesaj |
| 10 | Ferry | Vapur mesajı |

### Yanıt Kodları

| Kod | Tip | Açıklama |
|-----|-----|----------|
| 0 | Unknown | Bilinmeyen yanıt kodu |
| 1 | Accept | Kabul edildi |
| 2 | Reject | Reddedildi |

## Kullanım

TCP Listener modülünü kullanmak için Program.cs dosyasında servis olarak ekleyin:

```csharp
// Program.cs dosyasında:
using DeviceApi.TCPListener;

var builder = WebApplication.CreateBuilder(args);

// TCP Listener servisini ekle
builder.Services.AddTcpListener(builder.Configuration);

// ...
```

Uygulama yapılandırmasına özel konfigürasyon eklemek için appsettings.json dosyasını kullanabilirsiniz:

```json
{
  "TcpListenerSettings": {
    "Port": 3456,
    "IpAddress": "0.0.0.0",
    "MaxConnections": 100,
    "ConnectionTimeout": 30000,
    "BufferSize": 1024,
    "StartChar": "^",
    "DelimiterChar": "+",
    "EndChar": "~"
  }
}
```

## API Kullanımı

TCP Listener'ın durumunu ve istatistiklerini API üzerinden kontrol etmek için controller üzerinden erişebilirsiniz:

```csharp
// Controllerınızda:
[ApiController]
[Route("api/[controller]")]
public class TcpListenerController : ControllerBase
{
    private readonly ITcpServer _tcpServer;
    
    public TcpListenerController(ITcpServer tcpServer)
    {
        _tcpServer = tcpServer;
    }
    
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var stats = _tcpServer.GetStatistics();
        return Ok(stats);
    }
    
    // ...
}
```

## Test Etme

TCP Listener'ı test etmek için Hercules gibi TCP istemci araçları kullanabilirsiniz:

1. Hercules TCP Client'ı açın
2. IP adresi olarak sunucu IP'sini girin
3. Port olarak 3456 (veya konfigüre ettiğiniz port) girin
4. Bağlanın
5. Aşağıdaki gibi bir mesaj gönderin: `^1+123456789012345+1~` (Handshake mesajı örneği)

## Mimarisi

TCP Listener bileşeni aşağıdaki alt bileşenlerden oluşur:

- **Configuration**: Konfigürasyon modelleri
- **Connection**: TCP bağlantı yönetimi
- **Messaging**: Mesaj işleme ve yanıt üretme
- **Protocol**: Protokol sabitleri ve kuralları
- **Security**: Cihaz doğrulama ve güvenlik kontrolleri

## Best Practices

- TCP bağlantısı uzun süre açık bırakılmamalıdır, işlem tamamlandığında bağlantıyı kapatın
- Hassas IMEI bilgileri için uygun güvenlik önlemleri alın
- Büyük veri transferleri için mesajları bölün
- Yüksek yoğunluklu ortamlarda rate limiting değerlerini uygun şekilde ayarlayın 